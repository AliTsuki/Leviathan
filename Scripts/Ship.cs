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
    public Vector3 IntendedRotation;
    public Vector3 CurrentRotation;
    public Vector3 NextRotation;
    public Vector3 TiltRotation;
    public int RecentRotationsIndex = 0;
    public static int RecentRotationsIndexMax = 30;
    public float[] RecentRotations;
    public float RecentRotationsAverage = 0f;
    public float TiltAngle = 0f;
    public bool IntendedRotationClockwise = false;
    public float DifferenceAngle = 0f;
    public float IntendedAngle = 0f;
    public float MaxRotationSpeed = 5f;
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

    // Identification fields
    public uint ID;
    public GameController.IFF IFF;
    public bool Alive;


    // Start is called before the first frame update
    public virtual void Start()
    {
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
        this.RecentRotations = new float[RecentRotationsIndexMax];
        this.Alive = true;
        this.Health = this.MaxHealth;
        this.Armor = this.MaxArmor;
        this.Shields = this.MaxShields;
        this.Energy = this.MaxEnergy;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        this.ProcessInputs();
    }

    // Fixed Update is called a fixed number of times per second
    public virtual void FixedUpdate()
    {
        if(this.Alive)
        {
            this.UpdateShipState();
        }
        if(this.Health <= 0)
        {
            this.Kill();
        }
    }

    // Processes inputs
    public virtual void ProcessInputs()
    {

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
        
    }

    // Turns the ship
    public virtual void TurnShip()
    {
        // Get current rotation
        this.CurrentRotation = this.ShipObject.transform.rotation.eulerAngles;
        this.CurrentRotation.y += 360;
        // Check if the turn should be clockwise or anti-cw
        this.DifferenceAngle = MathOps.Modulo(this.CurrentRotation.y - this.IntendedRotation.y, 360);
        if(this.DifferenceAngle > 180)
        {
            this.DifferenceAngle -= 180;
            this.IntendedRotationClockwise = true;
        }
        else
        {
            this.IntendedRotationClockwise = false;
        }
        // If turning angle is greater than max rotation speed allows, rotate by max rot speed
        if(this.DifferenceAngle >= this.MaxRotationSpeed)
        {
            // If clockwise, add rotation speed
            if(this.IntendedRotationClockwise == true)
            {
                this.NextRotation.y = MathOps.Modulo(this.CurrentRotation.y + this.MaxRotationSpeed, 360);
                this.ShipObject.transform.rotation = Quaternion.Euler(this.NextRotation);
                this.RecentRotations[this.RecentRotationsIndex] = this.MaxRotationSpeed;
            }
            // If anticlockwise, subtract rotation speed
            else
            {
                this.NextRotation.y = MathOps.Modulo(this.CurrentRotation.y - this.MaxRotationSpeed, 360);
                this.ShipObject.transform.rotation = Quaternion.Euler(this.NextRotation);
                this.RecentRotations[this.RecentRotationsIndex] = -this.MaxRotationSpeed;
            }
        }
        // If turning angle is less than max rotation speed limit, immediately rotate to intended rotation
        else if(this.DifferenceAngle < this.MaxRotationSpeed && this.DifferenceAngle > 0)
        {
            this.ShipObject.transform.rotation = Quaternion.Euler(this.IntendedRotation);
            // Add or subtract recent rotation for less than a max rot amount
            if(this.CurrentRotation.y - this.IntendedRotation.y - 180 < 0)
            {
                this.RecentRotations[this.RecentRotationsIndex] = 2.5f;
            }
            else
            {
                this.RecentRotations[this.RecentRotationsIndex] = -2.5f;
            }
        }
        // If turning angle is 0, set the recent rotation to 0
        else
        {
            this.RecentRotations[this.RecentRotationsIndex] = 0;
        }
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
        this.RecentRotationsAverage = 0;
        for(int i = 0; i < RecentRotationsIndexMax; i++)
        {
            this.RecentRotationsAverage += this.RecentRotations[i];
        }
        this.RecentRotationsAverage /= RecentRotationsIndexMax;
        this.CurrentRotation = this.ShipObject.transform.rotation.eulerAngles;
        this.TiltAngle = -(this.RecentRotationsAverage * 5);
        this.TiltRotation = new Vector3(this.CurrentRotation.x, this.CurrentRotation.y, this.TiltAngle);
        this.ShipObject.transform.rotation = Quaternion.Euler(this.TiltRotation);
    }

    // Accelerates the ship
    public virtual void AccelerateShip()
    {
        if(this.ImpulseInput && !this.WarpInput)
        {
            if(this.ShipRigidbody.velocity.magnitude < this.MaxImpulseSpeed)
            {
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration));
                this.ImpulseParticleMain.startSpeed = 2.8f;
                this.WarpParticleMain.startLifetime = 0f;
                AudioController.FadeIn(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMaxVol);
                AudioController.FadeOut(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
            }
        }
        else if(this.WarpInput)
        {
            if(this.ShipRigidbody.velocity.magnitude < this.MaxWarpSpeed)
            {
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration * this.WarpAccelMultiplier));
                this.ImpulseParticleMain.startSpeed = 5f;
                this.WarpParticleMain.startSpeed = 10f;
                this.WarpParticleMain.startLifetime = 1f;
                AudioController.FadeIn(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMaxVol);
                AudioController.FadeOut(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
            }
        }
        else
        {
            this.ImpulseParticleMain.startSpeed = 1f;
            this.WarpParticleMain.startSpeed = 1f;
            this.WarpParticleMain.startLifetime = 0f;
            AudioController.FadeOut(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
            AudioController.FadeOut(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
        }
    }

    // Uses abilities: fire weapons, bombs, use shield and scanner
    public virtual void UseAbilities()
    {
        this.FireMainGun();
    }

    // Fires main guns
    public virtual void FireMainGun()
    {
        if(Time.time - this.LastShotFireTime >= this.ShotCooldownTime)
        {
            this.WeaponOnCooldown = false;
        }
        if(this.FireInput)
        {
            if(!this.WeaponOnCooldown)
            {
                this.GunBarrelLightsObject.SetActive(true);
                GameController.SpawnProjectile(GameController.IFF.friend, this.ShotDamage, this.GunBarrelObject.transform.position, this.GunBarrelObject.transform.rotation, this.ShipRigidbody.velocity, this.ShotSpeed, this.ShotLifetime);
                this.LastShotFireTime = Time.time;
                this.WeaponOnCooldown = true;
                this.GunAudio.Play();
            }
            else
            {
                this.GunBarrelLightsObject.SetActive(false);
            }
        }
        else
        {
            this.GunBarrelLightsObject.SetActive(false);
        }
    }

    // Called when receiving collision from projectile
    public virtual void ReceivedCollision(float _damage)
    {
        this.Health -= _damage;
    }

    // Called when entity is destroyed
    public virtual void Kill()
    {
        this.Alive = false;
        GameObject.Destroy(this.ShipObject);
    }
}
