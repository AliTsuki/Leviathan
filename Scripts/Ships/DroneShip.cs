using UnityEngine;

public class DroneShip : Ship
{
    // Drone ship types
    public enum DroneShipType
    {
        Standard,
    }
    public DroneShipType Type;

    public DroneShip(uint _id, PSEngineer _parent, DroneShipType _type, Vector3 _startingPosition, float _maxHealth, float _maxShields, float _maxSpeed, uint _gunShotProjectileType, float _gunCooldownTime, uint _gunShotAmount, float _gunShotDamage, float _gunShotAccuracy, float _gunShotSpeed, float _gunShotLifetime, float _maxTargetAcquisitionDistance, float _maxStrafeDistance, float _maxLeashDistance)
    {
        this.ID = _id;
        this.Parent = _parent;
        this.Type = _type;
        this.AItype = AIType.Drone;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = false;
        // Ship stats
        // --Health/Armor/Shields
        this.Health = _maxHealth;
        this.MaxHealth = _maxHealth;
        this.Armor = 99f;
        this.Shields = _maxShields;
        this.MaxShields = _maxShields;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 17f;
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 100f;
        this.WarpAccelerationMultiplier = 0f;
        this.StrafeAcceleration = 50f;
        // --Max Speed
        this.MaxImpulseSpeed = _maxSpeed;
        this.MaxWarpSpeed = 0f;
        this.MaxStrafeSpeed = 20f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 1;
        this.GunShotProjectileType = _gunShotProjectileType;
        this.GunCooldownTime = _gunCooldownTime;
        this.GunShotAmount = _gunShotAmount;
        this.GunShotCurvature = 0f;
        this.GunShotSightCone = 0f;
        this.GunShotDamage = _gunShotDamage;
        this.GunShotAccuracy = _gunShotAccuracy;
        this.GunShotSpeed = _gunShotSpeed;
        this.GunShotLifetime = _gunShotLifetime;
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
        this.Initialize();
    }


    // Called when ship is destroyed by damage, grants XP
    public override void Kill()
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
        GameController.ShipsToRemove.Add(this.ID);
        this.Parent.Drones.Remove(this);
    }
}
