using System.Collections.Generic;

using UnityEngine;

// Methods for AI
public static class AIController
{
    // Process AI for NPC ships
    public static void ProcessAI(Ship _ship)
    {
        // Non-Drone AI
        if(_ship.AItype != Ship.AIType.Drone)
        {
            // If there is no current target or current target is dead
            if(_ship.CurrentTarget == null || _ship.CurrentTarget.Alive == false)
            {
                // Acquire any target within distance
                _ship.CurrentTarget = AcquireTarget(_ship.ShipObject.transform.position, _ship.IFF, _ship.MaxTargetAcquisitionRange);
                // If no target was acquired
                if(_ship.CurrentTarget == null)
                {
                    // Set ship to wander
                    _ship.ShouldWander = true;
                }
            }
            // If there is a current target and it is alive
            else if(_ship.CurrentTarget != null && _ship.CurrentTarget.Alive == true)
            {
                // If target ship is greater than twice maximum target acquisition distance away then untarget and break out of AI loop
                if(Vector3.Distance(_ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position) > _ship.MaxTargetAcquisitionRange * 2f)
                {
                    _ship.CurrentTarget = null;
                    return;
                }
                // Stop wandering as currently have target
                _ship.ShouldWander = false;
                // If ship should accelerate
                if(_ship.ShouldAccelerate == true)
                {
                    // Use AI to figure out if ship should accelerate
                    if(ShouldAccelerate(_ship.ShouldStrafe, _ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position, _ship.MaxOrbitRange) == true)
                    {
                        _ship.ImpulseEngineInput = 1f;
                    }
                    else
                    {
                        _ship.ImpulseEngineInput = 0f;
                    }
                }
                // If ship should strafe
                if(_ship.ShouldStrafe == true)
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
                }
                // If ship should fire guns
                if(_ship.ShouldFireGuns == true)
                {
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
        }
        // Drone AI
        else if(_ship.AItype == Ship.AIType.Drone)
        {
            // If ship is drone and it is past its leash distance from parent
            if(_ship.AItype == Ship.AIType.Drone && Vector3.Distance(_ship.ShipObject.transform.position, _ship.Parent.ShipObject.transform.position) > _ship.MaxLeashDistance || _ship.Parent.WarpEngineInput > 0f)
            {
                _ship.ShouldFollowParent = true;
            }
            // If there is no current target or current target is dead
            if((_ship.CurrentTarget == null || _ship.CurrentTarget.Alive == false) && _ship.ShouldFollowParent == false)
            {
                // Acquire any target within distance
                _ship.CurrentTarget = AcquireTarget(_ship.ShipObject.transform.position, _ship.IFF, _ship.MaxTargetAcquisitionRange);
                // If no target was acquired
                if(_ship.CurrentTarget == null)
                {
                    _ship.ShouldFollowParent = true;
                }
            }
            // If there is a current target and it is alive
            else if(_ship.CurrentTarget != null && _ship.CurrentTarget.Alive == true && _ship.ShouldFollowParent == false)
            {
                // If target ship is greater than twice maximum target acquisition distance away then untarget and break out of AI loop
                if(Vector3.Distance(_ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position) > _ship.MaxTargetAcquisitionRange * 2f)
                {
                    _ship.CurrentTarget = null;
                    return;
                }
                // If ship should accelerate
                if(_ship.ShouldAccelerate == true)
                {
                    // Use AI to figure out if ship should accelerate
                    if(ShouldAccelerate(_ship.ShouldStrafe, _ship.ShipObject.transform.position, _ship.CurrentTarget.ShipObject.transform.position, _ship.MaxOrbitRange) == true)
                    {
                        _ship.ImpulseEngineInput = 1f;
                    }
                    else
                    {
                        _ship.ImpulseEngineInput = 0f;
                    }
                }
                // If ship should strafe
                if(_ship.ShouldStrafe == true)
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
                }
                // If ship should fire guns
                if(_ship.ShouldFireGuns == true)
                {
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
            if(ship.Value.IFF != _iff && ship.Value.Alive == true && ship.Value.AItype != Ship.AIType.Drone && Vector3.Distance(ship.Value.ShipObject.transform.position, _userPosition) < _maxTargetingDistance)
            {
                // Returns the target
                return ship.Value;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return null;
    }

    // Acquires closest target
    public static Ship AcquireClosestTarget(Vector3 _userPosition, GameController.IFF _iff, float _maxTargetingDistance)
    {
        Ship CurrentTarget = null;
        Ship FinalTarget = null;
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != _iff && ship.Value.Alive == true && Vector3.Distance(ship.Value.ShipObject.transform.position, _userPosition) < _maxTargetingDistance)
            {
                CurrentTarget = ship.Value;
                if(FinalTarget == null)
                {
                    FinalTarget = CurrentTarget;
                }
            }
            if(FinalTarget != null && CurrentTarget != null && Vector3.Distance(CurrentTarget.ShipObject.transform.position, _userPosition) < Vector3.Distance(FinalTarget.ShipObject.transform.position, _userPosition))
            {
                FinalTarget = CurrentTarget;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return FinalTarget;
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
    public static bool ShouldAccelerate(bool _shouldStrafe, Vector3 _userPosition, Vector3 _targetPosition, float _maxOrbitRange)
    {
        if(_shouldStrafe == true)
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
        else if(_shouldStrafe == false)
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
