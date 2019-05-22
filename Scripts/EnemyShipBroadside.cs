using UnityEngine;

// Controls the enemy ships
public class EnemyShipBroadside : Ship
{
    // Broadside ship-only GameObjects
    public GameObject GunBarrelObject1;
    public GameObject GunBarrelObject2;
    public GameObject GunBarrelObject3;
    public GameObject GunBarrelObject4;
    public GameObject GunBarrelObject5;
    public GameObject GunBarrelLightsObject1;
    public GameObject GunBarrelLightsObject2;
    public GameObject GunBarrelLightsObject3;
    public GameObject GunBarrelLightsObject4;
    public GameObject GunBarrelLightsObject5;

    // Enemy ship constructor
    public EnemyShipBroadside(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Enemy;
        this.AItype = AIType.Standard;
        this.IsPlayer = false;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 100f;
        this.MaxHealth = 100f;
        this.Armor = 80f;
        this.Shields = 25f;
        this.MaxShields = 25f;
        this.ShieldRegenSpeed = 0.5f;
        // Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 40f;
        this.WarpAccelerationMultiplier = 3f;
        this.StrafeAcceleration = 10f;
        this.MaxImpulseSpeed = 40f;
        this.MaxWarpSpeed = 150f;
        this.MaxRotationSpeed = 0.15f;
        this.MaxStrafeSpeed = 10f;
        // Weapon stats
        this.GunShotProjectileType = 4;
        this.GunShotAmount = 2f;
        this.GunShotCurvature = 0f;
        this.GunShotDamage = 4f;
        this.GunShotAccuracy = 75f;
        this.GunShotSpeed = 120f;
        this.GunShotLifetime = 1f;
        // Cooldowns
        this.GunCooldownTime = 1f;
        this.ShieldCooldownTime = 3f;
        this.BarrierCooldownTime = 10f;
        this.BombCooldownTime = 10f;
        this.BarrageCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 17f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // AI fields
        this.MaxTargetAcquisitionRange = 90f;
        this.MaxOrbitRange = 70f;
        this.MaxWeaponsRange = 60f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyBroadsidePrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.Initialize();
    }

