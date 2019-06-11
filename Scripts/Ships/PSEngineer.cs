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
        if(this.AbilityInput[0] == true && this.AbilityOnCooldown[0] == false && this.AbilityActive[0] == false)
        {
            // Activate shield overcharge gameobject
            this.ShieldOverchargeObject.SetActive(true);
            // Multiply shield regen speed and cooldown time by multipliers
            this.ShieldRegenSpeed *= this.ShieldRegenSpeedMultiplier;
            this.ShieldCooldownTime *= this.ShieldCooldownMultiplier;
            // Set ability one active
            this.AbilityActive[0] = true;
            // Record ability 1 activated time
            this.LastAbilityActivatedTime[0] = Time.time;
        }
        // If difference between current time and ability 1 last activated time is greater than ability 1 duration
        if(this.AbilityActive[0] == true && Time.time - this.LastAbilityActivatedTime[0] > this.AbilityDuration[0])
        {
            // Deactivate shield overcharge gameobject
            this.ShieldOverchargeObject.SetActive(false);
            // Set shield regen speed and cooldown back to defaults
            this.ShieldRegenSpeed = this.DefaultShieldRegenSpeed;
            this.ShieldCooldownTime = this.DefaultShieldCooldownTime;
            // Set ability one to inactive
            this.AbilityActive[0] = false;
            // Set ability one to on cooldown
            this.AbilityOnCooldown[0] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[0] = Time.time;
        }
        // If difference between current time and ability 1 started cooldown time is greater than ability 1 cooldown time
        if(this.AbilityOnCooldown[0] == true && Time.time - this.LastAbilityCooldownStartedTime[0] > this.AbilityCooldownTime[0])
        {
            // Take ability 1 off cooldown
            this.AbilityOnCooldown[0] = false;
        }
    }

    // Check Drone
    private void CheckDrones()
    {
        // If ability 2 input activated and ability 2 is not currently on cooldown and ability 2 is not currently active and drones are less than max amount
        if(this.AbilityInput[1] == true && this.AbilityOnCooldown[1] == false && this.AbilityActive[1] == false)
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
                // If Drones are maxed out
                else
                {
                    // Detonate oldest drone
                    this.Drones[0].Detonate();
                    // Get a new drone spawn position
                    Vector3 DroneSpawnPosition = new Vector3(this.ShipObject.transform.position.x + GameController.r.Next(-5, 6), 0, this.ShipObject.transform.position.z + GameController.r.Next(-5, 6));
                    // Create a new drone at position
                    DroneShip drone = GameController.SpawnDrone(this, this.DroneType, DroneSpawnPosition, this.DroneMaxHealth, this.DroneMaxShields, this.DroneMaxSpeed, this.DroneGunShotProjectileType, this.DroneGunCooldownTime, this.DroneGunShotAmount, this.DroneGunShotDamage, this.DroneGunShotAccuracy, this.DroneGunShotSpeed, this.DroneGunShotLifetime, this.DroneTargetAcquisitionDistance, this.DroneStrafeDistance, this.DroneLeashDistance);
                    // Add drone to list
                    this.Drones.Add(drone);
                }
            }
            // Set ability 2 to active
            this.AbilityActive[1] = true;
            // Record ability 2 activated time
            this.LastAbilityActivatedTime[1] = Time.time;
        }
        // If difference between current time and ability 2 last activated time is greater than ability 2 duration
        if(this.AbilityActive[1] == true && Time.time - this.LastAbilityActivatedTime[1] > this.AbilityDuration[1])
        {
            // Set ability 2 to inactive
            this.AbilityActive[1] = false;
            // Set ability 2 on cooldown
            this.AbilityOnCooldown[1] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[1] = Time.time;
        }
        // If difference between current time and ability 2 started cooldown time is greater than ability 2 cooldown time
        if(this.AbilityOnCooldown[1] == true && Time.time - this.LastAbilityCooldownStartedTime[1] > this.AbilityCooldownTime[1])
        {
            // Take ability 2 off cooldown
            this.AbilityOnCooldown[1] = false;
        }
    }

    // Check EMP
    private void CheckEMP()
    {
        // If ability 3 input activated and ability 3 is not currently on cooldown and ability 3 is not currently active
        if(this.AbilityInput[2] == true && this.AbilityOnCooldown[2] == false && this.AbilityActive[2] == false)
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
            this.AbilityActive[2] = true;
            // Record ability 3 activated time
            this.LastAbilityActivatedTime[2] = Time.time;
        }
        // If difference between current time and ability 3 last activated time is greater than ability 3 duration
        if(this.AbilityActive[2] == true && Time.time - this.LastAbilityActivatedTime[2] > this.AbilityDuration[2])
        {
            // Set EMP object inactive
            this.EMPObject.SetActive(false);
            // Set ability 3 inactive
            this.AbilityActive[2] = false;
            // Set ability 3 on cooldown
            this.AbilityOnCooldown[2] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[2] = Time.time;
        }
        // If difference between current time and ability 1 started cooldown time is greater than ability 1 cooldown time
        if(this.AbilityOnCooldown[2] == true && Time.time - this.LastAbilityCooldownStartedTime[2] > this.AbilityCooldownTime[2])
        {
            // Take ability 1 off cooldown
            this.AbilityOnCooldown[2] = false;
        }
    }
}
