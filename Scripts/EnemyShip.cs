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
        this.Health = 50;
        this.MaxHealth = 50;
        this.Armor = 75;
        this.Shields = 15;
        this.MaxShields = 15;
        this.ShieldRegenSpeed = 0.5f;
        // Current/Max energy
        this.Energy = 100;
        this.MaxEnergy = 100;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 40;
        this.WarpAccelMultiplier = 3;
        this.StrafeAcceleration = 10;
        this.MaxImpulseSpeed = 40;
        this.MaxWarpSpeed = 150;
        this.MaxRotationSpeed = 0.1f;
        this.MaxStrafeSpeed = 10;
        // Weapon stats
        this.ProjectileType = 1;
        this.ShotAmount = 1;
        this.ShotDamage = 4;
        this.ShotAccuracy = 90;
        this.ShotSpeed = 10;
        this.ShotLifetime = 2.5f;
        this.ShotCurvature = 0;
        // Cooldowns
        this.ShotCooldownTime = 1;
        this.RegenShieldCooldownTime = 3;
        this.ShieldCooldownTime = 10;
        this.BombCooldownTime = 10;
        this.ScannerCooldownTime = 10;
        // Energy cost
        this.WarpEnergyCost = 3;
        this.ShotEnergyCost = 17;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // AI fields
        this.MaxTargetAcquisitionRange = 75;
        this.MaxOrbitRange = 25;
        this.MaxWeaponsRange = 30;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load(GameController.EnemyPrefabName, typeof(GameObject)) as GameObject;
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
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
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
        {
            // Get rotation to face target
            this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
        }
    }
}
