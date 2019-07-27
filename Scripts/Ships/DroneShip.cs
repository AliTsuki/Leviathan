using System.Collections.Generic;

using UnityEngine;

public class DroneShip : Ship
{
    // Drone ship-only GameObjects
    private readonly GameObject BombExplosionPrefab;
    private readonly GameObject BombExplosionObject;

    // Ship stats
    private readonly float BombDamage;
    private readonly float BombRadius;
    private readonly Vector3 BombScale = new Vector3(0.5f, 0.5f, 0.5f);

    // Parent list
    private readonly List<DroneShip> ParentDroneList;

    // Drone ship types
    public enum DroneShipType
    {
        Standard,
    }
    public DroneShipType Type { get; private set; }

    public DroneShip(uint _id, Ship _parent, List<DroneShip> _parentDroneList, DroneShipType _type, Vector3 _startingPosition, float _maxHealth, float _maxShields, float _maxSpeed, uint _gunShotProjectileType, float _gunCooldownTime, uint _gunShotAmount, float _gunShotDamage, float _gunShotAccuracy, float _gunShotSpeed, float _gunShotLifetime, float _maxTargetAcquisitionDistance, float _maxStrafeDistance, float _maxLeashDistance)
    {
        this.ID = _id;
        this.Parent = _parent;
        this.ParentDroneList = _parentDroneList;
        this.Type = _type;
        this.AItype = AIType.Drone;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = false;
        // Ship stats
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = _maxHealth,
            MaxHealth = _maxHealth,
            Armor = 99f,
            Shields = _maxShields,
            MaxShields = _maxShields,
            ShieldRegenSpeed = 1f,
            ShieldCooldownTime = 3f,
            // --Current/Max energy
            Energy = 100f,
            MaxEnergy = 100f,
            EnergyRegenSpeed = 1.5f,
            // --Energy costs
            WarpEnergyCost = 3f,
            GunEnergyCost = 17f,
            // --Acceleration
            EngineCount = 1,
            ImpulseAcceleration = 100f,
            WarpAccelerationMultiplier = 0f,
            StrafeAcceleration = 50f,
            // --Max Speed
            MaxImpulseSpeed = _maxSpeed,
            MaxWarpSpeed = 0f,
            MaxStrafeSpeed = 20f,
            MaxRotationSpeed = 0.1f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 1,
            GunShotProjectileType = _gunShotProjectileType,
            GunCooldownTime = _gunCooldownTime,
            GunShotAmount = _gunShotAmount,
            GunShotCurvature = 0f,
            GunShotSightCone = 0f,
            GunShotDamage = _gunShotDamage,
            GunShotAccuracy = _gunShotAccuracy,
            GunShotSpeed = _gunShotSpeed,
            GunShotLifetime = _gunShotLifetime,
        };
        // Self Destruct
        this.BombRadius = 25f;
        this.BombDamage = 25f;
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = _maxTargetAcquisitionDistance;
        this.MaxOrbitRange = _maxStrafeDistance;
        this.MaxWeaponsRange = _maxTargetAcquisitionDistance;
        this.MaxLeashDistance = _maxLeashDistance;
        this.OrbitParentRange = 25f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.DronePrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        this.BombExplosionPrefab = Resources.Load<GameObject>(GameController.BombExplostionPrefabName);
        this.Initialize();
    }


    // Self destruct ship
    public void Detonate()
    {
        this.Detonate(this.BombExplosionPrefab, this.BombScale, this.BombDamage, this.BombRadius);
    }

    // Called when ship is destroyed by damage, grants XP
    protected override void Kill()
    {
        // Set to not alive
        this.Alive = false;
        // Tell UI to remove healthbar for this ship
        UIControllerNew.RemoveHealthbar(this.ID);
        // Create an explosion
        this.Explosion = GameObject.Instantiate(this.ExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(this.Explosion, 1f);
        // Destroy ship object
        GameObject.Destroy(this.ShipObject);
        // Add ship to removal list
        GameController.AddShipToRemovalList(this.ID);
        // Remove drone from parents drone list
        this.ParentDroneList.Remove(this);
    }
}
