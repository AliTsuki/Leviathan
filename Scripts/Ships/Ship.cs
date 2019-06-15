using UnityEngine;

public class Ship
{
    // GameObjects and Components
    public GameObject ShipObject;
    public GameObject ShipObjectPrefab;
    public Rigidbody ShipRigidbody;
    // Engine Objects
    public GameObject[] ImpulseEngineObjects;
    public GameObject[] WarpEngineObjects;
    public ParticleSystem[] ImpulseParticleSystems;
    public ParticleSystem[] WarpParticleSystems;
    public ParticleSystem.MainModule[] ImpulseParticleSystemMains;
    public ParticleSystem.MainModule[] WarpParticleSystemMains;
    public AudioSource[] ImpulseAudioSources;
    public AudioSource[] WarpAudioSources;
    // Gun Objects
    public GameObject[] GunBarrelObjects;
    public GameObject[] GunBarrelLightsObjects;
    public AudioSource[] GunAudioSources;
    // Shield Object
    public AudioSource ShieldRegenAudio;
    // Particle Objects
    public GameObject ProjectileShieldStrikePrefab;
    public GameObject ProjectileHullStrikePrefab;
    public GameObject ProjectileShieldStrike;
    public GameObject ProjectileHullStrike;
    public GameObject ExplosionPrefab;
    public GameObject Explosion;
    public GameObject ElectricityEffectPrefab;
    public GameObject ElectricityEffect;

    // Constants
    public float ImpulseEngineAudioStep = 0.05f;
    public float ImpulseEngineAudioMinVol = 0.25f;
    public float ImpulseEngineAudioMaxVol = 1f;
    public float WarpEngineAudioStep = 0.05f;
    public float WarpEngineAudioMinVol = 0f;
    public float WarpEngineAudioMaxVol = 1f;
    // Shader constants
    public const float DamageShaderCooldownTime = 0.5f;

    // Inputs
    public Vector2 AimInput;
    public float ImpulseEngineInput;
    public float WarpEngineInput;
    public bool StrafeInput;
    public bool MainGunInput;
    public bool[] AbilityInput = new bool[3];
    public bool PauseInput;

    // Rotation fields
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

    // Cooldown times
    public float LastGunFireTime = 0f;
    public float LastDamageTakenTime = 0f;
    public float[] LastAbilityActivatedTime = new float[3];
    public float[] LastAbilityCooldownStartedTime = new float[3];

    // On cooldown bools
    public bool GunOnCooldown = false;
    public bool ShieldOnCooldown = false;
    public bool[] AbilityOnCooldown = new bool[3];
    public bool[] AbilityActive = new bool[3];

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

    // Defaults for ability changes
    public float DefaultGunCooldownTime;
    public uint DefaultGunShotAmount;
    public float DefaultGunShotDamage;
    public float DefaultGunShotAccuracy;
    public float DefaultGunEnergyCost;
    public float DefaultEnergyRegenSpeed;
    public float DefaultShieldRegenSpeed;
    public float DefaultShieldCooldownTime;
    public float DefaultMaxRotationSpeed;
    public float DefaultMaxImpulseSpeed;
    public float DefaultMaxStrafeSpeed;

    // Status effects
    public bool IsEMPed;
    public bool IsEMPApplied;
    public float EMPEffectDuration;
    public float EMPStartedTime;

    // --Experience worth
    public uint XP;

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
    public AIType AItype;
    public Ship CurrentTarget;
    public bool AIAimAssist;
    public float MaxTargetAcquisitionRange;
    public float MaxOrbitRange;
    public float MaxWeaponsRange;
    public float MaxAbilityRange;
    public bool StrafeRight;
    public bool ResetStrafeDirection;
    public bool ShouldWander;
    public bool IsWaiting;
    public float StartedWaitingTime;
    public float TimeToWait;
    public bool IsWanderMove;
    public float StartedWanderMoveTime;
    public float TimeToWanderMove;
    public Ship Parent;
    public float MaxLeashDistance;
    public float OrbitParentRange;
    public bool ShouldFollowParent;
    public bool ShouldAcquireTargets;
    public bool ShouldAccelerate;
    public bool ShouldStrafe;
    public bool ShouldFireGuns;
    public bool ShouldUseAbilities;

