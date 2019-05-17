using UnityEngine;

public class Ship
{
    // GameObjects and Components
    public GameObject ShipObject;
    public GameObject ShipObjectPrefab;
    public GameObject ImpulseEngineObject;
    public GameObject WarpEngineObject;
    public GameObject GunBarrelObject;
    public GameObject GunBarrelLightsObject;
    public Rigidbody ShipRigidbody;
    public ParticleSystem ImpulseParticleSystem;
    public ParticleSystem WarpParticleSystem;
    public ParticleSystem.MainModule ImpulseParticleMain;
    public ParticleSystem.MainModule WarpParticleMain;
    public AudioSource ImpulseAudio;
    public AudioSource WarpAudio;
    public AudioSource GunAudio;

    // Inputs
    public float HorizontalInput;
    public float VerticalInput;
    public bool ImpulseInput;
    public bool WarpInput;
    public bool FireInput;
    public bool BombInput;
    public bool ShieldInput;
    public bool ScannerInput;
    public bool PauseInput;
    public bool StrafeInput;

    // Fields not modified by player equipment/level
    public Vector3 StartingPosition;
    public Vector3 CurrentRotationForwardVector;
    public Vector3 NextRotationForwardVector;
    public Quaternion IntendedRotation;
    public Quaternion CurrentRotation;
    public Quaternion NextRotation;
    public Quaternion TiltRotation;
    public float CurrentRotationAngle;
    public float NextRotationAngle;
    public int RecentRotationsIndex = 0;
    public static int RecentRotationsIndexMax = 30;
    public float[] RecentRotations;
    public float RecentRotationsAverage = 0f;
    public float TiltAngle = 0f;
    public float IntendedAngle = 0f;
    public float MaxRotationSpeed = 0.1f;
    public float LastShotFireTime = 0f;
    public bool WeaponOnCooldown = false;
    public float ImpulseEngineAudioStep = 0.05f;
    public float ImpulseEngineAudioMinVol = 0.1f;
    public float ImpulseEngineAudioMaxVol = 0.5f;
    public float WarpEngineAudioStep = 0.05f;
    public float WarpEngineAudioMinVol = 0f;
    public float WarpEngineAudioMaxVol = 1f;

    // Ship stats
    // Health/Armor/Shields
    public float Health;
    public float MaxHealth;
    public float Armor;
    public float MaxArmor;
    public float Shields;
    public float MaxShields;
    // Current/Max energy
    public float Energy;
    public float MaxEnergy;
    public float EnergyRegenSpeed;
    // Speed/Acceleration
    public float ImpulseAcceleration;
    public float WarpAccelMultiplier;
    public float StrafeAcceleration;
    public float MaxImpulseSpeed;
    public float MaxWarpSpeed;
    public float MaxStrafeSpeed;
    // Weapon stats
    public uint ProjectileType;
    public float ShotDamage;
    public float ShotAccuracy;
    public float ShotSpeed;
    public float ShotLifetime;
    public float ShotCurvature;
    // Cooldowns
    public float ShotCooldownTime;
    public float ShieldCooldownTime;
    public float BombCooldownTime;
    public float ScannerCooldownTime;
    // Energy cost
    public float WarpEnergyCost;
    public float ShotEnergyCost;

    // AI fields
    public Ship CurrentTarget;
    public float MaxTargetAcquisitionRange;
    public float MaxOrbitRange;
    public float MaxWeaponsRange;
    public bool StrafeRight;
    public bool ResetStrafeDirection;

    // Identification fields
    public uint ID;
    public GameController.IFF IFF;
    public bool Alive;
    public bool IsPlayer;


    // Start is called before the first frame update
    public virtual void Start()
    {
        // Set up universal ship fields
        this.ShipObject.name = $@"{this.ID}";
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ImpulseEngineObject = this.ShipObject.transform.Find("Impulse Engine").gameObject;
        this.WarpEngineObject = this.ShipObject.transform.Find("Warp Engine").gameObject;
        this.GunBarrelObject = this.ShipObject.transform.Find("Gun Barrel").gameObject;
        this.GunBarrelLightsObject = this.GunBarrelObject.transform.Find("Gun Barrel Lights").gameObject;
        this.ImpulseParticleSystem = this.ImpulseEngineObject.GetComponent<ParticleSystem>();
        this.ImpulseParticleMain = this.ImpulseParticleSystem.main;
        this.WarpParticleSystem = this.WarpEngineObject.GetComponent<ParticleSystem>();
        this.WarpParticleMain = this.WarpParticleSystem.main;
        this.ImpulseAudio = this.ImpulseEngineObject.GetComponent<AudioSource>();
        this.WarpAudio = this.WarpEngineObject.GetComponent<AudioSource>();
        this.GunAudio = this.GunBarrelObject.GetComponent<AudioSource>();
        this.Alive = true;
        this.Health = this.MaxHealth;
        this.Armor = this.MaxArmor;
        this.Shields = this.MaxShields;
        this.Energy = this.MaxEnergy;
        this.RecentRotations = new float[RecentRotationsIndexMax];
    }

