using UnityEngine;

// Controls the player ships
public abstract class PlayerShip : Ship
{
    // Player ship types
    public enum PlayerShipType
    {
        Bomber,
        Engineer,
        Scout
    }
    protected PlayerShipType Type;


    // Processes inputs
    protected override void ProcessInputs()
    {
        // Get inputs from PlayerInput
        this.MoveInput.Set(PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.MoveInputXAxis], PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.MoveInputYAxis]);
        this.AimInput.Set(PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.AimInputXAxis], PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.AimInputYAxis]);
        this.WarpEngineInput = PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.Warp];
        this.MainGunInput = (PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.MainGun] == 1f) ? true : false;
        this.AbilityInput[0] = (PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.Ability1] == 1f) ? true : false;
        this.AbilityInput[1] = (PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.Ability2] == 1f) ? true : false;
        this.AbilityInput[2] = (PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.Ability3] == 1f) ? true : false;
        this.PauseInput = (PlayerInput.CurrentInputValues[InputBinding.GameInputsEnum.Pause] == 1f) ? true : false;
    }

    // Gets intended rotation
    protected override void GetIntendedRotation()
    {
        // If joystick is pointed in some direction
        if(this.AimInput.x != 0 || this.AimInput.y != 0)
        {
            // Set intended angle to dual-argument arc-tangent of vertical input and horizontal input, finally multiply by constant to convert from rad to deg, add 90 to compensate for rotation of camera
            this.IntendedAngle = (Mathf.Atan2(this.AimInput.y, this.AimInput.x) * Mathf.Rad2Deg) + 90;
            // Set intended rotation to intended angle on y axis
            this.IntendedRotation = Quaternion.Euler(new Vector3(0, this.IntendedAngle, 0));
        }
    }

    // Accelerates the ship
    protected override void AccelerateShip()
    {
        // If input set to move in some direction and warp engine is not activated
        if((this.MoveInput.x != 0f || this.MoveInput.y != 0f) && this.WarpEngineInput <= 0f)
        {
            // If movement style is screenspace, add force based on worldspace
            if(PlayerInput.MovementStyle == PlayerInput.MovementStyleEnum.ScreenSpace)
            {
                this.ShipRigidbody.AddForce(this.Stats.ImpulseAcceleration * this.MoveInput.x, 0f, this.Stats.ImpulseAcceleration * this.MoveInput.y);
            }
            // If movement style is tank style, add force relative to ship rotation
            else
            {
                this.ShipRigidbody.AddRelativeForce(this.Stats.ImpulseAcceleration * this.MoveInput.x, 0f, this.Stats.ImpulseAcceleration * this.MoveInput.y);
            }
            // If current magnitude of velocity is beyond speed limit for impulse power
            if(this.ShipRigidbody.velocity.magnitude > this.Stats.MaxImpulseSpeed)
            {
                // Linearly interpolate velocity toward speed limit
                this.ShipRigidbody.velocity = Vector3.Lerp(this.ShipRigidbody.velocity,
                    Vector3.ClampMagnitude(this.ShipRigidbody.velocity, this.Stats.MaxImpulseSpeed), 0.05f);
            }
            // Loop through engines
            for(int i = 0; i < this.Stats.EngineCount; i++)
            {
                // Modify particle fx
                this.ImpulseParticleSystemMains[i].startSpeed = 2.8f;
                // Audio fadein
                AudioController.FadeIn(this.ImpulseAudioSources[i], ImpulseEngineAudioStep, ImpulseEngineAudioMaxVol);
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Modify warp particle fx
                    this.WarpParticleSystemMains[i].startLifetime = 0f;
                    // Warp audio fadeout
                    AudioController.FadeOut(this.WarpAudioSources[i], WarpEngineAudioStep, WarpEngineAudioMinVol);
                }
            }
        }
        // If warp engine is activated by player input or AI
        else if(this.WarpEngineInput > 0f)
        {
            // If movement style is screenspace, add force based on worldspace using warp multiplier
            if(PlayerInput.MovementStyle == PlayerInput.MovementStyleEnum.ScreenSpace)
            {
                this.ShipRigidbody.AddForce(this.Stats.ImpulseAcceleration * this.MoveInput.x * this.Stats.WarpAccelerationMultiplier, 0f, this.Stats.ImpulseAcceleration * this.MoveInput.y * this.Stats.WarpAccelerationMultiplier);
            }
            // If movement style is tank style, add force relative to ship rotation using warp multiplier
            else
            {
                this.ShipRigidbody.AddRelativeForce(this.Stats.ImpulseAcceleration * this.MoveInput.x, 0f, this.Stats.ImpulseAcceleration * this.MoveInput.y);
            }
            // Clamp magnitude at maximum warp speed
            this.ShipRigidbody.velocity = Vector3.ClampMagnitude(this.ShipRigidbody.velocity, this.Stats.MaxWarpSpeed);
            // Subtract warp energy cost
            this.SubtractEnergy(this.Stats.WarpEnergyCost);
            // Loop through engines
            for(int i = 0; i < this.Stats.EngineCount; i++)
            {
                // Modify particle fx
                this.ImpulseParticleSystemMains[i].startSpeed = 5f;
                // Audio fadeout
                AudioController.FadeOut(this.ImpulseAudioSources[i], ImpulseEngineAudioStep, ImpulseEngineAudioMinVol);
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Modify warp particle fx
                    this.WarpParticleSystemMains[i].startSpeed = 20f;
                    this.WarpParticleSystemMains[i].startLifetime = 1f;
                    // Warp audio fadein
                    AudioController.FadeIn(this.WarpAudioSources[i], WarpEngineAudioStep, WarpEngineAudioMaxVol);
                }
            }
        }
        // If no engines are active
        else
        {
            // Loop through engines
            for(int i = 0; i < this.Stats.EngineCount; i++)
            {
                // Turn particles back to default
                this.ImpulseParticleSystemMains[i].startSpeed = 1f;
                // Audio fadeout to default
                AudioController.FadeOut(this.ImpulseAudioSources[i], ImpulseEngineAudioStep, ImpulseEngineAudioMinVol);
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Warp particle fx to default
                    this.WarpParticleSystemMains[i].startSpeed = 0f;
                    this.WarpParticleSystemMains[i].startLifetime = 0f;
                    // Warp audio fadeout
                    AudioController.FadeOut(this.WarpAudioSources[i], WarpEngineAudioStep, WarpEngineAudioMinVol);
                }
            }
        }
    }

    // Called when ship is destroyed by damage, grants XP
    protected override void Kill()
    {
        // Set to not alive
        this.Alive = false;
        // Reset cooldowns
        for(int i = 0; i < 3; i++)
        {
            this.AbilityActive[i] = false;
            this.AbilityOnCooldown[i] = false;
        }
        // Create an explosion
        this.Explosion = GameObject.Instantiate(this.ExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(this.Explosion, 1f);
        // If this is player
        // Destroy ship objects
        for(int i = 0; i < this.ShipObject.transform.childCount; i++)
        {
            GameObject.Destroy(this.ShipObject.transform.GetChild(i).gameObject);
        }
        // Show game over screen
        UIController.OpenGameOverPopUp(true);
    }
}
