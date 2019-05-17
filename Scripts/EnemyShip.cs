using UnityEngine;

// Controls the enemy ships
public class EnemyShip : Ship
{
    // Enemy ship constructor
    public EnemyShip(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Enemy;
        this.IsPlayer = false;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 100f;
        this.MaxHealth = 100f;
        this.Armor = 1f;
        this.Shields = 100f;
        this.MaxShields = 100f;
        // Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 30f;
        this.WarpAccelMultiplier = 3f;
        this.MaxImpulseSpeed = 40f;
        this.MaxWarpSpeed = 150f;
        // Weapon stats
        this.ProjectileType = 1;
        this.ShotDamage = 5f;
        this.ShotAccuracy = 1f;
        this.ShotSpeed = 10f;
        this.ShotLifetime = 2.5f;
        this.ShotCurvature = 0f;
        // Cooldowns
        this.ShotCooldownTime = 1f;
        this.RegenShieldCooldownTime = 10f;
        this.ShieldCooldownTime = 10f;
        this.BombCooldownTime = 10f;
        this.ScannerCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 3f;
        this.ShotEnergyCost = 17f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // AI fields
        this.MaxTargetAcquisitionRange = 50f;
        this.MaxOrbitRange = 25f;
        this.MaxWeaponsRange = 30f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load(GameController.EnemyPrefabName, typeof(GameObject)) as GameObject;
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        this.Start();
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
            if(AIController.ShouldAccelerate(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
            {
                this.ImpulseInput = true;
            }
            else
            {
                this.ImpulseInput = false;
            }
            // Use AI to figure out if ship should strafe target
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
                this.FireInput = true;
            }
            else
            {
                this.FireInput = false;
            }
        }
        // If unable to acquire target set wandering to true
        else
        {
            this.IsWandering = true;
        }
        if(this.IsWandering == true)
        {
            this.Wander();
        }
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // If there is a current target
        if(this.CurrentTarget != null)
        {
            // Get rotation to face target
            this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
        }
    }
}