    // Initialize
    public override void Initialize()
    {
        // Set up universal ship fields
        this.ShipObject.name = $@"{this.ID}";
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ShipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.ImpulseEngineObject = this.ShipObject.transform.GetChild(0).Find(GameController.ImpulseEngineName).gameObject;
        this.WarpEngineObject = this.ShipObject.transform.GetChild(0).Find(GameController.WarpEngineName).gameObject;
        this.GunBarrelObject = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName + " 0").gameObject;
        this.GunBarrelObject1 = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName + " 1").gameObject;
        this.GunBarrelObject2 = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName + " 2").gameObject;
        this.GunBarrelObject3 = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName + " 3").gameObject;
        this.GunBarrelObject4 = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName + " 4").gameObject;
        this.GunBarrelObject5 = this.ShipObject.transform.GetChild(0).Find(GameController.GunBarrelName + " 5").gameObject;
        this.GunBarrelLightsObject = this.GunBarrelObject.transform.Find(GameController.GunBarrelLightsName + " 0").gameObject;
        this.GunBarrelLightsObject1 = this.GunBarrelObject1.transform.Find(GameController.GunBarrelLightsName + " 1").gameObject;
        this.GunBarrelLightsObject2 = this.GunBarrelObject2.transform.Find(GameController.GunBarrelLightsName + " 2").gameObject;
        this.GunBarrelLightsObject3 = this.GunBarrelObject3.transform.Find(GameController.GunBarrelLightsName + " 3").gameObject;
        this.GunBarrelLightsObject4 = this.GunBarrelObject4.transform.Find(GameController.GunBarrelLightsName + " 4").gameObject;
        this.GunBarrelLightsObject5 = this.GunBarrelObject5.transform.Find(GameController.GunBarrelLightsName + " 5").gameObject;
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


    // Processes inputs
    public override void ProcessInputs()
    {
        // If there is no current target or current target is dead
        if(this.CurrentTarget == null || this.CurrentTarget.Alive == false)
        {
            // Acquire new target
            this.CurrentTarget = AIController.AcquireTarget(this.ShipObject.transform.position, this.IFF, this.MaxTargetAcquisitionRange);
        }
        // If there is a current target and it is alive
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
        {
            // Stop wandering as currently have target
            this.IsWandering = false;
            // Use AI to figure out if ship should accelerate
            if(AIController.ShouldAccelerate(this.AItype, this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
            {
                this.ImpulseInput = true;
            }
            else
            {
                this.ImpulseInput = false;
            }
            // Use AI to figure out if ship should strafe target, resets strafe direction each time strafing is cancelled
            if(AIController.ShouldStrafe(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
            {
                this.StrafeInput = true;
                this.ResetStrafeDirection = false;
            }
            else
            {
                this.StrafeInput = false;
                this.ResetStrafeDirection = true;
            }
            // Use AI to figure out if ship should fire weapons
            if(AIController.ShouldFireGun(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxWeaponsRange) == true)
            {
                this.GunInput = true;
            }
            else
            {
                this.GunInput = false;
            }
        }
        // If unable to acquire target set wandering to true
        else
        {
            this.IsWandering = true;
        }
        // If wandering
        if(this.IsWandering == true)
        {
            // Wander
            this.Wander();
        }
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // If there is a current target and it is alive and not currently strafing
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true && this.StrafeInput == false)
        {
            // Get rotation to face target
            this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
        }
        // If there is a current target and it is alive and we are currenlty strafing
        else if(this.CurrentTarget != null && this.CurrentTarget.Alive == true && this.StrafeInput == true)
        {
            // Get rotation to face target
            this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
            // If strafe right
            if(this.StrafeRight == true)
            {
                // Add 90 degrees to y rotation axis
                this.IntendedRotation = Quaternion.Euler(this.IntendedRotation.eulerAngles.x, this.IntendedRotation.eulerAngles.y + 90, this.IntendedRotation.eulerAngles.z);
            }
            // If strafe left
            else
            {
                // Subtract 90 degrees from y rotation axis
                this.IntendedRotation = Quaternion.Euler(this.IntendedRotation.eulerAngles.x, this.IntendedRotation.eulerAngles.y - 90, this.IntendedRotation.eulerAngles.z);
            }
        }
    }

    // Strafe ship if within weapons range, note: strafe input is only set by NPCs
    public override void StrafeShip()
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
            // Accelerate forward
             this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.StrafeAcceleration));
        }
    }

    // Fires main guns
    public override void CheckMainGun()
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
            // If shot accuracy percentage is above 100, set to 100
            Mathf.Clamp(this.GunShotAccuracy, 0, 100);
            // Loop through shot amount
            for(int i = 0; i < this.GunShotAmount; i++)
            {
                // Get accuracy of current projectile as random number from negative shot accuracy to positive shot accuracy
                float accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                // Shot rotation is affected by accuracy and the rotation to its target (NPCs need a little aiming boost)
                Quaternion shotRotation = Quaternion.Euler(0, this.GunBarrelObject.transform.rotation.eulerAngles.y + accuracy, 0);
                // Spawn projectile
                GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotDamage, this.GunBarrelObject.transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
                accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                shotRotation = Quaternion.Euler(0, this.GunBarrelObject1.transform.rotation.eulerAngles.y + accuracy, 0);
                GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotDamage, this.GunBarrelObject1.transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
                accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                shotRotation = Quaternion.Euler(0, this.GunBarrelObject2.transform.rotation.eulerAngles.y + accuracy, 0);
                GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotDamage, this.GunBarrelObject2.transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
                accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                shotRotation = Quaternion.Euler(0, this.GunBarrelObject3.transform.rotation.eulerAngles.y + accuracy, 0);
                GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotDamage, this.GunBarrelObject3.transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
                accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                shotRotation = Quaternion.Euler(0, this.GunBarrelObject4.transform.rotation.eulerAngles.y + accuracy, 0);
                GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotDamage, this.GunBarrelObject4.transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
                accuracy = GameController.r.Next(-(int)(100 - this.GunShotAccuracy), (int)(100 - this.GunShotAccuracy) + 1);
                shotRotation = Quaternion.Euler(0, this.GunBarrelObject5.transform.rotation.eulerAngles.y + accuracy, 0);
                GameController.SpawnProjectile(this.IFF, this.GunShotProjectileType, this.GunShotCurvature, this.GunShotDamage, this.GunBarrelObject5.transform.position, shotRotation, this.ShipRigidbody.velocity, this.GunShotSpeed, this.GunShotLifetime);
            }
            // Set last shot time
            this.LastGunFireTime = Time.time;
            // Put weapon on cooldown
            this.GunOnCooldown = true;
            // Subtract energy for shot
            this.Energy -= this.GunEnergyCost;
            // Turn on gun lights
            this.GunBarrelLightsObject.SetActive(true);
            this.GunBarrelLightsObject1.SetActive(true);
            this.GunBarrelLightsObject2.SetActive(true);
            this.GunBarrelLightsObject3.SetActive(true);
            this.GunBarrelLightsObject4.SetActive(true);
            this.GunBarrelLightsObject5.SetActive(true);
            // Play gun audio
            this.GunAudio.Play();
        }
        // If weapons fire input is not active, weapon is on cooldown, or not enough energy to fire
        else
        {
            // Turn gun lights off
            this.GunBarrelLightsObject.SetActive(false);
            this.GunBarrelLightsObject1.SetActive(false);
            this.GunBarrelLightsObject2.SetActive(false);
            this.GunBarrelLightsObject3.SetActive(false);
            this.GunBarrelLightsObject4.SetActive(false);
            this.GunBarrelLightsObject5.SetActive(false);
        }
    }
}
