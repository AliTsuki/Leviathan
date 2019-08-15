using System.Collections.Generic;

using UnityEngine;

// Contains all ship abilities
public abstract partial class Ship
{
    // Check Shield Overcharge
    public void CheckShieldOvercharge(byte abilityID, GameObject shieldOverchargeObject, float shieldRegenSpeedMultiplier, 
        float shieldCooldownMultiplier)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && this.AbilityActive[abilityID] == false)
        {
            // Activate shield overcharge gameobject
            shieldOverchargeObject.SetActive(true);
            // Multiply shield regen speed and cooldown time by multipliers
            this.Stats.ShieldRegenSpeed *= shieldRegenSpeedMultiplier;
            this.Stats.ShieldCooldownTime *= shieldCooldownMultiplier;
            // Set ability active
            this.AbilityActive[abilityID] = true;
            // Record ability activated time
            this.LastAbilityActivatedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(this.AbilityActive[abilityID] == true && Time.time - this.LastAbilityActivatedTime[abilityID] > this.Stats.AbilityDuration[abilityID])
        {
            // Deactivate shield overcharge gameobject
            shieldOverchargeObject.SetActive(false);
            // Set shield regen speed and cooldown back to defaults
            this.Stats.ShieldRegenSpeed = this.DefaultShieldRegenSpeed;
            this.Stats.ShieldCooldownTime = this.DefaultShieldCooldownTime;
            // Set ability to inactive
            this.AbilityActive[abilityID] = false;
            // Set ability to on cooldown
            this.AbilityOnCooldown[abilityID] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take ability off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }

    // Check Drone
    public void CheckDrones(byte abilityID, List<DroneShip> drones, uint droneAmount, uint maxDroneAmount, DroneShip.DroneShipType droneType, 
        float droneMaxHealth, float droneMaxShields, float droneMaxSpeed, uint droneGunShotProjectileType, float droneGunCooldownTime, uint droneGunShotAmount, 
        float droneGunShotDamage, float droneGunShotAccuracy, float droneGunShotSpeed, float droneGunShotLifetime, float droneTargetAcquisitionDistance, 
        float droneStrafeDistance, float droneLeashDistance)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active and drones are less than max amount
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && this.AbilityActive[abilityID] == false)
        {
            // Loop through drone summon amount
            for(int i = 0; i < droneAmount; i++)
            {
                // If current drone count is less than max
                if(drones.Count < maxDroneAmount)
                {
                    // Get a new drone spawn position
                    Vector3 DroneSpawnPosition = new Vector3(this.ShipObject.transform.position.x + GameController.RandomNumGen.Next(-5, 6), 0, 
                        this.ShipObject.transform.position.z + GameController.RandomNumGen.Next(-5, 6));
                    // Create a new drone at position
                    DroneShip drone = GameController.SpawnDrone(this, drones, droneType, DroneSpawnPosition, droneMaxHealth, droneMaxShields, 
                        droneMaxSpeed, droneGunShotProjectileType, droneGunCooldownTime, droneGunShotAmount, droneGunShotDamage, droneGunShotAccuracy, 
                        droneGunShotSpeed, droneGunShotLifetime, droneTargetAcquisitionDistance, droneStrafeDistance, droneLeashDistance);
                    // Add drone to list
                    drones.Add(drone);
                }
                // If Drones are maxed out
                else
                {
                    // Detonate oldest drone
                    drones[abilityID].Detonate();
                    // Get a new drone spawn position
                    Vector3 DroneSpawnPosition = new Vector3(this.ShipObject.transform.position.x + GameController.RandomNumGen.Next(-5, 6), 0, 
                        this.ShipObject.transform.position.z + GameController.RandomNumGen.Next(-5, 6));
                    // Create a new drone at position
                    DroneShip drone = GameController.SpawnDrone(this, drones, droneType, DroneSpawnPosition, droneMaxHealth, droneMaxShields, 
                        droneMaxSpeed, droneGunShotProjectileType, droneGunCooldownTime, droneGunShotAmount, droneGunShotDamage, droneGunShotAccuracy, 
                        droneGunShotSpeed, droneGunShotLifetime, droneTargetAcquisitionDistance, droneStrafeDistance, droneLeashDistance);
                    // Add drone to list
                    drones.Add(drone);
                }
            }
            // Set ability to active
            this.AbilityActive[abilityID] = true;
            // Record ability activated time
            this.LastAbilityActivatedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(this.AbilityActive[abilityID] == true && Time.time - this.LastAbilityActivatedTime[abilityID] > this.Stats.AbilityDuration[abilityID])
        {
            // Set ability to inactive
            this.AbilityActive[abilityID] = false;
            // Set ability on cooldown
            this.AbilityOnCooldown[abilityID] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take ability off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }

    // Check EMP
    public void CheckEMP(byte abilityID, GameObject EMPObject, float EMPRadius, float EMPDuration, float EMPEnergyCost)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && this.AbilityActive[abilityID] == false)
        {
            // Loop through all ships
            foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
            {
                // If ship is enemy and distance to ship is less than or equal to EMP radius
                if(ship.Value.IFF != this.IFF && Vector3.Distance(this.ShipObject.transform.position, ship.Value.ShipObject.transform.position) <= EMPRadius)
                {
                    // Set ship to is EMPed
                    ship.Value.IsEMPed = true;
                    // Set EMP duration on ship
                    ship.Value.EMPEffectDuration = EMPDuration;
                }
            }
            // Subtract energy cost of EMP
            this.SubtractEnergy(EMPEnergyCost);
            // Activate EMP gameobject
            EMPObject.SetActive(true);
            EMPObject.GetComponent<ParticleSystem>().Play();
            // Set ability active
            this.AbilityActive[2] = true;
            // Record ability activated time
            this.LastAbilityActivatedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(this.AbilityActive[abilityID] == true && Time.time - this.LastAbilityActivatedTime[abilityID] > this.Stats.AbilityDuration[abilityID])
        {
            // Set EMP object inactive
            EMPObject.SetActive(false);
            // Set ability inactive
            this.AbilityActive[abilityID] = false;
            // Set ability on cooldown
            this.AbilityOnCooldown[abilityID] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take ability off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }

    // Check barrier
    public void CheckBarrier(byte abilityID, GameObject barrierObject)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && this.AbilityActive[abilityID] == false)
        {
            // Activate barrier object, set barrier active to true, and record time barrier was activated
            barrierObject.SetActive(true);
            this.AbilityActive[abilityID] = true;
            this.LastAbilityActivatedTime[abilityID] = Time.time;
        }
        // If difference between current time and shield last activated time is greater than barrier duration
        else if(this.AbilityActive[abilityID] == true && Time.time - this.LastAbilityActivatedTime[abilityID] > this.Stats.AbilityDuration[abilityID])
        {
            // Disable barrier object, set barrier to not active, set barrier to on cooldown, and record time cooldown started
            barrierObject.SetActive(false);
            this.AbilityActive[abilityID] = false;
            this.AbilityOnCooldown[abilityID] = true;
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
        }
        // If difference between current time and barrier started cooldown time is greater than barrier cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take barrier off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }

    // Check barrage
    public void CheckBarrage(byte abilityID, float barrageGunCooldownTimeMultiplier, uint barrageShotAmountIncrease, float barrageDamageMultiplier,
        float barrageAccuracyMultiplier, float barrageEnergyCostMultiplier)
    {
        // If barrage input is active, barrage is not on cooldown, and barrage is not currently active
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && this.AbilityActive[abilityID] == false)
        {
            // Set barrage to active
            this.AbilityActive[abilityID] = true;
            // Record last barrage activated time
            this.LastAbilityActivatedTime[abilityID] = Time.time;
            // Apply barrage multipliers
            this.Stats.GunCooldownTime *= barrageGunCooldownTimeMultiplier;
            this.Stats.GunShotAmount += barrageShotAmountIncrease;
            this.Stats.GunShotDamage *= barrageDamageMultiplier;
            this.Stats.GunShotAccuracy *= barrageAccuracyMultiplier;
            this.Stats.GunEnergyCost *= barrageEnergyCostMultiplier;
        }
        // If barrage is currently active and last barrage activated time is greater than barrage duration
        else if(this.AbilityActive[abilityID] == true && Time.time - this.LastAbilityActivatedTime[abilityID] > this.Stats.AbilityDuration[abilityID])
        {
            // Set barrage to inactive
            this.AbilityActive[abilityID] = false;
            // Set barrage on cooldown
            this.AbilityOnCooldown[abilityID] = true;
            // Record barrage cooldown started time
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
            // Remove barrage multipliers
            this.Stats.GunCooldownTime = this.DefaultGunCooldownTime;
            this.Stats.GunShotAmount = this.DefaultGunShotAmount;
            this.Stats.GunShotDamage = this.DefaultGunShotDamage;
            this.Stats.GunShotAccuracy = this.DefaultGunShotAccuracy;
            this.Stats.GunEnergyCost = this.DefaultGunEnergyCost;
        }
        // If barrage is on cooldown and last barrage cooldown started time is greater than barrage cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take barrage off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }

    // Check bomb
    public void CheckBomb(byte abilityID, ref Bomb bomb, float bombDamage, float bombRadius, float bombPrimerTime, float bombSpeed, float bombLifetime)
    {
        // If bomb input is active, bomb is not on cooldown, and there is no bomb in flight
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && bomb == null)
        {
            // TODO: Now that a ship can have more than one main gun, need to think of something better than just having bomb use the 0th gun barrel location/rotation. IDEA: make a game object child of ship to use as bomb firing location
            // Spawn a bomb
            bomb = GameController.SpawnBomb(this, this.IFF, bombDamage, bombRadius, this.GunBarrelObjects[0].transform.position, 
                this.GunBarrelObjects[0].transform.rotation, this.ShipRigidbody.velocity, bombSpeed, bombLifetime);
            // Set bomb on cooldown
            this.AbilityOnCooldown[abilityID] = true;
            // Record bomb activated time
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
        }
        // If bomb is in flight, bomb input is pressed, and time since bomb was activated is more than the bomb primer time
        else if(bomb != null && this.AbilityInput[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > bombPrimerTime)
        {
            // Detonate bomb
            bomb.Detonate();
            bomb = null;
        }
        else if(bomb != null && bomb.Alive == false)
        {
            bomb = null;
        }
        // If bomb is on cooldown and time since bomb was last activated is greater than bomb cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take bomb off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }

    // Self destruct ship
    public void Detonate(GameObject bombExplosionPrefab, Vector3 scale, float bombDamage, float bombRadius)
    {
        // Spawn explosion object
        GameObject BombExplosionObject = GameObject.Instantiate(bombExplosionPrefab, this.ShipObject.transform.position, Quaternion.identity);
        BombExplosionObject.transform.localScale = scale;
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(BombExplosionObject, 1);
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship is other faction and currently alive
            if(ship.Value.IFF != this.IFF && ship.Value.Alive == true)
            {
                // Get distance from ship
                float distance = Vector3.Distance(this.ShipObject.transform.position, ship.Value.ShipObject.transform.position);
                // If distance is less than radius
                if(distance <= bombRadius)
                {
                    // Tell ship to take damage relative to it's distance
                    ship.Value.TakeDamage(bombDamage - (distance / bombRadius * bombDamage));
                }
            }
        }
        // Ship dies in attack
        this.Kill();
    }

    // Check Flank Teleport
    public void CheckFlankTeleport(byte abilityID, GameObject flankTeleportPrefab, float flankTeleportRange)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(this.AbilityInput[abilityID] == true && this.AbilityOnCooldown[abilityID] == false && this.AbilityActive[abilityID] == false && this.CurrentTarget != null)
        {
            flankTeleportPrefab.SetActive(true);
            this.ShipObject.transform.position = this.CurrentTarget.ShipObject.transform.position + (this.CurrentTarget.ShipObject.transform.forward * -flankTeleportRange);
            // Set ability active
            this.AbilityActive[abilityID] = true;
            // Record ability activated time
            this.LastAbilityActivatedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(this.AbilityActive[abilityID] == true && Time.time - this.LastAbilityActivatedTime[abilityID] > this.Stats.AbilityDuration[abilityID])
        {
            flankTeleportPrefab.SetActive(false);
            // Set ability to inactive
            this.AbilityActive[abilityID] = false;
            // Set ability to on cooldown
            this.AbilityOnCooldown[abilityID] = true;
            // Record cooldown started time
            this.LastAbilityCooldownStartedTime[abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(this.AbilityOnCooldown[abilityID] == true && Time.time - this.LastAbilityCooldownStartedTime[abilityID] > this.Stats.AbilityCooldownTime[abilityID])
        {
            // Take ability off cooldown
            this.AbilityOnCooldown[abilityID] = false;
        }
    }
}
