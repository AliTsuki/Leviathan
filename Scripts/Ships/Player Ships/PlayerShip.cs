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
    public PlayerShipType Type;


    // Processes inputs
    public override void ProcessInputs()
    {
        // Get inputs from PlayerInput
        this.AimInput = PlayerInput.AimInput;
        this.ImpulseEngineInput = PlayerInput.ImpulseEngineInput;
        this.WarpEngineInput = PlayerInput.WarpEngineInput;
        this.MainGunInput = PlayerInput.MainGunInput;
        this.AbilityInput[0] = PlayerInput.Ability1Input;
        this.AbilityInput[1] = PlayerInput.Ability2Input;
        this.AbilityInput[2] = PlayerInput.Ability3Input;
        this.PauseInput = PlayerInput.PauseButtonInput;
        // Pause game if pause is pressed
        if(this.PauseInput == true)
        {
            GameController.CurrentGameState = GameController.GameState.Paused;
        }
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
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

    // Called when ship is destroyed by damage, grants XP
    public override void Kill()
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
        GameController.CurrentGameState = GameController.GameState.GameOver;
    }
}
