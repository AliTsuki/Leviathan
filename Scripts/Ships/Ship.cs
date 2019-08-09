using UnityEngine;

// Controls ships, parent class to all ship types
public abstract partial class Ship
{
    // GameObjects and Components
    public GameObject ShipObject { get; protected set; }
    protected GameObject ShipObjectPrefab;
    protected Rigidbody ShipRigidbody;
    // Engine Objects
    protected GameObject[] ImpulseEngineObjects;
    protected GameObject[] WarpEngineObjects;
    protected ParticleSystem[] ImpulseParticleSystems;
    protected ParticleSystem[] WarpParticleSystems;
    protected ParticleSystem.MainModule[] ImpulseParticleSystemMains;
    protected ParticleSystem.MainModule[] WarpParticleSystemMains;
    protected AudioSource[] ImpulseAudioSources;
    protected AudioSource[] WarpAudioSources;
    // Gun Objects
    protected GameObject[] GunBarrelObjects;
    protected GameObject[] GunBarrelLightsObjects;
    protected AudioSource[] GunAudioSources;
    // Shield Object
    protected AudioSource ShieldRegenAudio;
    // Particle Objects
    protected GameObject ProjectileShieldStrikePrefab;
    protected GameObject ProjectileHullStrikePrefab;
    protected GameObject ProjectileShieldStrike;
    protected GameObject ProjectileHullStrike;
    protected GameObject ExplosionPrefab;
    protected GameObject Explosion;
    protected GameObject ElectricityEffectPrefab;
    protected GameObject ElectricityEffect;

    // Inputs
    protected Float2 MoveInput = new Float2();
    protected Float2 AimInput = new Float2();
    protected float WarpEngineInput = 0f;
    protected bool MainGunInput = false;
    protected bool[] AbilityInput = new bool[3];
    public bool PauseInput = false;
    // AI-only Inputs
    protected bool ImpulseInput = false;
    protected bool StrafeInput = false;

    // Rotation fields
    protected Vector3 StartingPosition = new Vector3();
    protected Vector3 CurrentRotationForwardVector = new Vector3();
    protected Vector3 NextRotationForwardVector = new Vector3();
    protected Quaternion IntendedRotation = new Quaternion();
    protected Quaternion CurrentRotation = new Quaternion();
    protected Quaternion NextRotation = new Quaternion();
    protected Quaternion TiltRotation = new Quaternion();
    protected float CurrentRotationAngle = 0f;
    protected float NextRotationAngle = 0f;
    protected int RecentRotationsIndex = 0;
    protected static int RecentRotationsIndexMax = 30;
    protected float[] RecentRotations = new float[RecentRotationsIndexMax];
    protected float RecentRotationsAverage = 0f;
    protected float TiltAngle = 0f;
    protected float IntendedAngle = 0f;

    // Cooldown times
    protected float LastGunFireTime = 0f;
    protected float LastDamageTakenTime = 0f;
    protected float[] LastAbilityActivatedTime = new float[3];
    public float[] LastAbilityCooldownStartedTime { get; protected set; } = new float[3];

    // On cooldown bools
    protected bool GunOnCooldown = false;
    protected bool ShieldOnCooldown = false;
    public bool[] AbilityOnCooldown { get; protected set; } = new bool[3];
    public bool[] AbilityActive { get; protected set; } = new bool[3];

