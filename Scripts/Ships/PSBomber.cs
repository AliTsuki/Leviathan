using UnityEngine;

public class PSBomber : PlayerShip
{
    // Player-only GameObjects
    private GameObject BarrierObject;
    private Bomb bomb;

    // On cooldown bools
    public bool BombInFlight = false;

    // Ship stats
    // ----Ability 1: Barrier
    public float BarrierEnergyDrainCost; // Energy cost deducted when ship takes damage while barrier is active
    // ----Ability 2: Barrage
    public float BarrageGunCooldownTimeMultiplier; // Main gun cooldown is multiplied by this value during barrage, 0.5f means gun shoots twice as fast
    public float BarrageShotAmountIncrease; // Number of shots to add per gun barrel fired while barrage is active
    public float BarrageDamageMultiplier; // Shot damage is multiplied by this value
    public float BarrageAccuracyMultiplier; // Shot accuracy is multiplied by this value
    public float BarrageEnergyCostMultiplier; // Shot energy cost is multiplied by this value
    // ----Ability 3: Bombs
    public float BombDamage; // Maximum damage bomb explosion will do to targets in its direct center, damage is calculated as linear falloff percentage of target distance to center of bomb explosion
    public float BombRadius; // Radius in which bomb will deal damage to ships
    public float BombSpeed; // Maximum velocity of fired bombs
    public float BombLiftime; // Number of seconds bomb will fly before it self destructs
    public float BombPrimerTime; // Number of seconds the bomb must fly before it can be detonated

    // Player ship constructor
    public PSBomber(uint _id)
    {
        this.ID = _id;
        // TODO: Change starting position to be last home zone saved to
        this.StartingPosition = new Vector3(0, 0, 0);
        this.Type = PlayerShipType.Bomber;
        this.AItype = AIType.None;
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = true;
        // Ship stats
        // --Health/Armor/Shields
        this.Health = 200f;
        this.MaxHealth = 200f;
        this.Armor = 75f;
        this.Shields = 100f;
        this.MaxShields = 100f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 17f;
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 100f;
        this.WarpAccelerationMultiplier = 3f;
        this.StrafeAcceleration = 0f;
        // --Max Speed
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 150f;
        this.MaxStrafeSpeed = 0f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 1;
        this.GunShotProjectileType = 5;
        this.GunCooldownTime = 0.25f;
        this.GunShotAmount = 1;
        this.GunShotCurvature = 0.05f;
        this.GunShotSightCone = 0.8f;
        this.GunShotDamage = 30f;
        this.GunShotAccuracy = 99f;
        this.GunShotSpeed = 100f;
        this.GunShotLifetime = 2f;
        // -- Abilities
        // ----Ability 1: Barrier
        this.BarrierEnergyDrainCost = 10f;
        // ----Ability 2: Barrage
        this.BarrageGunCooldownTimeMultiplier = 0.5f;
        this.BarrageShotAmountIncrease = 1;
        this.BarrageDamageMultiplier = 0.7f;
        this.BarrageAccuracyMultiplier = 0.7f;
        this.BarrageEnergyCostMultiplier = 0.85f;
        // ----Ability 3: Bombs
        this.BombDamage = 150f;
        this.BombRadius = 50f;
        this.BombSpeed = 35f;
        this.BombLiftime = 3f;
        this.BombPrimerTime = 0.25f;
        // ---- Ability Cooldowns
        this.Ability1Duration = 5f;
        this.Ability1CooldownTime = 10f;
        this.Ability2Duration = 5f;
        this.Ability2CooldownTime = 10f;
        this.Ability3Duration = 0f;
        this.Ability3CooldownTime = 15f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        // Ship type specific objects
        this.BarrierObject = this.ShipObject.transform.GetChild(0).Find(GameController.BarrierObjectName).gameObject;
        this.BarrierObject.SetActive(false);
        // Audio levels
        this.ImpulseEngineAudioStep = 0.05f;
        this.ImpulseEngineAudioMinVol = 0.25f;
        this.ImpulseEngineAudioMaxVol = 1f;
        // Set up Default Cooldowns
        this.DefaultGunCooldownTime = this.GunCooldownTime;
        this.DefaultGunShotAmount = this.GunShotAmount;
        this.DefaultGunShotDamage = this.GunShotDamage;
        this.DefaultGunShotAccuracy = this.GunShotAccuracy;
        this.DefaultGunEnergyCost = this.GunEnergyCost;
        this.Initialize();
    }


    // Check ability 1
    public override void CheckAbility1()
    {
        this.CheckBarrier();
    }

    // Check ability 2
    public override void CheckAbility2()
    {
        this.CheckBarrage();
    }

    // Check ability 3
    public override void CheckAbility3()
    {
        this.CheckBomb();
    }