    // Identification fields
    public uint ID;
    public GameController.IFF IFF;
    public bool Alive;
    public bool IsPlayer;


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
    public virtual void ProcessInputs()
    {
        AIController.ProcessAI(this);
    }

    // Check health
    public void CheckHealth()
    {
        // If damage shader has been playing long enough
        if(Time.time - this.LastDamageTakenTime >= DamageShaderCooldownTime)
        {
            // Turn off damage shader
            this.ShowDamageShaderEffect(false);
        }
        // If health is less than or equal to half
        if(this.Health <= this.MaxHealth * 0.5f)
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
        if(this.Health <= 0f)
        {
            // Kill ship
            this.Kill();
        }
    }

    // Check effect status
    public void CheckStatusEffects()
    {
        // If ship is hit by EMP and EMP effect has not been applied yet
        if(this.IsEMPed == true && this.IsEMPApplied == false)
        {
            this.EMPStartedTime = Time.time;
            this.DefaultEnergyRegenSpeed = this.EnergyRegenSpeed;
            this.DefaultShieldRegenSpeed = this.ShieldRegenSpeed;
            this.DefaultMaxRotationSpeed = this.MaxRotationSpeed;
            this.DefaultMaxImpulseSpeed = this.MaxImpulseSpeed;
            this.DefaultMaxStrafeSpeed = this.MaxStrafeSpeed;
            this.DefaultGunCooldownTime = this.GunCooldownTime;
            this.EnergyRegenSpeed = 0f;
            this.ShieldRegenSpeed = 0f;
            this.MaxRotationSpeed = 0f;
            this.MaxImpulseSpeed = 0f;
            this.MaxStrafeSpeed = 0f;
            this.GunCooldownTime = 10f;
            this.Energy = 0f;
            this.Shields = 0f;
            this.ElectricityEffect = GameObject.Instantiate(this.ElectricityEffectPrefab, this.ShipObject.transform.position, Quaternion.identity, this.ShipObject.transform);
            GameObject.Destroy(this.ElectricityEffect, this.EMPEffectDuration);
            this.IsEMPApplied = true;
        }
        // If ship is hit by EMP and EMP has already been applied and it has been longer than EMP effect duration since EMP effect was applied
        else if(this.IsEMPed == true && this.IsEMPApplied == true && Time.time - this.EMPStartedTime >= this.EMPEffectDuration)
        {
            this.IsEMPed = false;
            this.IsEMPApplied = false;
            this.EnergyRegenSpeed = this.DefaultEnergyRegenSpeed;
            this.ShieldRegenSpeed = this.DefaultShieldRegenSpeed;
            this.MaxRotationSpeed = this.DefaultMaxRotationSpeed;
            this.MaxImpulseSpeed = this.DefaultMaxImpulseSpeed;
            this.MaxStrafeSpeed = this.DefaultMaxStrafeSpeed;
            this.GunCooldownTime = this.DefaultGunCooldownTime;
        }
    }

    // Updates the state of the ship, turning, accelerating, using weapons etc.
    public void UpdateShipState()
    {
        this.EnergyRegen();
        this.ShieldRegen();
        this.RotateShip();
        this.AccelerateShip();
        this.StrafeShip();
        this.CheckAbilities();
        // TODO: Maybe move wander and follow parent to AIController
        this.Wander();
        this.FollowParent();
    }

    // Regenerates energy
    public void EnergyRegen()
    {
        // Prevent energy from going below 0
        if(this.Energy < 0f)
        {
            this.Energy = 0f;
        }
        // If energy is below maximum
        if(this.Energy < this.MaxEnergy)
        {
            // Add energy regen
            this.Energy += this.EnergyRegenSpeed;
        }
        // Prevent energy from going over maximum
        if(this.Energy > this.MaxEnergy)
        {
            this.Energy = this.MaxEnergy;
        }
    }

