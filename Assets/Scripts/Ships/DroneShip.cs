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

    public DroneShip(uint id, Ship parent, List<DroneShip> parentDroneList, DroneShipType type, Vector3 startingPosition, float maxHealth, 
        float maxShields, float maxSpeed, uint gunShotProjectileType, float gunCooldownTime, uint gunShotAmount, float gunShotDamage, 
        float gunShotAccuracy, float gunShotSpeed, float gunShotLifetime, float maxTargetAcquisitionDistance, float maxStrafeDistance, float maxLeashDistance)
    {
        this.ID = id;
        this.Parent = parent;
        this.ParentDroneList = parentDroneList;
        this.Type = type;
        this.AItype = AIType.Drone;
        this.StartingPosition = startingPosition;
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = false;
        // Ship stats
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = maxHealth,
            MaxHealth = maxHealth,
            Armor = 99f,
            Shields = maxShields,
            MaxShields = maxShields,
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
            MaxImpulseSpeed = maxSpeed,
            MaxWarpSpeed = 0f,
            MaxStrafeSpeed = 20f,
            MaxRotationSpeed = 0.1f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 1,
            GunShotProjectileType = gunShotProjectileType,
            GunCooldownTime = gunCooldownTime,
            GunShotAmount = gunShotAmount,
            GunShotCurvature = 0f,
            GunShotSightCone = 0f,
            GunShotDamage = gunShotDamage,
            GunShotAccuracy = gunShotAccuracy,
            GunShotSpeed = gunShotSpeed,
            GunShotLifetime = gunShotLifetime,
        };
        // Self Destruct
        this.BombRadius = 25f;
        this.BombDamage = 25f;
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = maxTargetAcquisitionDistance;
        this.MaxOrbitRange = maxStrafeDistance;
        this.MaxWeaponsRange = maxTargetAcquisitionDistance;
        this.MaxLeashDistance = maxLeashDistance;
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
        UIController.RemoveHealthbar(this.ID);
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
