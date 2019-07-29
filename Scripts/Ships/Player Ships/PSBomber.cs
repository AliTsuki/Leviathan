using UnityEngine;

public class PSBomber : PlayerShip
{
    // Player-only GameObjects
    private readonly GameObject BarrierObject;
    private Bomb bomb;

    // Ship stats
    // ----Ability 1: Barrier
    private float BarrierEnergyDrainCost; // Energy cost deducted when ship takes damage while barrier is active
    // ----Ability 2: Barrage
    private float BarrageGunCooldownTimeMultiplier; // Main gun cooldown is multiplied by this value during barrage, 0.5f means gun shoots twice as fast
    private uint BarrageShotAmountIncrease; // Number of shots to add per gun barrel fired while barrage is active
    private float BarrageDamageMultiplier; // Shot damage is multiplied by this value
    private float BarrageAccuracyMultiplier; // Shot accuracy is multiplied by this value
    private float BarrageEnergyCostMultiplier; // Shot energy cost is multiplied by this value
    // ----Ability 3: Bombs
    private float BombDamage; // Maximum damage bomb explosion will do to targets in its direct center, damage is calculated as linear falloff percentage of target distance to center of bomb explosion
    private float BombRadius; // Radius in which bomb will deal damage to ships
    private float BombSpeed; // Maximum velocity of fired bombs
    private float BombLifetime; // Number of seconds bomb will fly before it self destructs
    private float BombPrimerTime; // Number of seconds the bomb must fly before it can be detonated

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
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = 200f,
            MaxHealth = 200f,
            Armor = 75f,
            Shields = 100f,
            MaxShields = 100f,
            ShieldRegenSpeed = 1f,
            ShieldCooldownTime = 3f,
            // --Current/Max energy
            Energy = 100f,
            MaxEnergy = 100f,
            EnergyRegenSpeed = 1.5f,
            // --Energy costs
            WarpEnergyCost = 3f,
            GunEnergyCost = 17f,
            // --Acceleration
            EngineCount = 1,
            ImpulseAcceleration = 100f,
            WarpAccelerationMultiplier = 3f,
            StrafeAcceleration = 0f,
            // --Max Speed
            MaxImpulseSpeed = 50f,
            MaxWarpSpeed = 150f,
            MaxStrafeSpeed = 0f,
            MaxRotationSpeed = 0.1f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 1,
            GunShotProjectileType = 5,
            GunCooldownTime = 0.25f,
            GunShotAmount = 1,
            GunShotCurvature = 0.05f,
            GunShotSightCone = 0.8f,
            GunShotDamage = 20f,
            GunShotAccuracy = 99f,
            GunShotSpeed = 100f,
            GunShotLifetime = 2f,
            // ---- Ability Cooldowns
            AbilityDuration = new float[3]{ 4f, 4f, 0f },
            AbilityCooldownTime = new float[3] { 12f, 14f, 12f },
        };
        // -- Abilities
        // ----Ability 1: Barrier
        this.BarrierEnergyDrainCost = 20f;
        // ----Ability 2: Barrage
        this.BarrageGunCooldownTimeMultiplier = 0.5f;
        this.BarrageShotAmountIncrease = 1;
        this.BarrageDamageMultiplier = 0.7f;
        this.BarrageAccuracyMultiplier = 0.7f;
        this.BarrageEnergyCostMultiplier = 0.85f;
        // ----Ability 3: Bombs
        this.BombDamage = 150f;
        this.BombRadius = 50f;
        this.BombSpeed = 40f;
        this.BombLifetime = 3f;
        this.BombPrimerTime = 0.25f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        // Ship type specific objects
        this.BarrierObject = this.ShipObject.transform.Find(GameController.BarrierObjectName).gameObject;
        this.BarrierObject.SetActive(false);
        // Set up Default Cooldowns
        this.DefaultGunCooldownTime = this.Stats.GunCooldownTime;
        this.DefaultGunShotAmount = this.Stats.GunShotAmount;
        this.DefaultGunShotDamage = this.Stats.GunShotDamage;
        this.DefaultGunShotAccuracy = this.Stats.GunShotAccuracy;
        this.DefaultGunEnergyCost = this.Stats.GunEnergyCost;
        this.Initialize();
    }


    // Check ability 1
    protected override void CheckAbility1()
    {
        this.CheckBarrier(0, this.BarrierObject);
    }

    // Check ability 2
    protected override void CheckAbility2()
    {
        this.CheckBarrage(1, this.BarrageGunCooldownTimeMultiplier, this.BarrageShotAmountIncrease, this.BarrageDamageMultiplier, this.BarrageAccuracyMultiplier, this.BarrageEnergyCostMultiplier);
    }

    // Check ability 3
    protected override void CheckAbility3()
    {
        this.bomb = this.CheckBomb(2, this.bomb, this.BombDamage, this.BombRadius, this.BombPrimerTime, this.BombSpeed, this.BombLifetime);
    }

    // Called when receiving collision from projectile
    public override void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // If shields are above 0 or barrier is active
        if(this.Stats.Shields > 0f || this.AbilityActive[0] == true)
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
        if(this.Stats.Health > 0f)
        {
            // If barrier is not active
            if(this.AbilityActive[0] == false)
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
            // If shield is active
            else
            {
                // Subtract barrier energy drain cost from energy
                this.SubtractEnergy(this.BarrierEnergyDrainCost);
            }
        }
    }
}
