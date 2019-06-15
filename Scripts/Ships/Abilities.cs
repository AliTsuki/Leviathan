using System.Collections.Generic;

using UnityEngine;

// Contains all ship abilities
public static class Abilities
{
    // Check Shield Overcharge
    public static void CheckShieldOvercharge(Ship _ship, byte _abilityID, GameObject _shieldOverchargeObject, float _shieldRegenSpeedMultiplier, float _shieldCooldownMultiplier)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _ship.AbilityActive[_abilityID] == false)
        {
            // Activate shield overcharge gameobject
            _shieldOverchargeObject.SetActive(true);
            // Multiply shield regen speed and cooldown time by multipliers
            _ship.ShieldRegenSpeed *= _shieldRegenSpeedMultiplier;
            _ship.ShieldCooldownTime *= _shieldCooldownMultiplier;
            // Set ability active
            _ship.AbilityActive[_abilityID] = true;
            // Record ability activated time
            _ship.LastAbilityActivatedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(_ship.AbilityActive[_abilityID] == true && Time.time - _ship.LastAbilityActivatedTime[_abilityID] > _ship.AbilityDuration[_abilityID])
        {
            // Deactivate shield overcharge gameobject
            _shieldOverchargeObject.SetActive(false);
            // Set shield regen speed and cooldown back to defaults
            _ship.ShieldRegenSpeed = _ship.DefaultShieldRegenSpeed;
            _ship.ShieldCooldownTime = _ship.DefaultShieldCooldownTime;
            // Set ability to inactive
            _ship.AbilityActive[_abilityID] = false;
            // Set ability to on cooldown
            _ship.AbilityOnCooldown[_abilityID] = true;
            // Record cooldown started time
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take ability off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
    }

    // Check Drone
    public static void CheckDrones(Ship _ship, byte _abilityID, List<DroneShip> _drones, uint _droneAmount, uint _maxDroneAmount, DroneShip.DroneShipType _droneType, float _droneMaxHealth, float _droneMaxShields, float _droneMaxSpeed, uint _droneGunShotProjectileType, float _droneGunCooldownTime, uint _droneGunShotAmount, float _droneGunShotDamage, float _droneGunShotAccuracy, float _droneGunShotSpeed, float _droneGunShotLifetime, float _droneTargetAcquisitionDistance, float _droneStrafeDistance, float _droneLeashDistance)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active and drones are less than max amount
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _ship.AbilityActive[_abilityID] == false)
        {
            // Loop through drone summon amount
            for(int i = 0; i < _droneAmount; i++)
            {
                // If current drone count is less than max
                if(_drones.Count < _maxDroneAmount)
                {
                    // Get a new drone spawn position
                    Vector3 DroneSpawnPosition = new Vector3(_ship.ShipObject.transform.position.x + GameController.r.Next(-5, 6), 0, _ship.ShipObject.transform.position.z + GameController.r.Next(-5, 6));
                    // Create a new drone at position
                    DroneShip drone = GameController.SpawnDrone(_ship, _drones, _droneType, DroneSpawnPosition, _droneMaxHealth, _droneMaxShields, _droneMaxSpeed, _droneGunShotProjectileType, _droneGunCooldownTime, _droneGunShotAmount, _droneGunShotDamage, _droneGunShotAccuracy, _droneGunShotSpeed, _droneGunShotLifetime, _droneTargetAcquisitionDistance, _droneStrafeDistance, _droneLeashDistance);
                    // Add drone to list
                    _drones.Add(drone);
                }
                // If Drones are maxed out
                else
                {
                    // Detonate oldest drone
                    _drones[_abilityID].Detonate();
                    // Get a new drone spawn position
                    Vector3 DroneSpawnPosition = new Vector3(_ship.ShipObject.transform.position.x + GameController.r.Next(-5, 6), 0, _ship.ShipObject.transform.position.z + GameController.r.Next(-5, 6));
                    // Create a new drone at position
                    DroneShip drone = GameController.SpawnDrone(_ship, _drones, _droneType, DroneSpawnPosition, _droneMaxHealth, _droneMaxShields, _droneMaxSpeed, _droneGunShotProjectileType, _droneGunCooldownTime, _droneGunShotAmount, _droneGunShotDamage, _droneGunShotAccuracy, _droneGunShotSpeed, _droneGunShotLifetime, _droneTargetAcquisitionDistance, _droneStrafeDistance, _droneLeashDistance);
                    // Add drone to list
                    _drones.Add(drone);
                }
            }
            // Set ability to active
            _ship.AbilityActive[_abilityID] = true;
            // Record ability activated time
            _ship.LastAbilityActivatedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(_ship.AbilityActive[_abilityID] == true && Time.time - _ship.LastAbilityActivatedTime[_abilityID] > _ship.AbilityDuration[_abilityID])
        {
            // Set ability to inactive
            _ship.AbilityActive[_abilityID] = false;
            // Set ability on cooldown
            _ship.AbilityOnCooldown[_abilityID] = true;
            // Record cooldown started time
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take ability off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
    }

    // Check EMP
    public static void CheckEMP(Ship _ship, byte _abilityID, GameObject _EMPObject, float _EMPRadius, float _EMPDuration, float _EMPEnergyCost)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _ship.AbilityActive[_abilityID] == false)
        {
            // Loop through all ships
            foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
            {
                // If ship is enemy and distance to ship is less than or equal to EMP radius
                if(ship.Value.IFF != _ship.IFF && Vector3.Distance(_ship.ShipObject.transform.position, ship.Value.ShipObject.transform.position) <= _EMPRadius)
                {
                    // Set ship to is EMPed
                    ship.Value.IsEMPed = true;
                    // Set EMP duration on ship
                    ship.Value.EMPEffectDuration = _EMPDuration;
                }
            }
            // Subtract energy cost of EMP
            _ship.Energy -= _EMPEnergyCost;
            // Activate EMP gameobject
            _EMPObject.SetActive(true);
            // Set ability active
            _ship.AbilityActive[2] = true;
            // Record ability activated time
            _ship.LastAbilityActivatedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(_ship.AbilityActive[_abilityID] == true && Time.time - _ship.LastAbilityActivatedTime[_abilityID] > _ship.AbilityDuration[_abilityID])
        {
            // Set EMP object inactive
            _EMPObject.SetActive(false);
            // Set ability inactive
            _ship.AbilityActive[_abilityID] = false;
            // Set ability on cooldown
            _ship.AbilityOnCooldown[_abilityID] = true;
            // Record cooldown started time
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take ability off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
    }

    // Check barrier
    public static void CheckBarrier(Ship _ship, byte _abilityID, GameObject _barrierObject)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _ship.AbilityActive[_abilityID] == false)
        {
            // Activate barrier object, set barrier active to true, and record time barrier was activated
            _barrierObject.SetActive(true);
            _ship.AbilityActive[_abilityID] = true;
            _ship.LastAbilityActivatedTime[_abilityID] = Time.time;
        }
        // If difference between current time and shield last activated time is greater than barrier duration
        else if(_ship.AbilityActive[_abilityID] == true && Time.time - _ship.LastAbilityActivatedTime[_abilityID] > _ship.AbilityDuration[_abilityID])
        {
            // Disable barrier object, set barrier to not active, set barrier to on cooldown, and record time cooldown started
            _barrierObject.SetActive(false);
            _ship.AbilityActive[_abilityID] = false;
            _ship.AbilityOnCooldown[_abilityID] = true;
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
        }
        // If difference between current time and barrier started cooldown time is greater than barrier cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take barrier off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
    }

    // Check barrage
    public static void CheckBarrage(Ship _ship, byte _abilityID, float _barrageGunCooldownTimeMultiplier, uint _barrageShotAmountIncrease, float _barrageDamageMultiplier, float _barrageAccuracyMultiplier, float _barrageEnergyCostMultiplier)
    {
        // If barrage input is active, barrage is not on cooldown, and barrage is not currently active
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _ship.AbilityActive[_abilityID] == false)
        {
            // Set barrage to active
            _ship.AbilityActive[_abilityID] = true;
            // Record last barrage activated time
            _ship.LastAbilityActivatedTime[_abilityID] = Time.time;
            // Apply barrage multipliers
            _ship.GunCooldownTime *= _barrageGunCooldownTimeMultiplier;
            _ship.GunShotAmount += _barrageShotAmountIncrease;
            _ship.GunShotDamage *= _barrageDamageMultiplier;
            _ship.GunShotAccuracy *= _barrageAccuracyMultiplier;
            _ship.GunEnergyCost *= _barrageEnergyCostMultiplier;
        }
        // If barrage is currently active and last barrage activated time is greater than barrage duration
        else if(_ship.AbilityActive[_abilityID] == true && Time.time - _ship.LastAbilityActivatedTime[_abilityID] > _ship.AbilityDuration[_abilityID])
        {
            // Set barrage to inactive
            _ship.AbilityActive[_abilityID] = false;
            // Set barrage on cooldown
            _ship.AbilityOnCooldown[_abilityID] = true;
            // Record barrage cooldown started time
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
            // Remove barrage multipliers
            _ship.GunCooldownTime = _ship.DefaultGunCooldownTime;
            _ship.GunShotAmount = _ship.DefaultGunShotAmount;
            _ship.GunShotDamage = _ship.DefaultGunShotDamage;
            _ship.GunShotAccuracy = _ship.DefaultGunShotAccuracy;
            _ship.GunEnergyCost = _ship.DefaultGunEnergyCost;
        }
        // If barrage is on cooldown and last barrage cooldown started time is greater than barrage cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take barrage off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
    }

    // Check bomb
    public static Bomb CheckBomb(Ship _ship, byte _abilityID, Bomb _bomb, float _bombDamage, float _bombRadius, float _bombPrimerTime, float _bombSpeed, float _bombLifetime)
    {
        Bomb bomb = _bomb;
        // If bomb input is active, bomb is not on cooldown, and there is no bomb in flight
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _bomb == null)
        {
            // TODO: Now that a ship can have more than one main gun, need to think of something better than just having bomb use the 0th gun barrel location/rotation. IDEA: make a game object child of ship to use as bomb firing location
            // Spawn a bomb
            bomb = GameController.SpawnBomb(_ship, _ship.IFF, _bombDamage, _bombRadius, _ship.GunBarrelObjects[0].transform.position, _ship.GunBarrelObjects[0].transform.rotation, _ship.ShipRigidbody.velocity, _bombSpeed, _bombLifetime);
            // Set bomb on cooldown
            _ship.AbilityOnCooldown[_abilityID] = true;
            // Record bomb activated time
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
        }
        // If bomb is in flight, bomb input is pressed, and time since bomb was activated is more than the bomb primer time
        else if(_bomb != null && _ship.AbilityInput[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _bombPrimerTime)
        {
            // Detonate bomb
            _bomb.Detonate();
            bomb = null;
        }
        else if(_bomb != null && _bomb.Alive == false)
        {
            bomb = null;
        }
        // If bomb is on cooldown and time since bomb was last activated is greater than bomb cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take bomb off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
        return bomb;
    }

    // Self destruct ship
    public static void Detonate(Ship _ship, GameObject _bombExplosionPrefab, Vector3 _scale, float _bombDamage, float _bombRadius)
    {
        // Spawn explosion object
        GameObject BombExplosionObject = GameObject.Instantiate(_bombExplosionPrefab, _ship.ShipObject.transform.position, Quaternion.identity);
        BombExplosionObject.transform.localScale = _scale;
        // Set explosion object to self destroy after 1 second
        GameObject.Destroy(BombExplosionObject, 1);
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship is other faction and currently alive
            if(ship.Value.IFF != _ship.IFF && ship.Value.Alive == true)
            {
                // Get distance from ship
                float distance = Vector3.Distance(_ship.ShipObject.transform.position, ship.Value.ShipObject.transform.position);
                // If distance is less than radius
                if(distance <= _bombRadius)
                {
                    // Tell ship to take damage relative to it's distance
                    ship.Value.TakeDamage(_bombDamage - (distance / _bombRadius * _bombDamage));
                }
            }
        }
        // Ship dies in attack
        _ship.Kill();
    }

    // Check Flank Teleport
    public static void CheckFlankTeleport(Ship _ship, byte _abilityID, GameObject _flankTeleportPrefab, float _flankTeleportRange)
    {
        // If ability input activated and ability is not currently on cooldown and ability is not currently active
        if(_ship.AbilityInput[_abilityID] == true && _ship.AbilityOnCooldown[_abilityID] == false && _ship.AbilityActive[_abilityID] == false && _ship.CurrentTarget != null)
        {
            _flankTeleportPrefab.SetActive(true);
            _ship.ShipObject.transform.position = _ship.CurrentTarget.ShipObject.transform.position + (_ship.CurrentTarget.ShipObject.transform.forward * -_flankTeleportRange);
            // Set ability active
            _ship.AbilityActive[_abilityID] = true;
            // Record ability activated time
            _ship.LastAbilityActivatedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability last activated time is greater than ability duration
        else if(_ship.AbilityActive[_abilityID] == true && Time.time - _ship.LastAbilityActivatedTime[_abilityID] > _ship.AbilityDuration[_abilityID])
        {
            _flankTeleportPrefab.SetActive(false);
            // Set ability to inactive
            _ship.AbilityActive[_abilityID] = false;
            // Set ability to on cooldown
            _ship.AbilityOnCooldown[_abilityID] = true;
            // Record cooldown started time
            _ship.LastAbilityCooldownStartedTime[_abilityID] = Time.time;
        }
        // If difference between current time and ability started cooldown time is greater than ability cooldown time
        else if(_ship.AbilityOnCooldown[_abilityID] == true && Time.time - _ship.LastAbilityCooldownStartedTime[_abilityID] > _ship.AbilityCooldownTime[_abilityID])
        {
            // Take ability off cooldown
            _ship.AbilityOnCooldown[_abilityID] = false;
        }
    }
}
