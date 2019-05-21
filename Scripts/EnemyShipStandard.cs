using UnityEngine;

// Controls the enemy ships
public class EnemyShipStandard : Ship
{
    // Enemy ship constructor
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
        this.ProjectileType = 1;
        this.GunShotAmount = 1f;
        this.ShotCurvature = 0f;
        this.ShotDamage = 4f;
        this.GunShotAccuracy = 95f;
        this.ShotSpeed = 120f;
        this.ShotLifetime = 1f;
        // Cooldowns
        this.GunCooldownTime = 1f;
        this.ShieldCooldownTime = 3f;
        this.BarrierCooldownTime = 10f;
        this.BombCooldownTime = 10f;
        this.ScannerCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 17f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // AI fields
        this.MaxTargetAcquisitionRange = 90f;
        this.MaxOrbitRange = 30f;
        this.MaxWeaponsRange = 40f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyStandardPrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.Initialize();
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
        // If there is a current target
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
        {
            // Get rotation to face target
            this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
        }
    }
}