    // Check barrier
    private void CheckBarrier()
    {
        // If barrier input activated and barrier is not currently on cooldown and barrier is not currently active
        if(this.Ability1Input == true && this.Ability1OnCooldown == false && this.Ability1Active == false)
        {
            // Activate barrier object, set barrier active to true, and record time barrier was activated
            this.BarrierObject.SetActive(true);
            this.Ability1Active = true;
            this.LastAbility1ActivatedTime = Time.time;
        }
        // If difference between current time and shield last activated time is greater than barrier duration
        if(this.Ability1Active == true && Time.time - this.LastAbility1ActivatedTime > this.Ability1Duration)
        {
            // Disable barrier object, set barrier to not active, set barrier to on cooldown, and record time cooldown started
            this.BarrierObject.SetActive(false);
            this.Ability1Active = false;
            this.Ability1OnCooldown = true;
            this.LastAbility1CooldownStartedTime = Time.time;
        }
        // If difference between current time and barrier started cooldown time is greater than barrier cooldown time
        if(this.Ability1OnCooldown == true && Time.time - this.LastAbility1CooldownStartedTime > this.Ability1CooldownTime)
        {
            // Take barrier off cooldown
            this.Ability1OnCooldown = false;
        }
    }

    // Check barrage
    private void CheckBarrage()
    {
        // If barrage input is active, barrage is not on cooldown, and barrage is not currently active
        if(this.Ability2Input == true && this.Ability2OnCooldown == false && this.Ability2Active == false)
        {
            // Set barrage to active
            this.Ability2Active = true;
            // Record last barrage activated time
            this.LastAbility2ActivatedTime = Time.time;
            // Apply barrage multipliers
            this.GunCooldownTime *= this.BarrageGunCooldownTimeMultiplier;
            this.GunShotAmount += this.BarrageShotAmountIncrease;
            this.GunShotDamage *= this.BarrageDamageMultiplier;
            this.GunShotAccuracy *= this.BarrageAccuracyMultiplier;
            this.GunEnergyCost *= this.BarrageEnergyCostMultiplier;
        }
        // If barrage is currently active and last barrage activated time is greater than barrage duration
        if(this.Ability2Active == true && Time.time - this.LastAbility2ActivatedTime > this.Ability2Duration)
        {
            // Set barrage to off
            this.Ability2Active = false;
            // Set barrage on cooldown
            this.Ability2OnCooldown = true;
            // Record barrage cooldown started time
            this.LastAbility2CooldownStartedTime = Time.time;
            // Remove barrage multipliers
            this.GunCooldownTime = this.DefaultGunCooldownTime;
            this.GunShotAmount = this.DefaultGunShotAmount;
            this.GunShotDamage = this.DefaultGunShotDamage;
            this.GunShotAccuracy = this.DefaultGunShotAccuracy;
            this.GunEnergyCost = this.DefaultGunEnergyCost;
        }
        // If barrage is on cooldown and last barrage cooldown started time is greater than barrage cooldown time
        if(this.Ability2OnCooldown == true && Time.time - this.LastAbility2CooldownStartedTime > this.Ability2CooldownTime)
        {
            // Take barrage off cooldown
            this.Ability2OnCooldown = false;
        }
    }

    // Check bomb
    private void CheckBomb()
    {
        // If bomb input is active, bomb is not on cooldown, and there is no bomb in flight
        if(this.Ability3Input == true && this.Ability3OnCooldown == false && this.BombInFlight == false)
        {
            // TODO: Now that a ship can have more than one main gun, need to think of something better than just having bomb use the 0th gun barrel location/rotation
            // Spawn a bomb
            this.bomb = GameController.SpawnBomb(this, this.IFF, this.BombDamage, this.BombRadius, this.GunBarrelObjects[0].transform.position, this.GunBarrelObjects[0].transform.rotation, this.ShipRigidbody.velocity, this.BombSpeed, this.BombLiftime);
            // Set bomb on cooldown
            this.Ability3OnCooldown = true;
            // Set bomb in flight
            this.BombInFlight = true;
            // Record bomb activated time
            this.LastAbility3CooldownStartedTime = Time.time;
        }
        // If bomb is in flight, bomb input is pressed, and time since bomb was activated is more than the bomb primer time
        if(this.BombInFlight == true && this.Ability3Input == true && Time.time - this.LastAbility3CooldownStartedTime > this.BombPrimerTime)
        {
            // Set bomb not in flight
            this.BombInFlight = false;
            // Detonate bomb
            this.bomb.Detonate();
            this.bomb = null;
        }
        // If bomb is on cooldown and time since bomb was last activated is greater than bomb cooldown time
        if(this.Ability3OnCooldown == true && Time.time - this.LastAbility3CooldownStartedTime > this.Ability3CooldownTime)
        {
            // Take bomb off cooldown
            this.Ability3OnCooldown = false;
        }
    }

    // Called when receiving collision from projectile
    public override void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // If shields are above 0 or barrier is active
        if(this.Shields > 0f || this.Ability1Active == true)
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

    // Called when ship receives a damaging attack
    public override void TakeDamage(float _damage)
    {
        // If barrier is not active
        if(this.Ability1Active == false)
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
        // If shield is active
        else
        {
            // Subtract barrier energy drain cost from energy
            this.Energy -= this.BarrierEnergyDrainCost;
        }
    }
}