    public class ShipStats
    {
        // Ship stats
        // --Health/Armor/Shields
        public float Health; // Current health value
        public float MaxHealth; // Maximum health value
        public float Armor; // Divide by 100 and apply percent as damage reduction modifier to ship to ship impact damage
        public float Shields; // Current shield value
        public float MaxShields; // Maximum shield value
        public float ShieldRegenSpeed; // Amount of shield to regenerate per game tick
        // --Current/Max energy
        public float Energy; // Current energy value
        public float MaxEnergy; // Maximum energy value
        public float EnergyRegenSpeed; // Amount of energy to regenerate per game tick
        // --Energy costs
        public float WarpEnergyCost; // Energy cost per tick applied by using warp engine
        public float GunEnergyCost; // Energy cost per main gun shot fired (doesn't apply multiple times for multi cannon or multi shot ships)
        // --Acceleration
        public uint EngineCount; // Number of engines on ship (used only for visual FX for thruster fire)
        public float ImpulseAcceleration; // The amount of acceleration to apply per game tick when impulse engine is on
        public float WarpAccelerationMultiplier; // Impulse acceleration is multiplied by this value while warp engine is on
        public float StrafeAcceleration; // The amount of acceleration to apply per game tick while strafe is ongoing (strafe is NPC only)
        // --Max Speed
        public float MaxImpulseSpeed; // Maximum velocity for impulse engines
        public float MaxWarpSpeed; // Maximum velocity for warp engines
        public float MaxStrafeSpeed; // Maximum velocity for strafing (strafe is NPC only)
        public float MaxRotationSpeed; // Maximum rotation amount per tick for ships
        // --Weapon stats
        // ----Main gun
        public uint GunBarrelCount; // Number of gun barrels from which to spawn projectiles
        public uint GunShotProjectileType; // Visual FX identifier for projectiles (references projectile prefab numbering system)
        public float GunCooldownTime; // How long in seconds to wait between each shot fired from main gun
        public uint GunShotAmount; // Amount of projectiles to spawn from each gun barrel
        public float GunShotCurvature; // Maximum rotation per game tick that projectiles can turn when homing on target (values of 0 produce no homing effect)
        public float GunShotSightCone; // Value describing the sight cone within which projectiles can acquire target if they have a curvature above 0 (-1 is full 360°, 0 is 180°, 1 is 0° cone)
        public float GunShotDamage; // Amount of damage each projectile inflicts on target collided with
        public float GunShotAccuracy; // Divide by 100 and use value to determine the maximum degree change that can be randomly assigned each fired projectile (5 means projectile can be ±5° from straight forward)
        public float GunShotSpeed; // Maximum velocity of fired projectile
        public float GunShotLifetime; // Number of seconds projectile lasts before burning out
        // --Cooldowns
        public float ShieldCooldownTime; // How long in seconds the regenerating shield must go without taking damage before it will recharge
        public float[] AbilityDuration = new float[3];
        public float[] AbilityCooldownTime = new float[3];
    }
    public ShipStats Stats { get; protected set; }

    // Defaults for ability changes
    protected float DefaultGunCooldownTime = 0f;
    protected uint DefaultGunShotAmount = 0;
    protected float DefaultGunShotDamage = 0f;
    protected float DefaultGunShotAccuracy = 0f;
    protected float DefaultGunEnergyCost = 0f;
    protected float DefaultEnergyRegenSpeed = 0f;
    protected float DefaultShieldRegenSpeed = 0f;
    protected float DefaultShieldCooldownTime = 0f;
    protected float DefaultMaxRotationSpeed = 0f;
    protected float DefaultMaxImpulseSpeed = 0f;
    protected float DefaultMaxStrafeSpeed = 0f;

    // Status effects
    protected bool IsEMPed = false;
    protected bool IsEMPApplied = false;
    protected float EMPEffectDuration = 0f;
    protected float EMPStartedTime = 0f;

    // --Experience worth
    public uint XP { get; protected set; } = 0;

    // AI fields
    public enum AIType
    {
        None,
        Drone,
        Standard,
        Ramming,
        Broadside,
        Flanker
    }
    public AIType AItype { get; protected set; }
    public Ship CurrentTarget { get; protected set; }
    public bool AIAimAssist { get; protected set; }
    public float MaxTargetAcquisitionRange { get; protected set; }
    public float MaxOrbitRange { get; protected set; }
    public float MaxWeaponsRange { get; protected set; }
    public float MaxAbilityRange { get; protected set; }
    public bool StrafeRight { get; protected set; }
    public bool ResetStrafeDirection { get; protected set; }
    public bool ShouldWander { get; protected set; }
    public bool IsWaiting { get; protected set; }
    public float StartedWaitingTime { get; protected set; }
    public float TimeToWait { get; protected set; }
    public bool IsWanderMove { get; protected set; }
    public float StartedWanderMoveTime { get; protected set; }
    public float TimeToWanderMove { get; protected set; }
    public Ship Parent { get; protected set; }
    public float MaxLeashDistance { get; protected set; }
    public float OrbitParentRange { get; protected set; }
    public bool CanFollowParent { get; protected set; }
    public bool CanAcquireTargets { get; protected set; }
    public bool CanAccelerate { get; protected set; }
    public bool CanStrafe { get; protected set; }
    public bool CanFireGuns { get; protected set; }
    public bool CanUseAbilities { get; protected set; }

