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
    public uint BarrageShotAmountIncrease; // Number of shots to add per gun barrel fired while barrage is active
    public float BarrageDamageMultiplier; // Shot damage is multiplied by this value
    public float BarrageAccuracyMultiplier; // Shot accuracy is multiplied by this value
    public float BarrageEnergyCostMultiplier; // Shot energy cost is multiplied by this value
    // ----Ability 3: Bombs
    public float BombDamage; // Maximum damage bomb explosion will do to targets in its direct center, damage is calculated as linear falloff percentage of target distance to center of bomb explosion
    public float BombRadius; // Radius in which bomb will deal damage to ships
    public float BombSpeed; // Maximum velocity of fired bombs
    public float BombLifetime; // Number of seconds bomb will fly before it self destructs
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
        this.GunShotDamage = 20f;
        this.GunShotAccuracy = 99f;
        this.GunShotSpeed = 100f;
        this.GunShotLifetime = 2f;
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
        // ---- Ability Cooldowns
        this.AbilityDuration[0] = 2.5f;
        this.AbilityCooldownTime[0] = 12f;
        this.AbilityDuration[1] = 4f;
        this.AbilityCooldownTime[1] = 14f;
        this.AbilityDuration[2] = 0f;
        this.AbilityCooldownTime[2] = 12f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        // Ship type specific objects
        this.BarrierObject = this.ShipObject.transform.Find(GameController.BarrierObjectName).gameObject;
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
        Abilities.CheckBarrier(this, 0, this.BarrierObject);
    }

    // Check ability 2
    public override void CheckAbility2()
    {
        Abilities.CheckBarrage(this, 1, this.BarrageGunCooldownTimeMultiplier, this.BarrageShotAmountIncrease, this.BarrageDamageMultiplier, this.BarrageAccuracyMultiplier, this.BarrageEnergyCostMultiplier);
    }

    // Check ability 3
    public override void CheckAbility3()
    {
        this.bomb = Abilities.CheckBomb(this, 2, this.bomb, this.BombDamage, this.BombRadius, this.BombPrimerTime, this.BombSpeed, this.BombLifetime);
    }

    // Called when receiving collision from projectile
    public override void ReceivedCollisionFromProjectile(float _damage, Vector3 _projectileStrikeLocation)
    {
        // If shields are above 0 or barrier is active
        if(this.Shields > 0f || this.AbilityActive[0] == true)
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
        if(this.Health > 0f)
        {
            // If barrier is not active
            if(this.AbilityActive[0] == false)
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
                this.SubtractEnergy(this.BarrierEnergyDrainCost);
            }
        }
    }
}
