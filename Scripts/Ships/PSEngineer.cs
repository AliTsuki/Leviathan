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
        this.Ability1Duration = 4f;
        this.Ability1CooldownTime = 12f;
        this.Ability2Duration = 0f;
        this.Ability2CooldownTime = 20f;
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
            // Activate shield overcharge gameobject
            this.ShieldOverchargeObject.SetActive(true);
            // Multiply shield regen speed and cooldown time by multipliers
            this.ShieldRegenSpeed *= this.ShieldRegenSpeedMultiplier;
            this.ShieldCooldownTime *= this.ShieldCooldownMultiplier;
            // Set ability one active
            this.Ability1Active = true;
            // Record ability 1 activated time
            this.LastAbility1ActivatedTime = Time.time;
        }
        // If difference between current time and ability 1 last activated time is greater than ability 1 duration
        if(this.Ability1Active == true && Time.time - this.LastAbility1ActivatedTime > this.Ability1Duration)
        {
            // Deactivate shield overcharge gameobject
            this.ShieldOverchargeObject.SetActive(false);
            // Set shield regen speed and cooldown back to defaults
            this.ShieldRegenSpeed = this.DefaultShieldRegenSpeed;
            this.ShieldCooldownTime = this.DefaultShieldCooldownTime;
            // Set ability one to inactive
            this.Ability1Active = false;
            // Set ability one to on cooldown
            this.Ability1OnCooldown = true;
            // Record cooldown started time
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
        // If ability 2 input activated and ability 2 is not currently on cooldown and ability 2 is not currently active and drones are less than max amount
        if(this.Ability2Input == true && this.Ability2OnCooldown == false && this.Ability2Active == false && this.Drones.Count < this.MaxDroneAmount)
        {
            // Loop through drone summon amount
            for(int i = 0; i < this.DroneAmount; i++)
            {
                // If current drone count is less than max
                if(this.Drones.Count < this.MaxDroneAmount)
                {
                    // Get a new drone spawn position
                    Vector3 DroneSpawnPosition = new Vector3(this.ShipObject.transform.position.x + GameController.r.Next(-5, 6), 0, this.ShipObject.transform.position.z + GameController.r.Next(-5, 6));
                    // Create a new drone at position
                    DroneShip drone = GameController.SpawnDrone(this, this.DroneType, DroneSpawnPosition, this.DroneMaxHealth, this.DroneMaxShields, this.DroneMaxSpeed, this.DroneGunShotProjectileType, this.DroneGunCooldownTime, this.DroneGunShotAmount, this.DroneGunShotDamage, this.DroneGunShotAccuracy, this.DroneGunShotSpeed, this.DroneGunShotLifetime, this.DroneTargetAcquisitionDistance, this.DroneStrafeDistance, this.DroneLeashDistance);
                    // Add drone to list
                    this.Drones.Add(drone);
                }
            }
            // Set ability 2 to active
            this.Ability2Active = true;
            // Record ability 2 activated time
            this.LastAbility2ActivatedTime = Time.time;
        }
        // If difference between current time and ability 2 last activated time is greater than ability 2 duration
        if(this.Ability2Active == true && Time.time - this.LastAbility2ActivatedTime > this.Ability2Duration)
        {
            // Set ability 2 to inactive
            this.Ability2Active = false;
            // Set ability 2 on cooldown
            this.Ability2OnCooldown = true;
            // Record cooldown started time
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
            // Loop through all ships
            foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
            {
                // If ship is enemy and distance to ship is less than or equal to EMP radius
                if(ship.Value.IFF != this.IFF && Vector3.Distance(this.ShipObject.transform.position, ship.Value.ShipObject.transform.position) <= this.EMPRadius)
                {
                    // Set ship to is EMPed
                    ship.Value.IsEMPed = true;
                    // Set EMP duration on ship
                    ship.Value.EMPEffectDuration = this.EMPDuration;
                }
            }
            // Subtract energy cost of EMP
            this.Energy -= this.EMPEnergyCost;
            // Activate EMP gameobject
            this.EMPObject.SetActive(true);
            // Set ability 3 active
            this.Ability3Active = true;
            // Record ability 3 activated time
            this.LastAbility3ActivatedTime = Time.time;
        }
        // If difference between current time and ability 3 last activated time is greater than ability 3 duration
        if(this.Ability3Active == true && Time.time - this.LastAbility3ActivatedTime > this.Ability3Duration)
        {
            // Set EMP object inactive
            this.EMPObject.SetActive(false);
            // Set ability 3 inactive
            this.Ability3Active = false;
            // Set ability 3 on cooldown
            this.Ability3OnCooldown = true;
            // Record cooldown started time
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
