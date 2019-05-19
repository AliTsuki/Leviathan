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
    private GameObject ProjectileShieldStrikePrefab;
    private GameObject ProjectileHullStrikePrefab;
    private GameObject ProjectileShieldStrike;
    private GameObject ProjectileHullStrike;
    private GameObject ExplosionPrefab;
    private GameObject Explosion;

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
    public float RecentRotationsAverage = 0;
    public float TiltAngle = 0;
    public float IntendedAngle = 0;
    public float LastShotFireTime = 0;
    public float LastTakenDamageTime = 0;
    public float LastShieldActivatedTime = 0;
    public float LastShieldCooldownStartedTime = 0;
    public float LastBombActivatedTime = 0;
    public float DamageShaderCooldownTime = 0.5f;
    public bool WeaponOnCooldown = false;
    public bool BombOnCooldown = false;
    public bool BombInFlight = false;
    public bool RegenShieldOnCooldown = false;
    public bool ShieldOnCooldown = false;
    public bool ShieldActive = false;
    public float ImpulseEngineAudioStep = 0.05f;
    public float ImpulseEngineAudioMinVol = 0.1f;
    public float ImpulseEngineAudioMaxVol = 0.5f;
    public float WarpEngineAudioStep = 0.05f;
    public float WarpEngineAudioMinVol = 0;
    public float WarpEngineAudioMaxVol = 1;

    // Ship stats
    // Health/Armor/Shields
    public float Health;
    public float MaxHealth;
    public float Armor;
    public float Shields;
    public float MaxShields;
    public float ShieldRegenSpeed;
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
    public float MaxRotationSpeed;
    public float MaxStrafeSpeed;
    // Weapon stats
    public uint ProjectileType;
    public float ShotAmount;
    public float ShotDamage;
    public float ShotAccuracy;
    public float ShotSpeed;
    public float ShotLifetime;
    public float ShotCurvature;
    public float BombDamage;
    public float BombRadius;
    public float BombSpeed;
    public float BombLiftime;
    public float BombPrimerTime;
    // Cooldowns
    public float ShotCooldownTime;
    public float RegenShieldCooldownTime;
    public float ShieldDuration;
    public float ShieldCooldownTime;
    public float BombCooldownTime;
    public float ScannerCooldownTime;
    // Energy cost
    public float WarpEnergyCost;
    public float ShotEnergyCost;
    public float ShieldDrainCostMultiplier;
    // Experience worth
    public uint XP;

    // AI fields
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


    // Start is called before the first frame update
    public void Start()
    {
        // Set up universal ship fields
        this.ShipObject.name = $@"{this.ID}";
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ImpulseEngineObject = this.ShipObject.transform.GetChild(0).Find("Impulse Engine").gameObject;
        this.WarpEngineObject = this.ShipObject.transform.GetChild(0).Find("Warp Engine").gameObject;
        this.GunBarrelObject = this.ShipObject.transform.GetChild(0).Find("Gun Barrel").gameObject;
        this.GunBarrelLightsObject = this.GunBarrelObject.transform.Find("Gun Barrel Lights").gameObject;
        this.ImpulseParticleSystem = this.ImpulseEngineObject.GetComponent<ParticleSystem>();
        this.ImpulseParticleMain = this.ImpulseParticleSystem.main;
        this.WarpParticleSystem = this.WarpEngineObject.GetComponent<ParticleSystem>();
        this.WarpParticleMain = this.WarpParticleSystem.main;
        this.ImpulseAudio = this.ImpulseEngineObject.GetComponent<AudioSource>();
        this.WarpAudio = this.WarpEngineObject.GetComponent<AudioSource>();
        this.GunAudio = this.GunBarrelObject.GetComponent<AudioSource>();
        this.ShieldRegenAudio = this.ShipObject.GetComponent<AudioSource>();
        this.ProjectileShieldStrikePrefab = Resources.Load(GameController.ProjectileShieldStrikePrefabName, typeof(GameObject)) as GameObject;
        this.ProjectileHullStrikePrefab = Resources.Load(GameController.ProjectileHullStrikePrefabName, typeof(GameObject)) as GameObject;
        this.ExplosionPrefab = Resources.Load(GameController.ExplosionPrefabName, typeof(GameObject)) as GameObject;
        this.Alive = true;
        this.Health = this.MaxHealth;
        this.Shields = this.MaxShields;
        this.Energy = this.MaxEnergy;
        this.RecentRotations = new float[RecentRotationsIndexMax];
    }

    // Update is called once per frame
    public void Update()
    {
        if(this.Alive == true)
        {
            // Process inputs
            this.ProcessInputs();
            // If damage shader has been playing long enough, turn it off
            if(Time.time - this.LastTakenDamageTime >= this.DamageShaderCooldownTime)
            {
                this.ShowDamageShaderEffect(false);
            }
        }
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public void FixedUpdate()
    {
        // If ship is alive, accept inputs from player input or AI
        if(this.Alive == true)
        {
            this.UpdateShipState();
            // If health is less than half, spawn fire particles on ship
            if(this.Health <= this.MaxHealth * 0.5f)
            {
                this.ProjectileHullStrike = GameObject.Instantiate(this.ProjectileHullStrikePrefab, this.ShipObject.transform.position, Quaternion.identity);
                this.ProjectileHullStrike.GetComponent<AudioSource>().Stop();
                GameObject.Destroy(this.ProjectileHullStrike, 1f);
                if(this.IsPlayer == true)
                {
                    UIController.ShowHealthDamageEffect();
                }
            }
            // If ship health has reached 0, run Kill method
            if(this.Health <= 0)
            {
                this.Kill();
            }
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
        this.RotateShip();
        this.ShieldRegen();
        this.EnergyRegen();
        this.AccelerateShip();
        this.StrafeShip();
        this.UseAbilities();
    }

    // Rotates the ship according to player input
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
    public void LeanShip()
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

    // Regenerates shield
    public void ShieldRegen()
    {
        // Take regen shield off cooldown if it has been long enough since last taken damage
        if(Time.time - this.LastTakenDamageTime >= this.RegenShieldCooldownTime)
        {
            this.RegenShieldOnCooldown = false;
        }
        // If shield is not on cooldown then regenerate shield
        if(this.RegenShieldOnCooldown == false && this.Shields < this.MaxShields)
        {
            this.Energy -= Mathf.Min(this.MaxShields - this.Shields, this.ShieldRegenSpeed);
            this.Shields += this.ShieldRegenSpeed;
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == false)
            {
                this.ShieldRegenAudio.volume = 1;
                this.ShieldRegenAudio.Play();
            }
        }
        // Prevent shield from going over max
        if(this.Shields > this.MaxShields)
        {
            this.Shields = this.MaxShields;
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == true)
            {
                AudioController.FadeOut(this.ShieldRegenAudio, 0.25f, 0);
            }
        }
    }

    // Regenerates energy
    public void EnergyRegen()
    {
        // Prevent energy from going below 0
        if(this.Energy < 0)
        {
            this.Energy = 0f;
        }
        // Add energy regen
        this.Energy += this.EnergyRegenSpeed;
        // Prevent energy from going over max
        if(this.Energy > this.MaxEnergy)
        {
            this.Energy = this.MaxEnergy;
        }
    }

    // Accelerates the ship
    public void AccelerateShip()
    {
        // If impulse engine is activated by player input or AI and warp engine is not activated
        if(this.ImpulseInput && !this.WarpInput)
        {
            // Check if at speed limit
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
        else if(this.WarpInput)
        {
            // Check if at speed limit
            if(this.ShipRigidbody.velocity.magnitude < this.MaxWarpSpeed || Vector3.Dot(this.ShipRigidbody.velocity.normalized, this.ShipObject.transform.forward) < 0.5f)
            {
                // Accelerate at warp speed
                this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.ImpulseAcceleration * this.WarpAccelMultiplier));
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

    // Strafe ship if within weapons range, strafe input is only set by NPCs
    public void StrafeShip()
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
    public void FireMainGun()
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
                // Spawn number of projectiles in shot amount
                for(int i = 0; i < this.ShotAmount; i++)
                {
                    // If shot accuracy percentage is above 100, set to 100
                    if(this.ShotAccuracy > 100)
                    {
                        this.ShotAccuracy = 100;
                    }
                    // Get accuracy of current projectile as random number from negative shot accuracy to positive shot accuracy
                    float accuracy = GameController.r.Next(-(int)(100 - this.ShotAccuracy), (int)(100 - this.ShotAccuracy));
                    Quaternion shotRotation;
                    // If player
                    if(this.IsPlayer == true)
                    {
                        // Shot rotation is affected by accuracy and the rotation of the gun barrel
                        shotRotation = Quaternion.Euler(0, this.GunBarrelObject.transform.rotation.eulerAngles.y + accuracy, 0);
                    }
                    // If NPC
                    else
                    {
                        // Shot rotation is affected by accuracy and the rotation to its target (NPCs need a little aiming boost)
                        shotRotation = Quaternion.Euler(0, this.IntendedRotation.eulerAngles.y + accuracy, 0);
                    }
                    GameController.SpawnProjectile(this.IFF, this.ProjectileType, this.ShotDamage, this.GunBarrelObject.transform.position, shotRotation, this.ShipRigidbody.velocity, this.ShotSpeed, this.ShotLifetime);
                }
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
    public void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // Spawn strike projectile
        if(this.Shields > 0 || this.ShieldActive == true)
        {
            this.ProjectileShieldStrike = GameObject.Instantiate(this.ProjectileShieldStrikePrefab, _projectileStrikeLocation, Quaternion.identity);
            GameObject.Destroy(this.ProjectileShieldStrike, 1f);
        }
        else
        {
            this.ProjectileHullStrike = GameObject.Instantiate(this.ProjectileHullStrikePrefab, _projectileStrikeLocation, Quaternion.identity);
            GameObject.Destroy(this.ProjectileHullStrike, 1f);
        }
        // Take damage from projectile
        this.TakeDamage(_damage);
    }

    // Called when receiving collision from ship
    public void ReceivedCollisionFromShip(Vector3 _collisionVelocity, GameController.IFF _iff)
    {
        // Apply velocity received from collision
        this.ShipRigidbody.AddRelativeForce(_collisionVelocity * 0.25f, ForceMode.Impulse);
        if(_iff != this.IFF)
        {
            // If armor percentage is above 100, cap it at 100
            if(this.Armor > 100)
            {
                this.Armor = 100;
            }
            // Take impact damage
            this.TakeDamage(_collisionVelocity.magnitude * ((100 - this.Armor) / 100));
        }
    }

    public void TakeDamage(float _damage)
    {
        if(this.ShieldActive == false)
        {
            // Apply damage to shields
            this.Shields -= _damage;
            // If shields are knocked below 0, apply that damage to health and reset shield to 0
            if(this.Shields < 0)
            {
                this.Health += this.Shields;
                this.Shields = 0f;
                if(this.IsPlayer == true)
                {
                    UIController.ShowHealthDamageEffect();
                }
            }
            else
            {
                if(this.IsPlayer == true)
                {
                    UIController.ShowShieldDamageEffect();
                }
            }
            // Set last damage taken time
            this.LastTakenDamageTime = Time.time;
            // Put shield regen on cooldown
            this.RegenShieldOnCooldown = true;
            // Fade out shield regen audio
            if(this.IsPlayer == true && this.ShieldRegenAudio.isPlaying == true)
            {
                AudioController.FadeOut(this.ShieldRegenAudio, 0.25f, 0f);
            }
            // Show damage shader effect
            if(this.Alive == true)
            {
                this.ShowDamageShaderEffect(true);
            }
        }
        else
        {
            this.Energy -= _damage * this.ShieldDrainCostMultiplier;
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
        // Set to not alive and destroy GameObject
        this.Alive = false;
        UIController.RemoveHealthbar(this.ID);
        // If enemy was killed, increase the score
        if(this.IFF == GameController.IFF.Enemy)
        {
            GameController.Score += this.XP;
        }
        this.Explosion = GameObject.Instantiate(this.ExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        GameObject.Destroy(this.Explosion, 1f);
        if(this.IsPlayer == true)
        {
            GameObject.Destroy(this.ShipObject.transform.GetChild(0).gameObject);
            UIController.GameOver();
        }
        else
        {
            GameObject.Destroy(this.ShipObject);
        }
    }

    // Called when ship is too far away from player, doesn't grant XP
    public void Despawn()
    {
        // Set to not alive and destroy GameObject
        this.Alive = false;
        UIController.RemoveHealthbar(this.ID);
        GameObject.Destroy(this.ShipObject);
    }

    // Called when a ship has no target and nothing else to do
    public void Wander()
    {
        // Stop shooting
        this.FireInput = false;
        // If not moving(wandering) and not currently waiting around
        if(this.IsWanderMove == false && this.IsWaiting == false)
        {
            // Start waiting for random amount of time between 0-10 seconds
            this.IsWaiting = true;
            this.StartedWaitingTime = Time.time;
            this.TimeToWait = GameController.r.Next(0, 10);
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
            this.TimeToWanderMove = GameController.r.Next(0, 10);
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
