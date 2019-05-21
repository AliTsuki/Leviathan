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
    public AudioSource ShieldRegenAudio;
    public GameObject ProjectileShieldStrikePrefab;
    public GameObject ProjectileHullStrikePrefab;
    public GameObject ProjectileShieldStrike;
    public GameObject ProjectileHullStrike;
    public GameObject ExplosionPrefab;
    public GameObject Explosion;

    // Audio fields
    public float ImpulseEngineAudioStep = 0.05f;
    public float ImpulseEngineAudioMinVol = 0.1f;
    public float ImpulseEngineAudioMaxVol = 0.5f;
    public float WarpEngineAudioStep = 0.05f;
    public float WarpEngineAudioMinVol = 0;
    public float WarpEngineAudioMaxVol = 1;

    // Inputs
    public float HorizontalInput;
    public float VerticalInput;
    public bool ImpulseInput;
    public bool WarpInput;
    public bool GunInput;
    public bool BombInput;
    public bool BarrierInput;
    public bool ScannerInput;
    public bool PauseInput;
    public bool StrafeInput;

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
    public float RecentRotationsAverage = 0;
    public float TiltAngle = 0;
    public float IntendedAngle = 0;

    // Cooldown fields
    public float LastGunFireTime = 0;
    public float LastDamageTakenTime = 0;
    public float LastBarrierActivatedTime = 0;
    public float LastBarrierCooldownStartedTime = 0;
    public float LastBombActivatedTime = 0;
    public float DamageShaderCooldownTime = 0.5f;
    public bool GunOnCooldown = false;
    public bool BombOnCooldown = false;
    public bool BombInFlight = false;
    public bool ShieldOnCooldown = false;
    public bool BarrierOnCooldown = false;
    public bool BarrierActive = false;

    // Ship stats
    // --Health/Armor/Shields
    public float Health;
    public float MaxHealth;
    public float Armor;
    public float Shields;
    public float MaxShields;
    public float ShieldRegenSpeed;
    // --Current/Max energy
    public float Energy;
    public float MaxEnergy;
    public float EnergyRegenSpeed;
    // --Speed/Acceleration
    public float ImpulseAcceleration;
    public float WarpAccelerationMultiplier;
    public float StrafeAcceleration;
    public float MaxImpulseSpeed;
    public float MaxWarpSpeed;
    public float MaxStrafeSpeed;
    public float MaxRotationSpeed;
    // --Weapon stats
    // ----Main gun
    public uint ProjectileType;
    public float GunShotAmount;
    public float ShotCurvature;
    public float ShotDamage;
    public float GunShotAccuracy;
    public float ShotSpeed;
    public float ShotLifetime;
    // ----Bombs
    public float BombCurvature;
    public float BombDamage;
    public float BombRadius;
    public float BombSpeed;
    public float BombLiftime;
    public float BombPrimerTime;
    // --Cooldowns
    public float GunCooldownTime;
    public float ShieldCooldownTime;
    public float BarrierDuration;
    public float BarrierCooldownTime;
    public float BombCooldownTime;
    public float ScannerCooldownTime;
    // --Energy cost
    public float WarpEnergyCost;
    public float GunEnergyCost;
    public float BarrierEnergyDrainCost;
    // --Experience worth
    public uint XP;

    // AI fields
    public enum AIType
    {
        Standard,
        Ramming,
        Broadside
    }
    public AIType AItype;
    public Ship CurrentTarget;
    public float MaxTargetAcquisitionRange;
    public float MaxOrbitRange;
    public float MaxWeaponsRange;
    public bool StrafeRight;
    public bool ResetStrafeDirection;
    public bool IsWandering;
    public bool IsWaiting;
    public float StartedWaitingTime;
    public float TimeToWait;
    public bool IsWanderMove;
    public float StartedWanderMoveTime;
    public float TimeToWanderMove;

    // Identification fields
    public uint ID;
    public GameController.IFF IFF;
    public bool Alive;
    public bool IsPlayer;


    // Initialize is called before the first frame update
    public virtual void Initialize()
    {
        // Set up universal ship fields
        this.ShipObject.name = $@"{this.ID}";
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ImpulseEngineObject = this.ShipObject.transform.GetChild(0).Find(GameController.ImpulseEngineName).gameObject;
        this.WarpEngineObject = this.ShipObject.transform.GetChild(0).Find(GameController.WarpEngineName).gameObject;
        this.GunBarrelObject = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName).gameObject;
        this.GunBarrelLightsObject = this.GunBarrelObject.transform.Find(GameController.GunBarrelLightsName).gameObject;
        this.ImpulseParticleSystem = this.ImpulseEngineObject.GetComponent<ParticleSystem>();
        this.ImpulseParticleMain = this.ImpulseParticleSystem.main;
        this.WarpParticleSystem = this.WarpEngineObject.GetComponent<ParticleSystem>();
        this.WarpParticleMain = this.WarpParticleSystem.main;
        this.ImpulseAudio = this.ImpulseEngineObject.GetComponent<AudioSource>();
        this.WarpAudio = this.WarpEngineObject.GetComponent<AudioSource>();
        this.GunAudio = this.GunBarrelObject.GetComponent<AudioSource>();
        this.ShieldRegenAudio = this.ShipObject.GetComponent<AudioSource>();
        this.ProjectileShieldStrikePrefab = Resources.Load<GameObject>(GameController.ProjectileShieldStrikePrefabName);
        this.ProjectileHullStrikePrefab = Resources.Load<GameObject>(GameController.ProjectileHullStrikePrefabName);
        this.ExplosionPrefab = Resources.Load<GameObject>(GameController.ExplosionPrefabName);
        this.Alive = true;
        this.Health = this.MaxHealth;
        this.Shields = this.MaxShields;
        this.Energy = this.MaxEnergy;
        this.RecentRotations = new float[RecentRotationsIndexMax];
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
        // Each subclass has its own AI for this method
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
    }

    // Check health
    public void CheckHealth()
    {
        // If damage shader has been playing long enough
        if(Time.time - this.LastDamageTakenTime >= this.DamageShaderCooldownTime)
        {
            // Turn off damage shader
            this.ShowDamageShaderEffect(false);
        }
        // If health is less than or equal to half
        if(this.Health <= this.MaxHealth * 0.5f)
        {
            // Spawn fire particles on ship
            this.ProjectileHullStrike = GameObject.Instantiate(this.ProjectileHullStrikePrefab, this.ShipObject.transform.position, Quaternion.identity);
            // Hull strike starts with an explosive sound, turn it off in this case
            this.ProjectileHullStrike.GetComponent<AudioSource>().Stop();
            // Set fire particles to self destroy after 1 second
            GameObject.Destroy(this.ProjectileHullStrike, 1f);
            // If this is player ship
            if(this.IsPlayer == true)
            {
                // Show damage vignette
                UIController.ShowHealthDamageEffect();
            }
        }
        // If health has reached 0
        if(this.Health <= 0)
        {
            // Kill ship
            this.Kill();
        }
    }

    // Regenerates energy
    public void EnergyRegen()
    {
        // Prevent energy from going below 0
        Mathf.Clamp(this.Energy, 0, this.MaxEnergy);
        // Add energy regen
        this.Energy += this.EnergyRegenSpeed;
        // Prevent energy from going over max
        Mathf.Clamp(this.Energy, 0, this.MaxEnergy);
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
                this.ShieldRegenAudio.volume = 1;
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
        // Each subclass has its own AI for this method
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
        this.TiltAngle = -(this.RecentRotationsAverage * 5);
        // Get next tilt rotation
        this.TiltRotation = Quaternion.Euler(0, this.CurrentRotation.eulerAngles.y, this.TiltAngle);
        // Rotate ship to new tilt rotation
        this.ShipObject.transform.rotation = this.TiltRotation;
    }

    // Accelerates the ship
    public void AccelerateShip()
    {
        // If impulse engine is activated by player input or AI and warp engine is not activated
        if(this.ImpulseInput == true && this.WarpInput == false)
        {
            // If below impulse speed limit
            if(this.ShipRigidbody.velocity.magnitude < this.MaxImpulseSpeed || Vector3.Dot(this.ShipRigidbody.velocity.normalized, this.ShipObject.transform.forward) < 0.7f)
            {
                // Accelerate forward
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration));
            }
            // Modify particle effects
            this.ImpulseParticleMain.startSpeed = 2.8f;
            this.WarpParticleMain.startLifetime = 0f;
            // Fade in/out audio
            AudioController.FadeIn(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMaxVol);
            AudioController.FadeOut(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
        }
        // If warp engine is activated by player input or AI
        else if(this.WarpInput == true)
        {
            // If below warp speed limit
            if(this.ShipRigidbody.velocity.magnitude < this.MaxWarpSpeed || Vector3.Dot(this.ShipRigidbody.velocity.normalized, this.ShipObject.transform.forward) < 0.5f)
            {
                // Accelerate forward with warp multiplier to speed
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration * this.WarpAccelerationMultiplier));
            }
            // Subtract warp energy cost
            this.Energy -= this.WarpEnergyCost;
            // Modify particle effects
            this.ImpulseParticleMain.startSpeed = 5f;
            this.WarpParticleMain.startSpeed = 20f;
            this.WarpParticleMain.startLifetime = 1f;
            // Fade in/out audio
            AudioController.FadeIn(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMaxVol);
            AudioController.FadeOut(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
        }
        // If no engines are active
        else
        {
            // Turn particles back to default
            this.ImpulseParticleMain.startSpeed = 1f;
            this.WarpParticleMain.startSpeed = 0f;
            this.WarpParticleMain.startLifetime = 0f;
            // Fade out audio
            AudioController.FadeOut(this.ImpulseAudio, this.ImpulseEngineAudioStep, this.ImpulseEngineAudioMinVol);
            AudioController.FadeOut(this.WarpAudio, this.WarpEngineAudioStep, this.WarpEngineAudioMinVol);
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
    public virtual void CheckAbilities()
    {
        this.CheckMainGun();
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
        if(this.GunInput == true && this.GunOnCooldown == false && this.Energy >= this.GunEnergyCost)
        {
            // Loop through shot amount
            for(int i = 0; i < this.GunShotAmount; i++)
            {
                // If shot accuracy percentage is above 100, set to 100
                Mathf.Clamp(this.GunShotAccuracy, 0, 100);
                // Get accuracy of current projectile as random number from negative shot accuracy to positive shot accuracy
                float accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                Quaternion shotRotation;
                // If this is player
                if(this.IsPlayer == true)
                {
                    // Shot rotation is affected by accuracy and the rotation of the gun barrel
                    shotRotation = Quaternion.Euler(0, this.GunBarrelObject.transform.rotation.eulerAngles.y + accuracy, 0);
                }
                // If this is NPC
                else
                {
                    // Shot rotation is affected by accuracy and the rotation to its target (NPCs need a little aiming boost)
                    shotRotation = Quaternion.Euler(0, this.IntendedRotation.eulerAngles.y + accuracy, 0);
                }
                // Spawn a projectile
                GameController.SpawnProjectile(this.IFF, this.ProjectileType, this.ShotCurvature, this.ShotDamage, this.GunBarrelObject.transform.position, shotRotation, this.ShipRigidbody.velocity, this.ShotSpeed, this.ShotLifetime);
            }
            // Set last shot time
            this.LastGunFireTime = Time.time;
            // Put weapon on cooldown
            this.GunOnCooldown = true;
            // Subtract energy for shot
            this.Energy -= this.GunEnergyCost;
            // Turn on gun lights
            this.GunBarrelLightsObject.SetActive(true);
            // Play gun audio
            this.GunAudio.Play();
        }
        // If weapons fire input is not active, weapon is on cooldown, or not enough energy to fire
        else
        {
            // Turn gun lights off
            this.GunBarrelLightsObject.SetActive(false);
        }
    }

    // Called when receiving collision from projectile
    public void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // If shields are above 0 or barrier is active
        if(this.Shields > 0 || this.BarrierActive == true)
        {
            // Spawn a shield strike particle effect
            this.ProjectileShieldStrike = GameObject.Instantiate(this.ProjectileShieldStrikePrefab, _projectileStrikeLocation, Quaternion.identity);
            // Set particle effect to self destroy after 1 second
            GameObject.Destroy(this.ProjectileShieldStrike, 1f);
        }
        // If shield is 0 or barrier is not active
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
        // Apply velocity received from collision
        this.ShipRigidbody.AddRelativeForce(_collisionVelocity * 0.25f, ForceMode.Impulse);
        // If ship is different faction
        if(_iff != this.IFF)
        {
            // If armor percentage is above 100, cap it at 100
            Mathf.Clamp(this.Armor, 0, 100);
            // Take impact damage less armor percentage
            this.TakeDamage(_collisionVelocity.magnitude * ((100 - this.Armor) / 100));
        }
    }

    // Called when ship receives a damaging attack
    public void TakeDamage(float _damage)
    {
        // If barrier is not active
        if(this.BarrierActive == false)
        {
            // Apply damage to shields
            this.Shields -= _damage;
            // If shields are knocked below 0
            if(this.Shields < 0)
            {
                // Add negative shield amount to health
                this.Health += this.Shields;
                // Reset shield to 0
                this.Shields = 0;
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
        // If shield is active
        else
        {
            // Subtract barrier energy drain cost from energy
            this.Energy -= this.BarrierEnergyDrainCost;
        }
    }

    // Turns the damage shader effect on or off
    public void ShowDamageShaderEffect(bool _show)
    {
        if(_show == true)
        {
            // Turn on damage shader
            this.ShipObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_ShowingEffect", 1);
        }
        else
        {
            // Turn off the damage shader
            this.ShipObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_ShowingEffect", 0);
        }
    }

    // Called when ship is destroyed by damage, grants XP
    public void Kill()
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
        // If this is player
        if(this.IsPlayer == true)
        {
            // Destroy ship model
            GameObject.Destroy(this.ShipObject.transform.GetChild(0).gameObject);
            // Show game over screen
            UIController.GameOver();
        }
        // If this is NPC
        else
        {
            // Destroy ship object
            GameObject.Destroy(this.ShipObject);
            // Add ship to removal list
            GameController.ShipsToRemove.Add(this.ID);
        }
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

    // Called when a ship has no target and nothing else to do
    public void Wander()
    {
        // Stop shooting
        this.GunInput = false;
        // If not moving(wandering) and not currently waiting around
        if(this.IsWanderMove == false && this.IsWaiting == false)
        {
            // Start waiting for random amount of time between 0-10 seconds
            this.IsWaiting = true;
            this.StartedWaitingTime = Time.time;
            this.TimeToWait = GameController.r.Next(0, 11);
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
        // If done moving, stop moving
        if(Time.time - this.StartedWanderMoveTime > this.TimeToWanderMove)
        {
            this.IsWanderMove = false;
        }
        // If moving, set impulse to true which causes ship to accelerate forward
        if(this.IsWanderMove == true)
        {
            this.ImpulseInput = true;
        }
    }
}
