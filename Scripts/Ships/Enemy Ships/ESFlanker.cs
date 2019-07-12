using UnityEngine;

// Standard AI enemy ship
public class ESFlanker : EnemyShip
{
    // Flanker GameObjects
    private readonly GameObject FlankTeleportObject;

    // Ship Stats
    private readonly float FlankTeleportRange;

    // Enemy ship standard constructor
    public ESFlanker(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.Type = EnemyShipType.Flanker;
        this.AItype = AIType.Flanker;
        this.IFF = GameController.IFF.Enemy;
        this.IsPlayer = false;
        // Ship stats
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = 50f,
            MaxHealth = 50f,
            Armor = 75f,
            Shields = 25f,
            MaxShields = 25f,
            ShieldRegenSpeed = 1f,
            ShieldCooldownTime = 3f,
            // --Current/Max energy
            Energy = 100f,
            MaxEnergy = 100f,
            EnergyRegenSpeed = 1.5f,
            // --Energy costs
            GunEnergyCost = 17f,
            // --Acceleration
            EngineCount = 1,
            ImpulseAcceleration = 60f,
            StrafeAcceleration = 20f,
            // --Max Speed
            MaxImpulseSpeed = 50f,
            MaxStrafeSpeed = 20f,
            MaxRotationSpeed = 0.1f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 2,
            GunShotProjectileType = 2,
            GunCooldownTime = 1f,
            GunShotAmount = 1,
            GunShotCurvature = 0f,
            GunShotSightCone = 0f,
            GunShotDamage = 2f,
            GunShotAccuracy = 95f,
            GunShotSpeed = 120f,
            GunShotLifetime = 2f,
            // Ability Cooldowns
            AbilityDuration = new float[1] { 1f },
            AbilityCooldownTime = new float[1] { 5f },
        };
        // Abilities
        this.FlankTeleportRange = 20f;
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 40f;
        this.MaxWeaponsRange = 50f;
        this.MaxAbilityRange = 40f;
        // Experience
        this.XP = (uint)(this.Stats.MaxHealth + this.Stats.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.RandomNumGen.Next(0, 360), 0));
        this.FlankTeleportObject = this.ShipObject.transform.Find(GameController.EMPObjectName).gameObject;
        this.Initialize();
    }


    protected override void CheckAbility1()
    {
        if(this.IsEMPed == false)
        {
            this.CheckFlankTeleport(0, this.FlankTeleportObject, this.FlankTeleportRange);
        }
    }
}
