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
    // Speed/Acceleration
    public float ImpulseAcceleration;
    public float WarpAccelMultiplier;
    public float MaxImpulseSpeed;
    public float MaxWarpSpeed;
    // Weapon stats
    public float ShotDamage;
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
        this.AccelerateShip();
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

    // Accelerates the ship
    public virtual void AccelerateShip()
    {
        // If impulse engine is activated by player input or AI and warp engine is not activated
        if(this.ImpulseInput && !this.WarpInput)
        {
            // TODO: Fix speed limit check
            // Check if at speed limit
            if(this.ShipRigidbody.velocity.magnitude < this.MaxImpulseSpeed)
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
            if(this.ShipRigidbody.velocity.magnitude < this.MaxWarpSpeed)
            {
                // Accelerate at warp speed
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration * this.WarpAccelMultiplier));
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
            if(this.WeaponOnCooldown == false)
            {
                // Turn on gun lights and spawn a projectile, set last fire time, put weapon on cooldown, and play gun audio
                this.GunBarrelLightsObject.SetActive(true);
                GameController.SpawnProjectile(this.IFF, this.ShotDamage, this.GunBarrelObject.transform.position, Quaternion.Euler(0, this.GunBarrelObject.transform.rotation.eulerAngles.y, 0), this.ShipRigidbody.velocity, this.ShotSpeed, this.ShotLifetime);
                this.LastShotFireTime = Time.time;
                this.WeaponOnCooldown = true;
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
    public virtual void ReceivedCollision(float _damage)
    {
        // Subtract projectile damage from health
        this.Health -= _damage;
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