    // Regenerates shield
    public void ShieldRegen()
    {
        // If time since last taken damage was more than shield cooldown time
        if(Time.time - this.LastDamageTakenTime >= this.ShieldCooldownTime)
        {
            // Take shield off cooldown
            this.ShieldOnCooldown = false;
        }
        // If shield is not on cooldown and shields are less than maximum
        if(this.ShieldOnCooldown == false && this.Shields < this.MaxShields)
        {
            // Remove energy for regenerating shield
            this.Energy -= Mathf.Min(this.MaxShields - this.Shields, this.ShieldRegenSpeed);
            // Add shield regen speed to shields
            this.Shields += this.ShieldRegenSpeed;
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
        if(this.Shields > this.MaxShields)
        {
            // Set shields to max value
            this.Shields = this.MaxShields;
            // If this is player and shield regen audio is playing
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == true)
            {
                // Fade out shield regen audio
                AudioController.FadeOut(this.ShieldRegenAudio, 0.25f, 0);
            }
        }
    }

    // Rotates the ship
    public void RotateShip()
    {
        this.GetIntendedRotation();
        this.TurnShip();
        this.LeanShip();
    }

    // Gets intended rotation
    public virtual void GetIntendedRotation()
    {
        AIController.GetIntendedRotation(this);
    }

    // Turns the ship
    public void TurnShip()
    {
        // Get current rotation
        this.CurrentRotation = this.ShipObject.transform.rotation;
        // Get next rotation by using intended rotation and max rotation speed
        this.NextRotation = Quaternion.Lerp(this.CurrentRotation, this.IntendedRotation, this.MaxRotationSpeed);
        // Rotate to next rotation
        this.ShipObject.transform.rotation = this.NextRotation;
    }

    // Lean the ship during turns
    public void LeanShip()
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
    public void AccelerateShip()
    {
        // If impulse engine is activated by player input or AI and warp engine is not activated
        if(this.ImpulseEngineInput > 0f && this.WarpEngineInput <= 0f)
        {
            // Accelerate forward
            this.ShipRigidbody.AddRelativeForce(new Vector3(0f, 0f, this.ImpulseAcceleration * this.ImpulseEngineInput));
            // If current magnitude of velocity is beyond speed limit for impulse power
            if(this.ShipRigidbody.velocity.magnitude > this.MaxImpulseSpeed)
            {
                // Linearly interpolate velocity toward speed limit
                this.ShipRigidbody.velocity = Vector3.Lerp(this.ShipRigidbody.velocity, Vector3.ClampMagnitude(this.ShipRigidbody.velocity, this.MaxImpulseSpeed), 0.05f);
            }
            // Loop through engines
            for(int i = 0; i < this.EngineCount; i++)
            {
                // Modify particle fx
                this.ImpulseParticleSystemMains[i].startSpeed = 2.8f * this.ImpulseEngineInput;
                // Audio fadein
                AudioController.FadeIn(this.ImpulseAudioSources[i], this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMaxVol);
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Modify warp particle fx
                    this.WarpParticleSystemMains[i].startLifetime = 0f;
                    // Warp audio fadeout
                    AudioController.FadeOut(this.WarpAudioSources[i], this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
                }
            }
        }
        // If warp engine is activated by player input or AI
        else if(this.WarpEngineInput > 0f)
        {
            // Accelerate forward with warp multiplier to speed
            this.ShipRigidbody.AddRelativeForce(new Vector3(0f, 0f, this.ImpulseAcceleration * this.WarpAccelerationMultiplier));
            // Clamp magnitude at maximum warp speed
            this.ShipRigidbody.velocity = Vector3.ClampMagnitude(this.ShipRigidbody.velocity, this.MaxWarpSpeed);
            // Subtract warp energy cost
            this.Energy -= this.WarpEnergyCost;
            // Loop through engines
            for(int i = 0; i < this.EngineCount; i++)
            {
                // Modify particle fx
                this.ImpulseParticleSystemMains[i].startSpeed = 5f;
                // Audio fadeout
                AudioController.FadeOut(this.ImpulseAudioSources[i], this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Modify warp particle fx
                    this.WarpParticleSystemMains[i].startSpeed = 20f;
                    this.WarpParticleSystemMains[i].startLifetime = 1f;
                    // Warp audio fadein
                    AudioController.FadeIn(this.WarpAudioSources[i], this.WarpEngineAudioStep, this.WarpEngineAudioMaxVol);
                }
            }
        }
        // If no engines are active
        else
        {
            // Loop through engines
            for(int i = 0; i < this.EngineCount; i++)
            {
                // Turn particles back to default
                this.ImpulseParticleSystemMains[i].startSpeed = 1f;
                // Audio fadeout to default
                AudioController.FadeOut(this.ImpulseAudioSources[i], this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Warp particle fx to default
                    this.WarpParticleSystemMains[i].startSpeed = 0f;
                    this.WarpParticleSystemMains[i].startLifetime = 0f;
                    // Warp audio fadeout
                    AudioController.FadeOut(this.WarpAudioSources[i], this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
                }
            }
        }
    }

    // Strafe ship if within weapons range, note: strafe input is only set by NPCs
    public virtual void StrafeShip()
    {
        // If strafe direction should be reset
        if(this.ResetStrafeDirection == true)
        {
            // Get random number 0 or 1, if 1 strafe right, if 0 strafe left
            if(GameController.r.Next(0, 2) == 1)
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
                this.ShipRigidbody.AddRelativeForce(new Vector3(this.StrafeAcceleration, 0f, 0f));
            }
            // If strafe left add negative strafe acceleration
            else
            {
                // Strafe
                this.ShipRigidbody.AddRelativeForce(new Vector3(-this.StrafeAcceleration, 0f, 0f));
            }
        }
    }

