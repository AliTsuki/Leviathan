using UnityEngine;

// Standard AI enemy ship
public class ESStandard : EnemyShip
{
    // Enemy ship standard constructor
    public ESStandard(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.Type = EnemyShipType.Standard;
        this.AItype = AIType.Standard;
        this.IFF = GameController.IFF.Enemy;
        this.IsPlayer = false;
        // Ship stats
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = 70f,
            MaxHealth = 70f,
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
            GunBarrelCount = 1,
            GunShotProjectileType = 1,
            GunCooldownTime = 1f,
            GunShotAmount = 1,
            GunShotCurvature = 0f,
            GunShotSightCone = 0f,
            GunShotDamage = 6f,
            GunShotAccuracy = 95f,
            GunShotSpeed = 120f,
            GunShotLifetime = 2f,
        };
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 30f;
        this.MaxWeaponsRange = 40f;
        // Experience
        this.XP = (uint)(this.Stats.MaxHealth + this.Stats.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.RandomNumGen.Next(0, 360), 0));
        this.Initialize();
    }
}
