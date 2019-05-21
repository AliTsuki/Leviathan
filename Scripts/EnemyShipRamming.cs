using System.Collections.Generic;
using UnityEngine;

// Controls the enemy ships
public class EnemyShipRamming : Ship
{
    // Ramming ship-only GameObjects
    private readonly GameObject BombExplosionPrefab;
    private GameObject BombExplosionObject;
    private readonly float DetonationDamage;
    private readonly float DetonationRadius;

    // Enemy ship constructor
    public EnemyShipRamming(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Enemy;
        this.AItype = AIType.Ramming;
        this.IsPlayer = false;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 25f;
        this.MaxHealth = 25f;
        this.Armor = 50f;
        this.Shields = 10f;
        this.MaxShields = 10f;
        this.ShieldRegenSpeed = 0.5f;
        // Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 75f;
        this.WarpAccelerationMultiplier = 3f;
        this.StrafeAcceleration = 20f;
        this.MaxImpulseSpeed = 75f;
        this.MaxWarpSpeed = 150f;
        this.MaxRotationSpeed = 0.1f;
        this.MaxStrafeSpeed = 20f;
        // Weapon stats
        this.DetonationDamage = 50f;
        this.DetonationRadius = 50f;
        this.ProjectileType = 0;
        this.GunShotAmount = 0f;
        this.ShotCurvature = 0f;
        this.ShotDamage = 0f;
        this.GunShotAccuracy = 0f;
        this.ShotSpeed = 0f;
        this.ShotLifetime = 0f;
        // Cooldowns
        this.GunCooldownTime = 0f;
        this.ShieldCooldownTime = 3f;
        // Energy cost
        this.GunEnergyCost = 17f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // AI fields
        this.MaxTargetAcquisitionRange = 90f;
        this.MaxOrbitRange = 0f;
        this.MaxWeaponsRange = 0f;
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

    // Called when receiving collision from ship
    public override void ReceivedCollisionFromShip(Vector3 _collisionVelocity, GameController.IFF _iff)
    {
        // Apply velocity received from collision
        this.ShipRigidbody.AddRelativeForce(_collisionVelocity * 0.25f, ForceMode.Impulse);
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
                if(distance <= this.DetonationRadius)
                {
                    // Tell ship to take damage relative to it's distance
                    ship.Value.TakeDamage(this.DetonationDamage - (distance / this.DetonationRadius * this.DetonationDamage));
                }
            }
        }
        // Take full life in damage
        this.TakeDamage(this.MaxShields + this.MaxHealth);
    }
}
