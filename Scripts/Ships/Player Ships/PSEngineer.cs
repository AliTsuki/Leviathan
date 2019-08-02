using System.Collections.Generic;

using UnityEngine;

// Controls the player ship engineer type
public class PSEngineer : PlayerShip
{
    // Engineer objects
    private readonly GameObject ShieldOverchargeObject;
    private readonly GameObject EMPObject;

    // Lists
    private readonly List<DroneShip> Drones = new List<DroneShip>();

    // Ship stats
    // ----Ability 1: Shield Overcharge
    private float ShieldRegenSpeedMultiplier; // 
    private float ShieldCooldownMultiplier; // 
    // ----Ability 2: Drones
    private DroneShip.DroneShipType DroneType; //
    private uint DroneAmount; //
    private uint MaxDroneAmount; //
    private float DroneMaxHealth; //
    private float DroneMaxShields; //
    private float DroneMaxSpeed; //
    private float DroneGunCooldownTime; //
    private uint DroneGunShotAmount; //
    private uint DroneGunShotProjectileType; // 
    private float DroneGunShotDamage; //
    private float DroneGunShotAccuracy; //
    private float DroneGunShotSpeed; //
    private float DroneGunShotLifetime; //
    private float DroneTargetAcquisitionDistance; //
    private float DroneStrafeDistance; //
    private float DroneLeashDistance; //
    // ----Ability 3: EMP
    private float EMPRadius; //
    private float EMPDuration; //
    private float EMPEnergyCost; //

    // Player ship constructor
    public PSEngineer(uint _id)
    {
        this.ID = _id;
        // TODO: Change starting position to be last home zone saved to
        this.StartingPosition = new Vector3(0, 0, 0);
        this.Type = PlayerShipType.Engineer;
        this.AItype = AIType.None;
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = true;
        // Ship stats
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = 150f,
            MaxHealth = 150f,
            Armor = 75f,
            Shields = 150f,
            MaxShields = 150f,
            ShieldRegenSpeed = 1f,
            ShieldCooldownTime = 2.5f,
            // --Current/Max energy
            Energy = 100f,
            MaxEnergy = 100f,
            EnergyRegenSpeed = 1.5f,
            // --Energy costs
            WarpEnergyCost = 3f,
            GunEnergyCost = 30f,
            // --Acceleration
            EngineCount = 2,
            ImpulseAcceleration = 100f,
            WarpAccelerationMultiplier = 3f,
            StrafeAcceleration = 0f,
            // --Max Speed
            MaxImpulseSpeed = 50f,
            MaxWarpSpeed = 150f,
            MaxStrafeSpeed = 0f,
            MaxRotationSpeed = 0.1f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 1,
            GunShotProjectileType = 3,
            GunCooldownTime = 0.38f,
            GunShotAmount = 5,
            GunShotCurvature = 0f,
            GunShotSightCone = 0f,
            GunShotDamage = 10f,
            GunShotAccuracy = 85f,
            GunShotSpeed = 150f,
            GunShotLifetime = 2f,
            // ---- Ability Cooldowns
            AbilityDuration = new float[3] { 4f, 0f, 1f },
            AbilityCooldownTime = new float[3] { 12f, 20f, 14f },
        };
        // -- Abilities
        // ----Ability 1: Shield Overcharge
        this.ShieldRegenSpeedMultiplier = 2f;
        this.ShieldCooldownMultiplier = 0.1f;
        // ----Ability 2: Drones
        this.DroneType = DroneShip.DroneShipType.Standard;
        this.DroneAmount = 1;
        this.MaxDroneAmount = 3;
        this.DroneMaxHealth = 10f;
        this.DroneMaxShields = 20f;
        this.DroneMaxSpeed = 50f;
        this.DroneGunCooldownTime = 1f;
        this.DroneGunShotAmount = 1;
        this.DroneGunShotProjectileType = 0;
        this.DroneGunShotDamage = 3f;
        this.DroneGunShotAccuracy = 97f;
        this.DroneGunShotSpeed = 150f;
        this.DroneGunShotLifetime = 2f;
        this.DroneTargetAcquisitionDistance = 60f;
        this.DroneStrafeDistance = 10f;
        this.DroneLeashDistance = 80f;
        // ----Ability 3: EMP
        this.EMPRadius = 60f;
        this.EMPDuration = 4f;
        this.EMPEnergyCost = 60f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        // Ship type specific objects
        this.ShieldOverchargeObject = this.ShipObject.transform.Find(GameController.ShieldOverchargeObjectName).gameObject;
        this.ShieldOverchargeObject.SetActive(false);
        this.EMPObject = this.ShipObject.transform.Find(GameController.EMPObjectName).gameObject;
        this.EMPObject.SetActive(false);
        // Defaults
        this.DefaultShieldRegenSpeed = this.Stats.ShieldRegenSpeed;
        this.DefaultShieldCooldownTime = this.Stats.ShieldCooldownTime;
        this.Initialize();
    }


    // Check ability 1
    protected override void CheckAbility1()
    {
        this.CheckShieldOvercharge(0, this.ShieldOverchargeObject, this.ShieldRegenSpeedMultiplier, this.ShieldCooldownMultiplier);
    }

    // Check ability 2
    protected override void CheckAbility2()
    {
        this.CheckDrones(1, this.Drones, this.DroneAmount, this.MaxDroneAmount, this.DroneType, this.DroneMaxHealth, this.DroneMaxShields, this.DroneMaxSpeed, this.DroneGunShotProjectileType, this.DroneGunCooldownTime, this.DroneGunShotAmount, this.DroneGunShotDamage, this.DroneGunShotAccuracy, this.DroneGunShotSpeed, this.DroneGunShotLifetime, this.DroneTargetAcquisitionDistance, this.DroneStrafeDistance, this.DroneLeashDistance);
    }

    // Check ability 3
    protected override void CheckAbility3()
    {
        this.CheckEMP(2, this.EMPObject, this.EMPRadius, this.EMPDuration, this.EMPEnergyCost);
    }
}
