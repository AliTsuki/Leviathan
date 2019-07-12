using System.Collections.Generic;

using UnityEngine;

// Suicide Bomber enemy ship
public class ESRamming : EnemyShip
{
    // Ramming ship-only GameObjects
    private readonly GameObject BombExplosionPrefab;

    // Ship stats
    private readonly float BombDamage;
    private readonly float BombRadius;
    private readonly Vector3 BombScale = new Vector3(1, 1, 1);

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
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = 10f,
            MaxHealth = 10f,
            Armor = 50f,
            Shields = 5f,
            MaxShields = 5f,
            ShieldRegenSpeed = 1f,
            ShieldCooldownTime = 3f,
            // --Current/Max energy
            Energy = 100f,
            MaxEnergy = 100f,
            EnergyRegenSpeed = 1.5f,
            // --Acceleration
            EngineCount = 1,
            ImpulseAcceleration = 150f,
            // --Max Speed
            MaxImpulseSpeed = 150f,
            MaxRotationSpeed = 0.15f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 0,
        };
        // ----Bomb
        this.BombDamage = 25f;
        this.BombRadius = 25f;
        // AI fields
        this.AIAimAssist = false;
        this.MaxTargetAcquisitionRange = 80f;
        // Experience
        this.XP = (uint)(this.Stats.MaxHealth + this.Stats.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.RandomNumGen.Next(0, 360), 0));
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
            Mathf.Clamp(this.Stats.Armor, 0, 100);
            // Take impact damage less armor percentage
            this.TakeDamage(_collisionVelocity.magnitude * ((100 - this.Stats.Armor) / 100));
            // If this ship is not EMPed
            if(this.IsEMPed == false)
            {
                // Detonate warhead
                this.Detonate(this.BombExplosionPrefab, this.BombScale, this.BombDamage, this.BombRadius);
            }
        }
    }
}
