using UnityEngine;

// Standard AI enemy ship
public class EnemyShipStandard : EnemyShip
{
    // Enemy ship standard constructor
    public EnemyShipStandard(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Enemy;
        this.AItype = AIType.Standard;
        this.IsPlayer = false;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 50f;
        this.MaxHealth = 50f;
        this.Armor = 75f;
        this.Shields = 15f;
        this.MaxShields = 15f;
        this.ShieldRegenSpeed = 0.5f;
        // Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 60f;
        this.WarpAccelerationMultiplier = 3f;
        this.StrafeAcceleration = 20f;
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 150f;
        this.MaxRotationSpeed = 0.1f;
        this.MaxStrafeSpeed = 20f;
        // Weapon stats
        this.GunShotProjectileType = 1;
        this.GunShotAmount = 1f;
        this.GunShotCurvature = 0f;
        this.GunShotDamage = 4f;
        this.GunShotAccuracy = 95f;
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
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 30f;
        this.MaxWeaponsRange = 40f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyStandardPrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.Initialize();
    }
}
