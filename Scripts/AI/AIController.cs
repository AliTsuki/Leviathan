using System.Collections.Generic;

using UnityEngine;

// Methods for AI
public static class AIController
{
    // Process AI for NPC ships
    public static void ProcessAI(Ship _ship)
    {
        // If ship is enemy
        if(_ship.IFF == GameController.IFF.Enemy)
        {
            // If there is no current target or current target is dead
            if(_ship.CurrentTarget == null || _ship.CurrentTarget.Alive == false)
            {
                // Acquire new target
                _ship.CurrentTarget = AcquireTarget(_ship.ShipObject.transform.position, _ship.IFF, _ship.MaxTargetAcquisitionRange);
            }
            // If there is a current target and it is alive
            if(_ship.CurrentTarget != null && _ship.CurrentTarget.Alive == true)
            {
                // Stop wandering as currently have target
                _ship.IsWandering = false;
                // Use AI to figure out if ship should accelerate
                if(ShouldAccelerate(_ship.AItype, _ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position, _ship.MaxOrbitRange) == true)
                {
                    _ship.ImpulseInput = true;
                }
                else
                {
                    _ship.ImpulseInput = false;
                }
                // Ramming type ships don't use strafing or main guns
                if(_ship.AItype != Ship.AIType.Ramming)
                {
                    // Use AI to figure out if ship should strafe target, resets strafe direction each time strafing is cancelled
                    if(ShouldStrafe(_ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position, _ship.MaxOrbitRange) == true)
                    {
                        _ship.StrafeInput = true;
                        _ship.ResetStrafeDirection = false;
                    }
                    else
                    {
                        _ship.StrafeInput = false;
                        _ship.ResetStrafeDirection = true;
                    }
                    // Use AI to figure out if ship should fire weapons
                    if(ShouldFireGun(_ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position, _ship.MaxWeaponsRange) == true)
                    {
                        _ship.MainGunInput = true;
                    }
                    else
                    {
                        _ship.MainGunInput = false;
                    }
                }
            }
            // If unable to acquire target set wandering to true
            else
            {
                _ship.IsWandering = true;
            }
            // If wandering
            if(_ship.IsWandering == true)
            {
                // Wander
                _ship.Wander();
            }
        }
        else if(_ship.IFF == GameController.IFF.Friend)
        {
            // TODO: Friendly ship AI
        }
    }

    // Get intended rotation
    public static void GetIntendedRotation(Ship _ship)
    {
        if(_ship.AItype == Ship.AIType.Broadside)
        {
            // If there is a current target and it is alive and not currently strafing
            if(_ship.CurrentTarget != null && _ship.CurrentTarget.Alive == true && _ship.StrafeInput == false)
            {
                // Get rotation to face target
                _ship.IntendedRotation = GetRotationToTarget(_ship.ShipObject.transform, _ship.CurrentTarget.ShipObject.transform.position);
            }
            // If there is a current target and it is alive and we are currenlty strafing
            else if(_ship.CurrentTarget != null && _ship.CurrentTarget.Alive == true && _ship.StrafeInput == true)
            {
                // Get rotation to face target
                _ship.IntendedRotation = GetRotationToTarget(_ship.ShipObject.transform, _ship.CurrentTarget.ShipObject.transform.position);
                // If strafe right
                if(_ship.StrafeRight == true)
                {
                    // Add 90 degrees to y rotation axis
                    _ship.IntendedRotation = Quaternion.Euler(_ship.IntendedRotation.eulerAngles.x, _ship.IntendedRotation.eulerAngles.y + 90, _ship.IntendedRotation.eulerAngles.z);
                }
                // If strafe left
                else
                {
                    // Subtract 90 degrees from y rotation axis
                    _ship.IntendedRotation = Quaternion.Euler(_ship.IntendedRotation.eulerAngles.x, _ship.IntendedRotation.eulerAngles.y - 90, _ship.IntendedRotation.eulerAngles.z);
                }
            }
        }
        else
        {
            // If there is a current target that is alive
            if(_ship.CurrentTarget != null && _ship.CurrentTarget.Alive == true)
            {
                // Get rotation to face target
                _ship.IntendedRotation = GetRotationToTarget(_ship.ShipObject.transform, _ship.CurrentTarget.ShipObject.transform.position);
            }
        }
    }

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
    public static Ship AcquireForwardTarget(Transform _userTransform, GameController.IFF _iff, float _maxTargetingDistance, float _sightCone)
    {
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != _iff && ship.Value.Alive == true && Vector3.Distance(ship.Value.ShipObject.transform.position, _userTransform.position) < _maxTargetingDistance && Vector3.Dot(_userTransform.forward, (ship.Value.ShipObject.transform.position - _userTransform.position).normalized) > _sightCone)
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
