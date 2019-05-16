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
        this.Health = 0f;
        this.MaxHealth = 100f;
        this.Armor = 0f;
        this.MaxArmor = 100f;
        this.Shields = 0f;
        this.MaxShields = 100f;
        // Current/Max energy
        this.Energy = 0f;
        this.MaxEnergy = 100f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 15f;
        this.WarpAccelMultiplier = 3f;
        this.MaxImpulseSpeed = 20f;
        this.MaxWarpSpeed = 0f;
        // Weapon stats
        this.ShotDamage = 1f;
        this.ShotSpeed = 5f;
        this.ShotLifetime = 2.5f;
        this.ShotCurvature = 0f;
        // Cooldowns
        this.ShotCooldownTime = 0.5f;
        this.ShieldCooldownTime = 10f;
        this.BombCooldownTime = 10f;
        this.ScannerCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 5f;
        this.ShotEnergyCost = 5f;
        // AI fields
        this.MaxTargetAcquisitionRange = 50f;
        this.MaxOrbitRange = 20f;
        this.MaxWeaponsRange = 25f;
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
            // If a new target can't be acquired
            if(this.CurrentTarget == null)
            {
                // Wander around until finding a target
                this.Wander();
            }
        }
        // If there is a current target and it is alive
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
        {
            // Use AI to figure out if ship should accelerate
            if(AIController.ShouldAccelerate(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
            {
                this.ImpulseInput = true;
            }
            else
            {
                this.ImpulseInput = false;
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