    // Uses abilities: fire weapons, use abilities 1, 2, and 3
    public void CheckAbilities()
    {
        this.CheckMainGun();
        this.CheckAbility1();
        this.CheckAbility2();
        this.CheckAbility3();
    }

    // Fires main guns
    public virtual void CheckMainGun()
    {
        // If time since last fired weapon is greater than or equal to cooldown time
        if(Time.time - this.LastGunFireTime >= this.GunCooldownTime)
        {
            // Take gun off cooldown
            this.GunOnCooldown = false;
        }
        // If weapons fire gun input is activated by player input or AI, the weapon is not on cooldown, and there is more available energy than the cost to fire
        if(this.MainGunInput == true && this.GunOnCooldown == false && this.Energy >= this.GunEnergyCost)
        {
            // Loop through gun barrel count
            for(int c = 0; c < this.GunBarrelCount; c++)
            {
                // Loop through gun shot amount
                for(int a = 0; a < this.GunShotAmount; a++)
                {
                    // If shot accuracy percentage is above 100, set to 100
                    Mathf.Clamp(this.GunShotAccuracy, 0f, 100f);
                    // Get accuracy of current projectile as random number from negative shot accuracy to positive shot accuracy
                    float accuracy = GameController.r.Next(-(int)(100f - this.GunShotAccuracy), (int)(100f - this.GunShotAccuracy) + 1);
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
                    GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotSightCone, this.GunShotDamage, this.GunBarrelObjects[c].transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
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
            this.Energy -= this.GunEnergyCost;
        }
        // If weapons fire input is not active, weapon is on cooldown, or not enough energy to fire
        else
        {
            // Loop through gun barrels
            for(int i = 0; i < this.GunBarrelCount; i++)
            {
                // Turn gun lights off
                this.GunBarrelLightsObjects[i].SetActive(false);
            }
        }
    }

    // Check ablity 1
    public virtual void CheckAbility1()
    {
        // Each ship has their own special ablities
    }

    // Check ability 2
    public virtual void CheckAbility2()
    {
        // Each ship has their own special ablities
    }

    // Check ability 3
    public virtual void CheckAbility3()
    {
        // Each ship has their own special ablities
    }

    // Called when receiving collision from projectile
    public virtual void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // If shields are above 0 or barrier is active
        if(this.Shields > 0f)
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
        Quaternion rotationToTarget = AIController.GetRotationToTarget(this.ShipObject.transform, _collisionVelocity);
        this.ShipRigidbody.AddRelativeForce(rotationToTarget * new Vector3(0, 0, -1000));
        // If ship is different faction
        if(_iff != this.IFF)
        {
            // If armor percentage is above 100, cap it at 100
            Mathf.Clamp(this.Armor, 0f, 100f);
            // Take impact damage less armor percentage
            this.TakeDamage((this.MaxHealth + this.MaxShields) * 0.10f * ((100f - this.Armor) / 100f));
        }
    }

    // Called when ship receives a damaging attack
    public virtual void TakeDamage(float _damage)
    {
        // Apply damage to shields
        this.Shields -= _damage;
        // If shields are knocked below 0
        if(this.Shields < 0f)
        {
            // Add negative shield amount to health
            this.Health += this.Shields;
            // Reset shield to 0
            this.Shields = 0f;
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

    // Turns the damage shader effect on or off
    public void ShowDamageShaderEffect(bool _show)
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
    public void SpawnFireParticleFX()
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
    public virtual void Kill()
    {
        // Set to not alive
        this.Alive = false;
        // Tell UI to remove healthbar for this ship
        UIController.RemoveHealthbar(this.ID);
        // If ship is an enemy
        if(this.IFF == GameController.IFF.Enemy)
        {
            // Increment the score by XP amount
            GameController.Score += this.XP;
        }
        // Create an explosion
        this.Explosion = GameObject.Instantiate(this.ExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(this.Explosion, 1f);
        // Destroy ship object
        GameObject.Destroy(this.ShipObject);
        // Add ship to removal list
        GameController.ShipsToRemove.Add(this.ID);
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
        GameController.ShipsToRemove.Add(this.ID);
    }

    // Get references to engine objects
    public void GetEngineObjects()
    {
        // Set up array defaults
        this.ImpulseEngineObjects = new GameObject[this.EngineCount];
        this.WarpEngineObjects = new GameObject[this.EngineCount];
        this.ImpulseParticleSystems = new ParticleSystem[this.EngineCount];
        this.ImpulseParticleSystemMains = new ParticleSystem.MainModule[this.EngineCount];
        this.WarpParticleSystems = new ParticleSystem[this.EngineCount];
        this.WarpParticleSystemMains = new ParticleSystem.MainModule[this.EngineCount];
        this.ImpulseAudioSources = new AudioSource[this.EngineCount];
        this.WarpAudioSources = new AudioSource[this.EngineCount];
        // Loop through engine count
        for(int i = 0; i < this.EngineCount; i++)
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
    public void GetGunBarrelObjects()
    {
        // Set up array defaults
        this.GunBarrelObjects = new GameObject[this.GunBarrelCount];
        this.GunBarrelLightsObjects = new GameObject[this.GunBarrelCount];
        this.GunAudioSources = new AudioSource[this.GunBarrelCount];
        // Loop through gun barrel count
        for(int i = 0; i < this.GunBarrelCount; i++)
        {
            // Fill arrays with references to gameobjects
            this.GunBarrelObjects[i] = this.ShipObject.transform.Find(GameController.GunBarrelObjectName + $@" {i}").gameObject;
            this.GunBarrelLightsObjects[i] = this.GunBarrelObjects[i].transform.Find(GameController.GunBarrelLightsObjectName + $@" {i}").gameObject;
            this.GunAudioSources[i] = this.GunBarrelObjects[i].GetComponent<AudioSource>();
        }
    }

    // Get references to particle fx objects
    public void GetParticleFXObjects()
    {
        // Load prefabs from resources folder for all particle fx
        this.ProjectileShieldStrikePrefab = Resources.Load<GameObject>(GameController.ProjectileShieldStrikePrefabName);
        this.ProjectileHullStrikePrefab = Resources.Load<GameObject>(GameController.ProjectileHullStrikePrefabName);
        this.ExplosionPrefab = Resources.Load<GameObject>(GameController.ExplosionPrefabName);
        this.ElectricityEffectPrefab = Resources.Load<GameObject>(GameController.ElectricityEffectPrefabName);
    }

    // Set up basic ship default values
    public void SetupShipDefaultValues()
    {
        this.Alive = true;
        this.Health = this.MaxHealth;
        this.Shields = this.MaxShields;
        this.Energy = this.MaxEnergy;
        this.RecentRotations = new float[RecentRotationsIndexMax];
        // Default audio levels
        this.ImpulseEngineAudioStep = 0.05f;
        this.ImpulseEngineAudioMinVol = 0.1f;
        this.ImpulseEngineAudioMaxVol = 0.5f;
        this.WarpEngineAudioStep = 0.05f;
        this.WarpEngineAudioMinVol = 0f;
        this.WarpEngineAudioMaxVol = 1f;
    }

    // Set up AI
    public void SetupAI()
    {
        // AI type is switch
        switch(this.AItype)
        {
            // Set AI fields based on AI type
            case AIType.Standard:
            {
                this.ShouldAcquireTargets = true;
                this.ShouldAccelerate = true;
                this.ShouldStrafe = true;
                this.ShouldFireGuns = true;
                break;
            }
            case AIType.Ramming:
            {
                this.ShouldAcquireTargets = true;
                this.ShouldAccelerate = true;
                this.ShouldStrafe = false;
                this.ShouldFireGuns = false;
                break;
            }
            case AIType.Broadside:
            {
                this.ShouldAcquireTargets = true;
                this.ShouldAccelerate = true;
                this.ShouldStrafe = true;
                this.ShouldFireGuns = true;
                break;
            }
            case AIType.Flanker:
            {
                this.ShouldAcquireTargets = true;
                this.ShouldAccelerate = true;
                this.ShouldStrafe = true;
                this.ShouldFireGuns = true;
                this.ShouldUseAbilities = true;
                break;
            }
            case AIType.Drone:
            {
                this.ShouldAcquireTargets = true;
                this.ShouldAccelerate = true;
                this.ShouldStrafe = true;
                this.ShouldFireGuns = true;
                break;
            }
        }
    }

    // Called when a ship has no target and nothing else to do
    public void Wander()
    {
        // If should wander is true
        if(this.ShouldWander == true)
        {
            // Stop shooting
            this.MainGunInput = false;
            // If not moving(wandering) and not currently waiting around
            if(this.IsWanderMove == false && this.IsWaiting == false)
            {
                // Start waiting for random amount of time between 0-10 seconds
                this.IsWaiting = true;
                this.StartedWaitingTime = Time.time;
                this.TimeToWait = GameController.r.Next(0, 11);
                // Turn off engines
                this.ImpulseEngineInput = 0f;
            }
            // If done waiting, stop waiting
            if(Time.time - this.StartedWaitingTime > this.TimeToWait)
            {
                this.IsWaiting = false;
            }
            // If done waiting and not yet moving
            if(this.IsWaiting == false && this.IsWanderMove == false)
            {
                // Start moving for random amount of time between 0-10 seconds and rotate some random direction
                this.IsWanderMove = true;
                this.StartedWanderMoveTime = Time.time;
                this.TimeToWanderMove = GameController.r.Next(0, 11);
                this.IntendedRotation = Quaternion.Euler(0, GameController.r.Next(0, 360), 0);
            }
            // If moving, set impulse to true which causes ship to accelerate forward
            if(this.IsWanderMove == true)
            {
                this.ImpulseEngineInput = 1f;
            }
            // If done moving, stop moving
            if(Time.time - this.StartedWanderMoveTime > this.TimeToWanderMove)
            {
                this.IsWanderMove = false;
            }
        }
    }

    // Called when drones need to follow their parent ship
    public void FollowParent()
    {
        // If should follow parent is true
        if(this.ShouldFollowParent == true)
        {
            // Stop shooting
            this.MainGunInput = false;
            // If distance to parent ship is greater than max leash distance multiplied by 1.5
            if(Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) > this.MaxLeashDistance * 1.5f)
            {
                // Teleport directly to parent
                this.ShipObject.transform.position = this.Parent.ShipObject.transform.position;
                // Rotate same rotation as parent
                this.IntendedRotation = this.Parent.ShipObject.transform.rotation;
            }
            // If distance to parent ship is greater than orbit parent range
            else if(Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) > this.OrbitParentRange)
            {
                // Reset targetting
                this.CurrentTarget = null;
                // Rotate to face parent
                this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.Parent.ShipObject.transform.position);
                // Accelerate forward
                this.ImpulseEngineInput = 1f;
            }
            // If distance to parent is less than or equal to orbit parent range
            else if(Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) <= this.OrbitParentRange)
            {
                // Turn off engine
                this.ImpulseEngineInput = 0f;
                // Set velocity to parent velocity
                this.ShipRigidbody.velocity = this.Parent.ShipRigidbody.velocity;
                // Set rotation to parent rotation
                this.IntendedRotation = this.Parent.ShipObject.transform.rotation;
                // Stop following parent
                this.ShouldFollowParent = false;
            }
        }
    }
}
