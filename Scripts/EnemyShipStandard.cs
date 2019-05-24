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
        // --Health/Armor/Shields
        this.Health = 50f;
        this.MaxHealth = 50f;
        this.Armor = 75f;
        this.Shields = 15f;
        this.MaxShields = 15f;
        this.ShieldRegenSpeed = 1f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.WarpEnergyCost = 0f; // Unused by enemy
        this.GunEnergyCost = 17f;
        this.BarrierEnergyDrainCost = 0f; // Unused by enemy
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 60f;
        this.WarpAccelerationMultiplier = 0f; // Unused by enemy
        this.StrafeAcceleration = 20f;
        // --Max Speed
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 0f; // Unused by enemy
        this.MaxStrafeSpeed = 20f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 1;
        this.GunShotProjectileType = 1;
        this.GunCooldownTime = 1f;
        this.GunShotAmount = 1;
        this.GunShotCurvature = 0f;
        this.GunShotDamage = 4f;
        this.GunShotAccuracy = 95f;
        this.GunShotSpeed = 120f;
        this.GunShotLifetime = 2f;
        // ----Bombs
        this.BombCurvature = 0f; // Unused by enemy
        this.BombDamage = 0f; // Unused by enemy
        this.BombRadius = 0f; // Unused by enemy
        this.BombSpeed = 0f; // Unused by enemy
        this.BombLiftime = 0f; // Unused by enemy
        this.BombPrimerTime = 0f; // Unused by enemy
        // ----Barrage
        this.BarrageGunCooldownTimeMultiplier = 0f; // Unused by enemy
        this.BarrageShotAmountIncrease = 0; // Unused by enemy
        this.BarrageDamageMultiplier = 0f; // Unused by enemy
        this.BarrageAccuracyMultiplier = 0f; // Unused by enemy
        this.BarrageEnergyCostMultiplier = 0f; // Unused by enemy
        // --Cooldowns
        this.ShieldCooldownTime = 3f;
        this.BarrierDuration = 0f; // Unused by enemy
        this.BarrierCooldownTime = 0f; // Unused by enemy
        this.BombCooldownTime = 0f; // Unused by enemy
        this.BarrageDuration = 0f; // Unused by enemy
        this.BarrageCooldownTime = 0f; // Unused by enemy
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 30f;
        this.MaxWeaponsRange = 40f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyStandardPrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.Initialize();
    }
}
