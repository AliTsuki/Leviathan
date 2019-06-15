using System.Collections.Generic;

using UnityEngine;

public class PSEngineer : PlayerShip
{
    private GameObject ShieldOverchargeObject;
    private GameObject EMPObject;

    // Lists
    public List<DroneShip> Drones = new List<DroneShip>();

    // Ship stats
    // ----Ability 1: Shield Overcharge
    public float ShieldRegenSpeedMultiplier; // 
    public float ShieldCooldownMultiplier; // 
    // ----Ability 2: Drones
    public DroneShip.DroneShipType DroneType; //
    public uint DroneAmount; //
    public uint MaxDroneAmount; //
    public float DroneMaxHealth; //
    public float DroneMaxShields; //
    public float DroneMaxSpeed; //
    public float DroneGunCooldownTime; //
    public uint DroneGunShotAmount; //
    public uint DroneGunShotProjectileType; // 
    public float DroneGunShotDamage; //
    public float DroneGunShotAccuracy; //
    public float DroneGunShotSpeed; //
    public float DroneGunShotLifetime; //
    public float DroneTargetAcquisitionDistance; //
    public float DroneStrafeDistance; //
    public float DroneLeashDistance; //
    // ----Ability 3: EMP
    public float EMPRadius; //
    public float EMPDuration; //
    public float EMPEnergyCost; //

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
        // --Health/Armor/Shields
        this.Health = 150f;
        this.MaxHealth = 150f;
        this.Armor = 75f;
        this.Shields = 150f;
        this.MaxShields = 150f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 2.5f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 30f;
        // --Acceleration
        this.EngineCount = 2;
        this.ImpulseAcceleration = 100f;
        this.WarpAccelerationMultiplier = 3f;
        this.StrafeAcceleration = 0f;
        // --Max Speed
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 150f;
        this.MaxStrafeSpeed = 0f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 1;
        this.GunShotProjectileType = 3;
        this.GunCooldownTime = 0.38f;
        this.GunShotAmount = 5;
        this.GunShotCurvature = 0f;
        this.GunShotSightCone = 0f;
        this.GunShotDamage = 10f;
        this.GunShotAccuracy = 85f;
        this.GunShotSpeed = 150f;
        this.GunShotLifetime = 2f;
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
        // ---- Ability Cooldowns
        this.AbilityDuration[0] = 4f;
        this.AbilityCooldownTime[0] = 12f;
        this.AbilityDuration[1] = 0f;
        this.AbilityCooldownTime[1] = 20f;
        this.AbilityDuration[2] = 1f;
        this.AbilityCooldownTime[2] = 14f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        // Ship type specific objects
        this.ShieldOverchargeObject = this.ShipObject.transform.Find(GameController.ShieldOverchargeObjectName).gameObject;
        this.ShieldOverchargeObject.SetActive(false);
        this.EMPObject = this.ShipObject.transform.Find(GameController.EMPObjectName).gameObject;
        this.EMPObject.SetActive(false);
        // Audio levels
        this.ImpulseEngineAudioStep = 0.05f;
        this.ImpulseEngineAudioMinVol = 0.25f;
        this.ImpulseEngineAudioMaxVol = 1f;
        // Defaults
        this.DefaultShieldRegenSpeed = this.ShieldRegenSpeed;
        this.DefaultShieldCooldownTime = this.ShieldCooldownTime;
        this.Initialize();
    }


    // Check ability 1
    public override void CheckAbility1()
    {
        Abilities.CheckShieldOvercharge(this, 0, this.ShieldOverchargeObject, this.ShieldRegenSpeedMultiplier, this.ShieldCooldownMultiplier);
    }

    // Check ability 2
    public override void CheckAbility2()
    {
        Abilities.CheckDrones(this, 1, this.Drones, this.DroneAmount, this.MaxDroneAmount, this.DroneType, this.DroneMaxHealth, this.DroneMaxShields, this.DroneMaxSpeed, this.DroneGunShotProjectileType, this.DroneGunCooldownTime, this.DroneGunShotAmount, this.DroneGunShotDamage, this.DroneGunShotAccuracy, this.DroneGunShotSpeed, this.DroneGunShotLifetime, this.DroneTargetAcquisitionDistance, this.DroneStrafeDistance, this.DroneLeashDistance);
    }

    // Check ability 3
    public override void CheckAbility3()
    {
        Abilities.CheckEMP(this, 2, this.EMPObject, this.EMPRadius, this.EMPDuration, this.EMPEnergyCost);
    }
}
