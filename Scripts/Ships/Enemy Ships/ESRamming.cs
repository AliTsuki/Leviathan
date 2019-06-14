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
        // --Health/Armor/Shields
        this.Health = 10f;
        this.MaxHealth = 10f;
        this.Armor = 50f;
        this.Shields = 5f;
        this.MaxShields = 5f;
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
        this.BombDamage = 25f;
        this.BombRadius = 25f;
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
            // If this ship is not EMPed
            if(this.IsEMPed == false)
            {
                // Detonate warhead
                Abilities.Detonate(this, this.BombExplosionPrefab, this.BombScale, this.BombDamage, this.BombRadius);
            }
        }
    }
}