    // Identification fields
    public uint ID { get; protected set; }
    public GameController.IFF IFF { get; protected set; }
    public bool Alive { get; protected set; }
    public bool IsPlayer { get; protected set; }

    // Constants
    public const float ImpulseEngineAudioStep = 0.05f;
    public const float ImpulseEngineAudioMinVol = 0.25f;
    public const float ImpulseEngineAudioMaxVol = 1f;
    public const float WarpEngineAudioStep = 0.05f;
    public const float WarpEngineAudioMinVol = 0f;
    public const float WarpEngineAudioMaxVol = 1f;
    // Shader constants
    public const float DamageShaderCooldownTime = 0.5f;


    // Initialize
    public virtual void Initialize()
    {
        // Set up universal ship fields
        this.ShipObject.name = $@"{this.ID}";
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.GetEngineObjects();
        this.GetGunBarrelObjects();
        this.GetParticleFXObjects();
        this.SetupShipDefaultValues();
        // Player specific values
        if(this.IsPlayer == true)
        {
            this.ShieldRegenAudio = this.ShipObject.transform.Find(GameController.ShieldObjectName).GetComponent<AudioSource>();
        }
        // NPC specific values
        else
        {
            this.SetupAI();
        }
    }

    // Update is called once per frame
    public void Update()
    {
        // If ship is alive
        if(this.Alive == true)
        {
            // Process inputs
            this.ProcessInputs();
            // Check health status
            this.CheckHealth();
            // Check Buff/Debuff status
            this.CheckStatusEffects();
        }
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public void FixedUpdate()
    {
        // If ship is alive
        if(this.Alive == true)
        {
            // Update ship state
            this.UpdateShipState();
        }
    }

    // Processes inputs
    protected virtual void ProcessInputs()
    {
        this.ProcessAI();
    }

    // Check health
    protected void CheckHealth()
    {
        // If damage shader has been playing long enough
        if(Time.time - this.LastDamageTakenTime >= DamageShaderCooldownTime)
        {
            // Turn off damage shader
            this.ShowDamageShaderEffect(false);
        }
        // If health is less than or equal to half
        if(this.Stats.Health <= this.Stats.MaxHealth * 0.5f)
        {
            // Spawn fire fx on ship
            this.SpawnFireParticleFX();
            // If this is player ship
            if(this.IsPlayer == true)
            {
                // Show damage vignette
                UIController.ShowHealthDamageEffect();
            }
        }
        // If health has reached 0
        if(this.Stats.Health <= 0f)
        {
            this.Stats.Shields = 0f;
            this.Stats.Health = 0f;
            // Kill ship
            this.Kill();
        }
    }

    // Check effect status
    protected void CheckStatusEffects()
    {
        // If ship is hit by EMP and EMP effect has not been applied yet
        if(this.IsEMPed == true && this.IsEMPApplied == false)
        {
            this.EMPStartedTime = Time.time;
            this.DefaultEnergyRegenSpeed = this.Stats.EnergyRegenSpeed;
            this.DefaultShieldRegenSpeed = this.Stats.ShieldRegenSpeed;
            this.DefaultMaxRotationSpeed = this.Stats.MaxRotationSpeed;
            this.DefaultMaxImpulseSpeed = this.Stats.MaxImpulseSpeed;
            this.DefaultMaxStrafeSpeed = this.Stats.MaxStrafeSpeed;
            this.DefaultGunCooldownTime = this.Stats.GunCooldownTime;
            this.Stats.EnergyRegenSpeed = 0f;
            this.Stats.ShieldRegenSpeed = 0f;
            this.Stats.MaxRotationSpeed = 0f;
            this.Stats.MaxImpulseSpeed = 0f;
            this.Stats.MaxStrafeSpeed = 0f;
            this.Stats.GunCooldownTime = 10f;
            this.Stats.Energy = 0f;
            this.Stats.Shields = 0f;
            this.ElectricityEffect = GameObject.Instantiate(this.ElectricityEffectPrefab, this.ShipObject.transform.position, Quaternion.identity, this.ShipObject.transform);
            GameObject.Destroy(this.ElectricityEffect, this.EMPEffectDuration);
            this.IsEMPApplied = true;
        }
        // If ship is hit by EMP and EMP has already been applied and it has been longer than EMP effect duration since EMP effect was applied
        else if(this.IsEMPed == true && this.IsEMPApplied == true && Time.time - this.EMPStartedTime >= this.EMPEffectDuration)
        {
            this.IsEMPed = false;
            this.IsEMPApplied = false;
            this.Stats.EnergyRegenSpeed = this.DefaultEnergyRegenSpeed;
            this.Stats.ShieldRegenSpeed = this.DefaultShieldRegenSpeed;
            this.Stats.MaxRotationSpeed = this.DefaultMaxRotationSpeed;
            this.Stats.MaxImpulseSpeed = this.DefaultMaxImpulseSpeed;
            this.Stats.MaxStrafeSpeed = this.DefaultMaxStrafeSpeed;
            this.Stats.GunCooldownTime = this.DefaultGunCooldownTime;
        }
    }

    // Updates the state of the ship, turning, accelerating, using weapons etc.
    protected void UpdateShipState()
    {
        this.EnergyRegen();
        this.ShieldRegen();
        this.RotateShip();
        this.AccelerateShip();
        this.StrafeShip();
        this.CheckAbilities();
        this.Wander();
        this.FollowParent();
    }

    // Regenerates energy
    protected void EnergyRegen()
    {
        // Prevent energy from going below 0
        if(this.Stats.Energy < 0f)
        {
            this.Stats.Energy = 0f;
        }
        // If energy is below maximum
        if(this.Stats.Energy < this.Stats.MaxEnergy)
        {
            // Add energy regen
            this.Stats.Energy += this.Stats.EnergyRegenSpeed;
        }
        // Prevent energy from going over maximum
        if(this.Stats.Energy > this.Stats.MaxEnergy)
        {
            this.Stats.Energy = this.Stats.MaxEnergy;
        }
    }

    // Regenerates shield
    protected void ShieldRegen()
    {
        // If time since last taken damage was more than shield cooldown time
        if(Time.time - this.LastDamageTakenTime >= this.Stats.ShieldCooldownTime)
        {
            // Take shield off cooldown
            this.ShieldOnCooldown = false;
        }
        // If shield is not on cooldown and shields are less than maximum
        if(this.ShieldOnCooldown == false && this.Stats.Shields < this.Stats.MaxShields)
        {
            // Remove energy for regenerating shield
            this.SubtractEnergy(Mathf.Min(this.Stats.MaxShields - this.Stats.Shields, this.Stats.ShieldRegenSpeed));
            // Add shield regen speed to shields
            this.Stats.Shields += this.Stats.ShieldRegenSpeed;
            // If this is player and shield regen audio is not currently playing
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == false)
            {
                // Set shield regen audio to max volume
                this.ShieldRegenAudio.volume = 1f;
                // Play shield regen audio
                this.ShieldRegenAudio.Play();
            }
        }
        // If shields are greater than max
        if(this.Stats.Shields > this.Stats.MaxShields)
        {
            // Set shields to max value
            this.Stats.Shields = this.Stats.MaxShields;
            // If this is player and shield regen audio is playing
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == true)
            {
                // Fade out shield regen audio
                AudioController.FadeOut(this.ShieldRegenAudio, 0.25f, 0);
            }
        }
    }

    // Rotates the ship
    protected void RotateShip()
    {
        this.GetIntendedRotation();
        this.TurnShip();
        this.LeanShip();
    }

    // Gets intended rotation
    protected virtual void GetIntendedRotation()
    {
        this.GetIntendedRotationAI();
    }

    // Turns the ship
    protected void TurnShip()
    {
        // Get current rotation
        this.CurrentRotation = this.ShipObject.transform.rotation;
        // Get next rotation by using intended rotation and max rotation speed
        this.NextRotation = Quaternion.Lerp(this.CurrentRotation, this.IntendedRotation, this.Stats.MaxRotationSpeed);
        // Rotate to next rotation
        this.ShipObject.transform.rotation = this.NextRotation;
    }

    // Lean the ship during turns
    protected void LeanShip()
    {
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
        this.TiltAngle = -(this.RecentRotationsAverage * 5f);
        // Get next tilt rotation
        this.TiltRotation = Quaternion.Euler(0f, this.CurrentRotation.eulerAngles.y, this.TiltAngle);
        // Rotate ship to new tilt rotation
        this.ShipObject.transform.rotation = this.TiltRotation;
    }

    // Accelerates the ship
    protected virtual void AccelerateShip()
    {
        // If impulse engine is activated by player input or AI and warp engine is not activated
        if(this.ImpulseInput == true && this.WarpEngineInput <= 0f)
        {
            // Accelerate forward
            this.ShipRigidbody.AddRelativeForce(new Vector3(0f, 0f, this.Stats.ImpulseAcceleration));
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
            // Accelerate forward with warp multiplier to speed
            this.ShipRigidbody.AddRelativeForce(new Vector3(0f, 0f, this.Stats.ImpulseAcceleration * this.Stats.WarpAccelerationMultiplier));
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

    // Strafe ship if within weapons range, note: strafe input is only set by NPCs
    protected virtual void StrafeShip()
    {
        // If strafe direction should be reset
        if(this.ResetStrafeDirection == true)
        {
            // Get random number 0 or 1, if 1 strafe right, if 0 strafe left
            if(GameController.RandomNumGen.Next(0, 2) == 1)
            {
                this.StrafeRight = true;
            }
            else
            {
                this.StrafeRight = false;
            }
        }
        // If strafe is activated by AI and ship is below max strafing speed
        if(this.StrafeInput == true && this.ShipRigidbody.velocity.magnitude < this.Stats.MaxStrafeSpeed)
        {
            // If strafe right add positive strafe acceleration
            if(this.StrafeRight == true)
            {
                // Strafe
                this.ShipRigidbody.AddRelativeForce(new Vector3(this.Stats.StrafeAcceleration, 0f, 0f));
            }
            // If strafe left add negative strafe acceleration
            else
            {
                // Strafe
                this.ShipRigidbody.AddRelativeForce(new Vector3(-this.Stats.StrafeAcceleration, 0f, 0f));
            }
        }
    }

    // Uses abilities: fire weapons, use abilities 1, 2, and 3
    protected void CheckAbilities()
    {
        this.CheckMainGun();
        this.CheckAbility1();
        this.CheckAbility2();
        this.CheckAbility3();
    }

    // Fires main guns
    protected virtual void CheckMainGun()
    {
        // If time since last fired weapon is greater than or equal to cooldown time
        if(Time.time - this.LastGunFireTime >= this.Stats.GunCooldownTime)
        {
            // Take gun off cooldown
            this.GunOnCooldown = false;
        }
        // If weapons fire gun input is activated by player input or AI, the weapon is not on cooldown, and there is more available energy than the cost to fire
        if(this.MainGunInput == true && this.GunOnCooldown == false && this.Stats.Energy >= this.Stats.GunEnergyCost)
        {
            // Loop through gun barrel count
            for(int c = 0; c < this.Stats.GunBarrelCount; c++)
            {
                // Loop through gun shot amount
                for(int a = 0; a < this.Stats.GunShotAmount; a++)
                {
                    // If shot accuracy percentage is above 100, set to 100
                    Mathf.Clamp(this.Stats.GunShotAccuracy, 0f, 100f);
                    // Get accuracy of current projectile as random number from negative shot accuracy to positive shot accuracy
                    float accuracy = GameController.RandomNumGen.Next(-(int)(100f - this.Stats.GunShotAccuracy), (int)(100f - this.Stats.GunShotAccuracy) + 1);
                    Quaternion shotRotation;
                    // If AI aim assist is on
                    if(this.AIAimAssist == true)
                    {
                        // Shot rotation is affected by accuracy and the rotation to its target (instead of rotation of gun barrel, some NPCs need a little aiming boost)
                        shotRotation = Quaternion.Euler(0f, this.IntendedRotation.eulerAngles.y + accuracy, 0f);
                    }
                    // If AI aim assist is off
                    else
                    {
                        // Shot rotation is affected by accuracy and the rotation of the gun barrel
                        shotRotation = Quaternion.Euler(0f, this.GunBarrelObjects[c].transform.rotation.eulerAngles.y + accuracy, 0f);
                    }
                    // Spawn a projectile
                    GameController.SpawnBolt(this.IFF, this.Stats.GunShotProjectileType, this.Stats.GunShotCurvature, this.Stats.GunShotSightCone, 
                        this.Stats.GunShotDamage, this.GunBarrelObjects[c].transform.position, shotRotation, this.ShipRigidbody.velocity, this.Stats.GunShotSpeed, 
                        this.Stats.GunShotLifetime);
                    // Turn on gun lights
                    this.GunBarrelLightsObjects[c].SetActive(true);
                    // Play gun audio
                    this.GunAudioSources[c].Play();
                }
            }
            // Set last shot time
            this.LastGunFireTime = Time.time;
            // Put weapon on cooldown
            this.GunOnCooldown = true;
            // Subtract energy for shot
            this.SubtractEnergy(this.Stats.GunEnergyCost);
        }
        // If weapons fire input is not active, weapon is on cooldown, or not enough energy to fire
        else
        {
            // Loop through gun barrels
            for(int i = 0; i < this.Stats.GunBarrelCount; i++)
            {
                // Turn gun lights off
                this.GunBarrelLightsObjects[i].SetActive(false);
            }
        }
    }

    // Check ablity 1
    protected virtual void CheckAbility1()
    {
        // Each ship has their own special ablities
    }

    // Check ability 2
    protected virtual void CheckAbility2()
    {
        // Each ship has their own special ablities
    }

    // Check ability 3
    protected virtual void CheckAbility3()
    {
        // Each ship has their own special ablities
    }

    // Called when receiving collision from projectile
    public virtual void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // If shields are above 0 or barrier is active
        if(this.Stats.Shields > 0f)
        {
            // Spawn a shield strike particle effect
            this.ProjectileShieldStrike = GameObject.Instantiate(this.ProjectileShieldStrikePrefab, _projectileStrikeLocation, Quaternion.identity);
            // Set particle effect to self destroy after 1 second
            GameObject.Destroy(this.ProjectileShieldStrike, 1f);
        }
        // If shield is 0 or less
        else
        {
            // Spawn a hull strike particle effect
            this.ProjectileHullStrike = GameObject.Instantiate(this.ProjectileHullStrikePrefab, _projectileStrikeLocation, Quaternion.identity);
            // Set particle effect to self destroy after 1 second
            GameObject.Destroy(this.ProjectileHullStrike, 1f);
        }
        // Take damage from projectile
        this.TakeDamage(_damage);
    }

    // Called when receiving collision from ship
    public virtual void ReceivedCollisionFromShip(Vector3 _collisionVelocity, GameController.IFF _iff)
    {
        // Apply velocity inverse to the rotation of the colliding ship
        Quaternion rotationToTarget = Targeting.GetRotationToTarget(this.ShipObject.transform, _collisionVelocity);
        this.ShipRigidbody.AddRelativeForce(rotationToTarget * new Vector3(0, 0, -1000));
        // If ship is different faction
        if(_iff != this.IFF)
        {
            // If armor percentage is above 100, cap it at 100
            Mathf.Clamp(this.Stats.Armor, 0f, 100f);
            // Take impact damage less armor percentage
            this.TakeDamage((this.Stats.MaxHealth + this.Stats.MaxShields) * 0.10f * ((100f - this.Stats.Armor) / 100f));
        }
    }

    // Subtract energy
    protected void SubtractEnergy(float _energyCost)
    {
        this.Stats.Energy -= _energyCost;
        if(this.Stats.Energy < 0f)
        {
            this.Stats.Energy = 0f;
        }
    }

    // Called when ship receives a damaging attack
    public virtual void TakeDamage(float _damage)
    {
        if(this.Stats.Health > 0f)
        {
            // Apply damage to shields
            this.Stats.Shields -= _damage;
            // If shields are knocked below 0
            if(this.Stats.Shields < 0f)
            {
                // Add negative shield amount to health
                this.Stats.Health += this.Stats.Shields;
                // Reset shield to 0
                this.Stats.Shields = 0f;
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Show health damage vignette
                    UIController.ShowHealthDamageEffect();
                }
            }
            // If shields are still above 0
            else
            {
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Show shield damage vignette
                    UIController.ShowShieldDamageEffect();
                }
            }
            // Set last damage taken time
            this.LastDamageTakenTime = Time.time;
            // Put shield on cooldown
            this.ShieldOnCooldown = true;
            // If this is player and shield regen audio is currently playing
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == true)
            {
                // Fade out shield regen audio
                AudioController.FadeOut(this.ShieldRegenAudio, 0.25f, 0f);
            }
            // If ship is currently alive
            if(this.Alive == true)
            {
                // Show damage shader
                this.ShowDamageShaderEffect(true);
            }
        }
    }

    // Turns the damage shader effect on or off
    protected void ShowDamageShaderEffect(bool _show)
    {
        if(_show == true)
        {
            // Turn on damage shader
            this.ShipObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_ShowingEffect", 1f);
        }
        else
        {
            // Turn off the damage shader
            this.ShipObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_ShowingEffect", 0f);
        }
    }

    // Spawns fire particle FX on damaged ships
    protected void SpawnFireParticleFX()
    {
        // Spawn fire particles on ship
        this.ProjectileHullStrike = GameObject.Instantiate(this.ProjectileHullStrikePrefab, this.ShipObject.transform.position, Quaternion.identity);
        // TODO: Modify hull strike projectile to remove sound effect and add explosive sound effect as a component of the Explosion prefab instead
        // Hull strike starts with an explosive sound, turn it off in this case
        this.ProjectileHullStrike.GetComponent<AudioSource>().Stop();
        // Set fire particles to self destroy after 1 second
        GameObject.Destroy(this.ProjectileHullStrike, 1f);
    }

    // Called when ship is destroyed by damage, grants XP
    protected virtual void Kill()
    {
        // Set to not alive
        this.Alive = false;
        // Reset cooldowns
        for(int i = 0; i < 3; i++)
        {
            this.AbilityActive[i] = false;
            this.AbilityOnCooldown[i] = false;
        }
        // Tell UI to remove healthbar for this ship
        UIController.RemoveHealthbar(this.ID);
        // If ship is an enemy
        if(this.IFF == GameController.IFF.Enemy)
        {
            // Increment the score by XP amount
            GameController.AddToScore(this.XP);
        }
        // Create an explosion
        this.Explosion = GameObject.Instantiate(this.ExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(this.Explosion, 1f);
        // Destroy ship object
        GameObject.Destroy(this.ShipObject);
        // Add ship to removal list
        GameController.AddShipToRemovalList(this.ID);
    }

    // Called when ship is too far away from player, doesn't grant XP
    public void Despawn()
    {
        // Set to not alive
        this.Alive = false;
        // Tell UI to remove healthbar for this ship
        UIController.RemoveHealthbar(this.ID);
        // Destroy ship object
        GameObject.Destroy(this.ShipObject);
        // Add ship to removal list
        GameController.AddShipToRemovalList(this.ID);
    }

    // Get references to engine objects
    protected void GetEngineObjects()
    {
        // Set up array defaults
        this.ImpulseEngineObjects = new GameObject[this.Stats.EngineCount];
        this.WarpEngineObjects = new GameObject[this.Stats.EngineCount];
        this.ImpulseParticleSystems = new ParticleSystem[this.Stats.EngineCount];
        this.ImpulseParticleSystemMains = new ParticleSystem.MainModule[this.Stats.EngineCount];
        this.WarpParticleSystems = new ParticleSystem[this.Stats.EngineCount];
        this.WarpParticleSystemMains = new ParticleSystem.MainModule[this.Stats.EngineCount];
        this.ImpulseAudioSources = new AudioSource[this.Stats.EngineCount];
        this.WarpAudioSources = new AudioSource[this.Stats.EngineCount];
        // Loop through engine count
        for(int i = 0; i < this.Stats.EngineCount; i++)
        {
            // Fill arrays with references to gameobjects
            this.ImpulseEngineObjects[i] = this.ShipObject.transform.Find(GameController.ImpulseEngineObjectName + $@" {i}").gameObject;
            this.ImpulseParticleSystems[i] = this.ImpulseEngineObjects[i].GetComponent<ParticleSystem>();
            this.ImpulseParticleSystemMains[i] = this.ImpulseParticleSystems[i].main;
            this.ImpulseAudioSources[i] = this.ImpulseEngineObjects[i].GetComponent<AudioSource>();
            // If this is player
            if(this.IsPlayer == true)
            {
                // Fill warp arrays also
                this.WarpEngineObjects[i] = this.ShipObject.transform.Find(GameController.WarpEngineObjectName + $@" {i}").gameObject;
                this.WarpParticleSystems[i] = this.WarpEngineObjects[i].GetComponent<ParticleSystem>();
                this.WarpParticleSystemMains[i] = this.WarpParticleSystems[i].main;
                this.WarpAudioSources[i] = this.WarpEngineObjects[i].GetComponent<AudioSource>();
            }
        }
    }

    // Get references to gun objects
    protected void GetGunBarrelObjects()
    {
        // Set up array defaults
        this.GunBarrelObjects = new GameObject[this.Stats.GunBarrelCount];
        this.GunBarrelLightsObjects = new GameObject[this.Stats.GunBarrelCount];
        this.GunAudioSources = new AudioSource[this.Stats.GunBarrelCount];
        // Loop through gun barrel count
        for(int i = 0; i < this.Stats.GunBarrelCount; i++)
        {
            // Fill arrays with references to gameobjects
            this.GunBarrelObjects[i] = this.ShipObject.transform.Find(GameController.GunBarrelObjectName + $@" {i}").gameObject;
            this.GunBarrelLightsObjects[i] = this.GunBarrelObjects[i].transform.Find(GameController.GunBarrelLightsObjectName + $@" {i}").gameObject;
            this.GunAudioSources[i] = this.GunBarrelObjects[i].GetComponent<AudioSource>();
        }
    }

    // Get references to particle fx objects
    protected void GetParticleFXObjects()
    {
        // Load prefabs from resources folder for all particle fx
        this.ProjectileShieldStrikePrefab = Resources.Load<GameObject>(GameController.ProjectileShieldStrikePrefabName);
        this.ProjectileHullStrikePrefab = Resources.Load<GameObject>(GameController.ProjectileHullStrikePrefabName);
        this.ExplosionPrefab = Resources.Load<GameObject>(GameController.ExplosionPrefabName);
        this.ElectricityEffectPrefab = Resources.Load<GameObject>(GameController.ElectricityEffectPrefabName);
    }

    // Set up basic ship default values
    protected void SetupShipDefaultValues()
    {
        this.Alive = true;
        this.Stats.Health = this.Stats.MaxHealth;
        this.Stats.Shields = this.Stats.MaxShields;
        this.Stats.Energy = this.Stats.MaxEnergy;
    }

    // Set up AI
    protected void SetupAI()
    {
        // AI type is switch
        switch(this.AItype)
        {
            // Set AI fields based on AI type
            case AIType.Standard:
            {
                this.CanAcquireTargets = true;
                this.CanAccelerate = true;
                this.CanStrafe = true;
                this.CanFireGuns = true;
                break;
            }
            case AIType.Ramming:
            {
                this.CanAcquireTargets = true;
                this.CanAccelerate = true;
                this.CanStrafe = false;
                this.CanFireGuns = false;
                break;
            }
            case AIType.Broadside:
            {
                this.CanAcquireTargets = true;
                this.CanAccelerate = true;
                this.CanStrafe = true;
                this.CanFireGuns = true;
                break;
            }
            case AIType.Flanker:
            {
                this.CanAcquireTargets = true;
                this.CanAccelerate = true;
                this.CanStrafe = true;
                this.CanFireGuns = true;
                this.CanUseAbilities = true;
                break;
            }
            case AIType.Drone:
            {
                this.CanAcquireTargets = true;
                this.CanAccelerate = true;
                this.CanStrafe = true;
                this.CanFireGuns = true;
                break;
            }
        }
    }
}
