using System;
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
        this.Health = 200f;
        this.MaxHealth = 200f;
        this.Armor = 75f;
        this.Shields = 100f;
        this.MaxShields = 100f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 25f;
        // --Acceleration
        this.EngineCount = 1;
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
        this.GunCooldownTime = 0.5f;
        this.GunShotAmount = 5;
        this.GunShotCurvature = 0f;
        this.GunShotSightCone = 0f;
        this.GunShotDamage = 15f;
        this.GunShotAccuracy = 85f;
        this.GunShotSpeed = 150f;
        this.GunShotLifetime = 2f;
        // -- Abilities
        // ----Ability 1: Shield Overcharge
        this.ShieldRegenSpeedMultiplier = 2f;
        this.ShieldCooldownMultiplier = 0.1f;
        // ----Ability 2: Drones
        this.DroneType = DroneShip.DroneShipType.Standard;
        this.DroneAmount = 3;
        this.MaxDroneAmount = 3;
        this.DroneMaxHealth = 25f;
        this.DroneMaxShields = 10f;
        this.DroneMaxSpeed = 50f;
        this.DroneGunCooldownTime = 1f;
        this.DroneGunShotAmount = 1;
        this.DroneGunShotProjectileType = 4;
        this.DroneGunShotDamage = 5f;
        this.DroneGunShotAccuracy = 97f;
        this.DroneGunShotSpeed = 150f;
        this.DroneGunShotLifetime = 2f;
        this.DroneTargetAcquisitionDistance = 60f;
        this.DroneStrafeDistance = 10f;
        this.DroneLeashDistance = 80f;
        // ----Ability 3: EMP
        this.EMPRadius = 60f;
        this.EMPDuration = 5f;
        this.EMPEnergyCost = 50f;
        // ---- Ability Cooldowns
        this.Ability1Duration = 5f;
        this.Ability1CooldownTime = 10f;
        this.Ability2Duration = 0f;
        this.Ability2CooldownTime = 15f;
        this.Ability3Duration = 1f;
        this.Ability3CooldownTime = 14f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        // Ship type specific objects
        this.ShieldOverchargeObject = this.ShipObject.transform.GetChild(0).Find(GameController.ShieldOverchargeObjectName).gameObject;
        this.ShieldOverchargeObject.SetActive(false);
        this.EMPObject = this.ShipObject.transform.GetChild(0).Find(GameController.EMPObjectName).gameObject;
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
        this.CheckShieldOvercharge();
    }

    // Check ability 2
    public override void CheckAbility2()
    {
        this.CheckDrones();
    }

    // Check ability 3
    public override void CheckAbility3()
    {
        this.CheckEMP();
    }

    // Check Shield Overcharge
    private void CheckShieldOvercharge()
    {
        // If ability 1 input activated and ability 1 is not currently on cooldown and ability 1 is not currently active
        if(this.Ability1Input == true && this.Ability1OnCooldown == false && this.Ability1Active == false)
        {
            // Set ability 1 to active, record ability 1 last activated time
            this.ShieldOverchargeObject.SetActive(true);
            this.ShieldRegenSpeed *= this.ShieldRegenSpeedMultiplier;
            this.ShieldCooldownTime *= this.ShieldCooldownMultiplier;
            this.Ability1Active = true;
            this.LastAbility1ActivatedTime = Time.time;
        }
        // If difference between current time and ability 1 last activated time is greater than ability 1 duration
        if(this.Ability1Active == true && Time.time - this.LastAbility1ActivatedTime > this.Ability1Duration)
        {
            // Set ability 1 to not active, set ability 1 to on cooldown, and record time cooldown started
            this.ShieldOverchargeObject.SetActive(false);
            this.ShieldRegenSpeed = this.DefaultShieldRegenSpeed;
            this.ShieldCooldownTime = this.DefaultShieldCooldownTime;
            this.Ability1Active = false;
            this.Ability1OnCooldown = true;
            this.LastAbility1CooldownStartedTime = Time.time;
        }
        // If difference between current time and ability 1 started cooldown time is greater than ability 1 cooldown time
        if(this.Ability1OnCooldown == true && Time.time - this.LastAbility1CooldownStartedTime > this.Ability1CooldownTime)
        {
            // Take ability 1 off cooldown
            this.Ability1OnCooldown = false;
        }
    }

    // Check Drone
    private void CheckDrones()
    {
        // If ability 2 input activated and ability 2 is not currently on cooldown and ability 2 is not currently active
        if(this.Ability2Input == true && this.Ability2OnCooldown == false && this.Ability2Active == false && this.Drones.Count < this.MaxDroneAmount)
        {
            for(int i = 0; i < this.DroneAmount; i++)
            {
                if(this.Drones.Count < this.MaxDroneAmount)
                {
                    Vector3 DroneSpawnPosition = new Vector3(this.ShipObject.transform.position.x + GameController.r.Next(-5, 6), 0, this.ShipObject.transform.position.z + GameController.r.Next(-5, 6));
                    DroneShip drone = GameController.SpawnDrone(this, this.DroneType, DroneSpawnPosition, this.DroneMaxHealth, this.DroneMaxShields, this.DroneMaxSpeed, this.DroneGunShotProjectileType, this.DroneGunCooldownTime, this.DroneGunShotAmount, this.DroneGunShotDamage, this.DroneGunShotAccuracy, this.DroneGunShotSpeed, this.DroneGunShotLifetime, this.DroneTargetAcquisitionDistance, this.DroneStrafeDistance, this.DroneLeashDistance);
                    this.Drones.Add(drone);
                }
            }
            this.Ability2Active = true;
            this.LastAbility2ActivatedTime = Time.time;
        }
        // If difference between current time and ability 2 last activated time is greater than ability 2 duration
        if(this.Ability2Active == true && Time.time - this.LastAbility2ActivatedTime > this.Ability2Duration)
        {
            this.Ability2Active = false;
            this.Ability2OnCooldown = true;
            this.LastAbility2CooldownStartedTime = Time.time;
        }
        // If difference between current time and ability 2 started cooldown time is greater than ability 2 cooldown time
        if(this.Ability2OnCooldown == true && Time.time - this.LastAbility2CooldownStartedTime > this.Ability2CooldownTime)
        {
            // Take ability 2 off cooldown
            this.Ability2OnCooldown = false;
        }
    }

    // Check EMP
    private void CheckEMP()
    {
        // If ability 3 input activated and ability 3 is not currently on cooldown and ability 3 is not currently active
        if(this.Ability3Input == true && this.Ability3OnCooldown == false && this.Ability3Active == false)
        {
            // Set ability 3 to active, record ability 3 last activated time
            foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
            {
                if(ship.Value.IFF != this.IFF && Vector3.Distance(this.ShipObject.transform.position, ship.Value.ShipObject.transform.position) <= this.EMPRadius)
                {
                    ship.Value.IsEMPed = true;
                    ship.Value.EMPEffectDuration = this.EMPDuration;
                }
            }
            this.Energy -= this.EMPEnergyCost;
            this.EMPObject.SetActive(true);
            this.Ability3Active = true;
            this.LastAbility3ActivatedTime = Time.time;
        }
        // If difference between current time and ability 3 last activated time is greater than ability 3 duration
        if(this.Ability3Active == true && Time.time - this.LastAbility3ActivatedTime > this.Ability3Duration)
        {
            // Set ability 3 to not active, set ability 3 to on cooldown, and record time cooldown started
            this.EMPObject.SetActive(false);
            this.Ability3Active = false;
            this.Ability3OnCooldown = true;
            this.LastAbility3CooldownStartedTime = Time.time;
        }
        // If difference between current time and ability 1 started cooldown time is greater than ability 1 cooldown time
        if(this.Ability3OnCooldown == true && Time.time - this.LastAbility3CooldownStartedTime > this.Ability3CooldownTime)
        {
            // Take ability 1 off cooldown
            this.Ability3OnCooldown = false;
        }
    }
}
