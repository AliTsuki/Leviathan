using System.Collections.Generic;

using UnityEngine;

// Methods for AI
public static class AIController
{
    // Acquires a target with opposite IFF
    public static Ship AcquireTarget(Vector3 _userPosition, GameController.IFF _iff, float _maxTargetingDistance)
    {
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != _iff && ship.Value.Alive == true && Vector3.Distance(ship.Value.ShipObject.transform.position, _userPosition) < _maxTargetingDistance)
            {
                // Returns the target
                return ship.Value;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return null;
    }

    // Acquires a target with opposite IFF that is forward
    public static Ship AcquireForwardTarget(Transform _userTransform, GameController.IFF _iff, float _maxTargetingDistance)
    {
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != _iff && ship.Value.Alive == true && Vector3.Distance(ship.Value.ShipObject.transform.position, _userTransform.position) < _maxTargetingDistance && Vector3.Dot(_userTransform.forward, (ship.Value.ShipObject.transform.position - _userTransform.position).normalized) > 0.8f)
            {
                // Returns the target
                return ship.Value;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return null;
    }

    // Gets the intended rotation to face toward given target
    public static Quaternion GetRotationToTarget(Transform _userTransform, Vector3 _targetPosition)
    {
        return Quaternion.LookRotation(_targetPosition - _userTransform.position);
    }

    // Checks if the ship should accelerate based on its distance to its target
    public static bool ShouldAccelerate(Ship.AIType _aitype, Vector3 _userPosition, Vector3 _targetPosition, float _maxOrbitRange)
    {
        if(_aitype == Ship.AIType.Standard || _aitype == Ship.AIType.Broadside)
        {
            if(Vector3.Distance(_userPosition, _targetPosition) > _maxOrbitRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(_aitype == Ship.AIType.Ramming)
        {
            return true;
        }
        return false;
    }

    // Checks if the ship should strafe based on its distance to its target
    public static bool ShouldStrafe(Vector3 _userPosition, Vector3 _targetPosition, float _maxOrbitRange)
    {
        if(Vector3.Distance(_userPosition, _targetPosition) <= _maxOrbitRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Checks if the ship should fire main weapons based on its distance to its target
    public static bool ShouldFireGun(Vector3 _userPosition, Vector3 _targetPosition, float _maxWeaponsRange)
    {
        if(Vector3.Distance(_userPosition, _targetPosition) < _maxWeaponsRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
