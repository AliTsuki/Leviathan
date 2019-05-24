using System.Collections.Generic;

using UnityEngine;

// Suicide Bomber enemy ship
public class EnemyShipRamming : EnemyShip
{
    // Ramming ship-only GameObjects
    private readonly GameObject BombExplosionPrefab;
    private GameObject BombExplosionObject;

    // Enemy ship constructor
    public EnemyShipRamming(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Enemy;
        this.AItype = AIType.Ramming;
        this.IsPlayer = false;
        // Ship stats
        // --Health/Armor/Shields
        this.Health = 25f;
        this.MaxHealth = 25f;
        this.Armor = 50f;
        this.Shields = 10f;
        this.MaxShields = 10f;
        this.ShieldRegenSpeed = 1f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.WarpEnergyCost = 0f; // Unused by enemy
        this.GunEnergyCost = 0f; // Unused by enemy
        this.BarrierEnergyDrainCost = 0f; // Unused by enemy
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 100f;
        this.WarpAccelerationMultiplier = 0f; // Unused by enemy
        this.StrafeAcceleration = 0f; // Unused by enemy
        // --Max Speed
        this.MaxImpulseSpeed = 100f;
        this.MaxWarpSpeed = 0f; // Unused by enemy
        this.MaxStrafeSpeed = 0f; // Unused by enemy
        this.MaxRotationSpeed = 0.15f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 0;
        this.GunShotProjectileType = 0; // Unused by enemy
        this.GunCooldownTime = 0f; // Unused by enemy
        this.GunShotAmount = 0; // Unused by enemy
        this.GunShotCurvature = 0f; // unused by enemy
        this.GunShotDamage = 0f; // Unused by enemy
        this.GunShotAccuracy = 0f; // Unused by enemy
        this.GunShotSpeed = 0f; // Unused by enemy
        this.GunShotLifetime = 0f; // Unused by enemy
        // ----Bombs
        this.BombCurvature = 0f; // Unused by enemy
        this.BombDamage = 50f;
        this.BombRadius = 50f;
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
        this.AIAimAssist = false;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 0f; // Unused by enemy
        this.MaxWeaponsRange = 0f; // Unused by enemy
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyRammingPrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.BombExplosionPrefab = Resources.Load<GameObject>(GameController.BombExplostionPrefabName);
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

    // Called when receiving collision from ship
    public override void ReceivedCollisionFromShip(Vector3 _collisionVelocity, GameController.IFF _iff)
    {
        // Apply velocity received from collision
        this.ShipRigidbody.AddRelativeForce(_collisionVelocity * 0.20f, ForceMode.Impulse);
        // If ship is different faction
        if(_iff != this.IFF)
        {
            // If armor percentage is above 100, cap it at 100
            Mathf.Clamp(this.Armor, 0, 100);
            // Take impact damage less armor percentage
            this.TakeDamage(_collisionVelocity.magnitude * ((100 - this.Armor) / 100));
            // Detonate warhead
            this.Detonate();
        }
    }

    // Self destruct ship
    public void Detonate()
    {
        // Spawn explosion object
        this.BombExplosionObject = GameObject.Instantiate(this.BombExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(this.BombExplosionObject, 1);
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship is other faction and currently alive
            if(ship.Value.IFF != this.IFF && ship.Value.Alive == true)
            {
                // Get distance from ship
                float distance = Vector3.Distance(this.ShipObject.transform.position, ship.Value.ShipObject.transform.position);
                // If distance is less than radius
                if(distance <= this.BombRadius)
                {
                    // Tell ship to take damage relative to it's distance
                    ship.Value.TakeDamage(this.BombDamage - (distance / this.BombRadius * this.BombDamage));
                }
            }
        }
        // Take full life in damage
        this.TakeDamage(this.MaxShields + this.MaxHealth);
    }
}
