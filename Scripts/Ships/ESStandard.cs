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
        // --Health/Armor/Shields
        this.Health = 50f;
        this.MaxHealth = 50f;
        this.Armor = 75f;
        this.Shields = 15f;
        this.MaxShields = 15f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.GunEnergyCost = 17f;
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 60f;
        this.StrafeAcceleration = 20f;
        // --Max Speed
        this.MaxImpulseSpeed = 50f;
        this.MaxStrafeSpeed = 20f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 1;
        this.GunShotProjectileType = 1;
        this.GunCooldownTime = 1f;
        this.GunShotAmount = 1;
        this.GunShotCurvature = 0f;
        this.GunShotSightCone = 0f;
        this.GunShotDamage = 4f;
        this.GunShotAccuracy = 95f;
        this.GunShotSpeed = 120f;
        this.GunShotLifetime = 2f;
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 30f;
        this.MaxWeaponsRange = 40f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.Initialize();
    }
}
