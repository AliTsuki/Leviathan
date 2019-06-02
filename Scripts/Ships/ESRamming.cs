using System.Collections.Generic;

using UnityEngine;

// Suicide Bomber enemy ship
public class ESRamming : EnemyShip
{
    // Ramming ship-only GameObjects
    private readonly GameObject BombExplosionPrefab;
    private GameObject BombExplosionObject;

    // Ship stats
    private readonly float BombDamage;
    private readonly float BombRadius;

    // Enemy ship constructor
    public ESRamming(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.Type = EnemyShipType.Ramming;
        this.AItype = AIType.Ramming;
        this.IFF = GameController.IFF.Enemy;
        this.IsPlayer = false;
        // Ship stats
        // --Health/Armor/Shields
        this.Health = 25f;
        this.MaxHealth = 25f;
        this.Armor = 50f;
        this.Shields = 10f;
        this.MaxShields = 10f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 150f;
        // --Max Speed
        this.MaxImpulseSpeed = 150f;
        this.MaxRotationSpeed = 0.15f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 0;
        // ----Bomb
        this.BombDamage = 50f;
        this.BombRadius = 50f;
        // AI fields
        this.AIAimAssist = false;
        this.MaxTargetAcquisitionRange = 80f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.BombExplosionPrefab = Resources.Load<GameObject>(GameController.BombExplostionPrefabName);
        this.Initialize();
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