    // Update is called once per frame
    public virtual void Update()
    {
        this.ProcessInputs();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public virtual void FixedUpdate()
    {
        // If ship is alive, accept inputs from player input or AI
        if(this.Alive == true)
        {
            this.UpdateShipState();
        }
        // If ship health has reached 0, run Kill method
        if(this.Health <= 0)
        {
            this.Kill();
        }
    }

    // Processes inputs
    public virtual void ProcessInputs()
    {
        // Each subclass has its own AI for this method
    }

    // Updates the state of the ship, turning, accelerating, using weapons etc.
    public virtual void UpdateShipState()
    {
        this.RotateShip();
        this.EnergyRegen();
        this.AccelerateShip();
        this.StrafeShip();
        this.UseAbilities();
    }

    // Rotates the ship according to player input
    public virtual void RotateShip()
    {
        this.GetIntendedRotation();
        this.TurnShip();
        this.LeanShip();
    }

    // Gets intended rotation
    public virtual void GetIntendedRotation()
    {
        // Each subclass has its own AI for this method
    }

    // Turns the ship
    public virtual void TurnShip()
    {
        // Get current rotation
        this.CurrentRotation = this.ShipObject.transform.rotation;
        // Get next rotation by using intended rotation and max rotation speed
        this.NextRotation = Quaternion.Lerp(this.CurrentRotation, this.IntendedRotation, this.MaxRotationSpeed);
        // Rotate to next rotation
        this.ShipObject.transform.rotation = this.NextRotation;
        // Get recent rotation angle amount for tilting
        // Get forward vector for each rotation
        this.CurrentRotationForwardVector = this.CurrentRotation * Vector3.forward;
        this.NextRotationForwardVector = this.NextRotation * Vector3.forward;
        // Get a numeric angle for each vector on the X-Z plane
        this.CurrentRotationAngle = Mathf.Atan2(this.CurrentRotationForwardVector.x, this.CurrentRotationForwardVector.z) * Mathf.Rad2Deg;
        this.NextRotationAngle = Mathf.Atan2(this.NextRotationForwardVector.x, this.NextRotationForwardVector.z) * Mathf.Rad2Deg;
        // Store recent rotation amount to be used for leaning ship
        this.RecentRotations[this.RecentRotationsIndex] = Mathf.DeltaAngle(this.CurrentRotationAngle, this.NextRotationAngle);
        // Go to next recent rotation index
        this.RecentRotationsIndex++;
        // If recent rotation index has hit the end of the array, go back to the start
        if(this.RecentRotationsIndex == RecentRotationsIndexMax)
        {
            this.RecentRotationsIndex = 0;
        }
    }

    // Lean the ship during turns
    public virtual void LeanShip()
    {
        // Reset recent rotations average
        this.RecentRotationsAverage = 0;
        // Loop through recent rotations and add them together
        for(int i = 0; i < RecentRotationsIndexMax; i++)
        {
            this.RecentRotationsAverage += this.RecentRotations[i];
        }
        // Divide recent rotations by max index to get average
        this.RecentRotationsAverage /= RecentRotationsIndexMax;
        // Get current rotation
        this.CurrentRotation = this.ShipObject.transform.rotation;
        // Amplify tilt angle by average multiplied by some amount
        this.TiltAngle = -(this.RecentRotationsAverage * 5);
        // Get next tilt rotation
        this.TiltRotation = Quaternion.Euler(0, this.CurrentRotation.eulerAngles.y, this.TiltAngle);
        // Rotate ship to new tilt rotation
        this.ShipObject.transform.rotation = this.TiltRotation;
    }

    // Regenerates energy
    public virtual void EnergyRegen()
    {
        // Prevent energy from going below 0
        if(this.Energy < 0)
        {
            this.Energy = 0f;
        }
        // Add energy regen
        this.Energy += this.EnergyRegenSpeed;
        // Prevent energy from going above max
        if(this.Energy > this.MaxEnergy)
        {
            this.Energy = this.MaxEnergy;
        }
    }

    // Accelerates the ship
    public virtual void AccelerateShip()
    {
        // If impulse engine is activated by player input or AI and warp engine is not activated
        if(this.ImpulseInput && !this.WarpInput)
        {
            // Check if at speed limit
            if(this.ShipRigidbody.velocity.magnitude < this.MaxImpulseSpeed || Vector3.Dot(this.ShipRigidbody.velocity.normalized, this.ShipObject.transform.forward) < 0.7f)
            {
                // Accelerate forward
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration));
                // Modify particle effects
                this.ImpulseParticleMain.startSpeed = 2.8f;
                this.WarpParticleMain.startLifetime = 0f;
                // Fade in/out audio
                AudioController.FadeIn(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMaxVol);
                AudioController.FadeOut(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
            }
        }
        // If warp engine is activated by player input or AI
        else if(this.WarpInput)
        {
            // Check if at speed limit
            if(this.ShipRigidbody.velocity.magnitude < this.MaxWarpSpeed || Vector3.Dot(this.ShipRigidbody.velocity.normalized, this.ShipObject.transform.forward) < 0.5f)
            {
                // Accelerate at warp speed
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration * this.WarpAccelMultiplier));
                // Subtract warp energy cost
                this.Energy -= this.WarpEnergyCost;
                // Modify particle effects
                this.ImpulseParticleMain.startSpeed = 5f;
                this.WarpParticleMain.startSpeed = 10f;
                this.WarpParticleMain.startLifetime = 1f;
                // Fade in/out audio
                AudioController.FadeIn(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMaxVol);
                AudioController.FadeOut(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
            }
        }
        // If no engines are active
        else
        {
            // Turn particles back to default
            this.ImpulseParticleMain.startSpeed = 1f;
            this.WarpParticleMain.startSpeed = 1f;
            this.WarpParticleMain.startLifetime = 0f;
            // Fade out audio
            AudioController.FadeOut(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
            AudioController.FadeOut(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
        }
    }

    // Strafe ship if within weapons range, strafe input is only set by NPCs
    public virtual void StrafeShip()
    {
        // If strafe direction should be reset
        if(this.ResetStrafeDirection == true)
        {
            // Get random number 0 or 1, if 1 strafe right, if 0 strafe left
            if(GameController.r.Next(0, 1) == 1)
            {
                this.StrafeRight = true;
            }
            else
            {
                this.StrafeRight = false;
            }
        }
        // If strafe is activated by AI and ship is below max strafing speed
        if(this.StrafeInput == true && this.ShipRigidbody.velocity.magnitude < this.MaxStrafeSpeed)
        {
            // If strafe right add positive strafe acceleration
            if(this.StrafeRight == true)
            {
                // Strafe
                this.ShipRigidbody.AddRelativeForce(new Vector3(this.StrafeAcceleration, 0, 0));
            }
            // If strafe left add negative strafe acceleration
            else
            {
                // Strafe
                this.ShipRigidbody.AddRelativeForce(new Vector3(-this.StrafeAcceleration, 0, 0));
            }
            
        }
    }

    // Uses ship abilities
    public virtual void UseAbilities()
    {
        this.FireMainGun();
    }

    // Fires main guns
    public virtual void FireMainGun()
    {
        // Take weapon off cooldown if it has been long enough since last fired
        if(Time.time - this.LastShotFireTime >= this.ShotCooldownTime)
        {
            this.WeaponOnCooldown = false;
        }
        // If weapons fire input is active by player input or AI
        if(this.FireInput)
        {
            // Check if weapon is on cooldown
            if(this.WeaponOnCooldown == false && this.Energy >= this.ShotEnergyCost)
            {
                // Spawn a projectile
                GameController.SpawnProjectile(this.IFF, this.ProjectileType, this.ShotDamage, this.GunBarrelObject.transform.position, Quaternion.Euler(0, this.GunBarrelObject.transform.rotation.eulerAngles.y, 0), this.ShipRigidbody.velocity, this.ShotSpeed, this.ShotLifetime);
                // Set last shot time
                this.LastShotFireTime = Time.time;
                // Put weapon on cooldown
                this.WeaponOnCooldown = true;
                // Subtract energy for shot
                this.Energy -= this.ShotEnergyCost;
                // Turn on gun lights
                this.GunBarrelLightsObject.SetActive(true);
                // Play gun audio
                this.GunAudio.Play();
            }
            // If weapon is on cooldown
            else
            {
                // Turn gun lights off
                this.GunBarrelLightsObject.SetActive(false);
            }
        }
        // If weapons fire input is not active
        else
        {
            // Turn gun lights off
            this.GunBarrelLightsObject.SetActive(false);
        }
    }

    // Called when receiving collision from projectile
    public virtual void ReceivedCollisionFromProjectile(float _damage)
    {
        // TODO: Add regenerating shields
        // TODO: Add TakeDamage() method that handles whether damage should be applied to shield/health etc
        // Subtract projectile damage from health
        this.Health -= _damage;
    }

    // Called when receiving collision from ship
    public virtual void ReceivedCollisionFromShip(Vector3 _collisionVelocity)
    {
        // Apply velocity received from collision
        this.ShipRigidbody.AddRelativeForce(_collisionVelocity * 0.35f, ForceMode.Impulse);
        // Take impact damage
        this.Health -= _collisionVelocity.magnitude * 0.1f;
    }

    // Called when ship is destroyed by damage
    public virtual void Kill()
    {
        // Set to not alive and destroy GameObject
        this.Alive = false;
        UIController.RemoveHealthbar(this.ID);
        GameObject.Destroy(this.ShipObject);
    }

    // Called when ship is too far away from player
    public virtual void Despawn()
    {
        // Set to not alive and destroy GameObject
        this.Alive = false;
        UIController.RemoveHealthbar(this.ID);
        GameObject.Destroy(this.ShipObject);
    }

    // Called when a ship has no target and nothing else to do
    public virtual void Wander()
    {

    }
}
