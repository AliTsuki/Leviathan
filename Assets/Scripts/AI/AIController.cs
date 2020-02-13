using System.Collections.Generic;

using UnityEngine;

// Methods for AI
public abstract partial class Ship
{
    // Process AI for NPC ships
    public void ProcessAI()
    {
        // Non-Drone AI
        if(this.AItype != Ship.AIType.Drone)
        {
            // If there is no current target or current target is dead
            if(this.CurrentTarget == null || this.CurrentTarget.Alive == false)
            {
                // Acquire any target within distance
                this.CurrentTarget = Targeting.AcquireTarget(this.ShipObject.transform.position, this.IFF, this.MaxTargetAcquisitionRange);
                // If no target was acquired
                if(this.CurrentTarget == null)
                {
                    // Set ship to wander
                    this.ShouldWander = true;
                }
            }
            // If there is a current target and it is alive
            else if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
            {
                // If target ship is greater than twice maximum target acquisition distance away then untarget and break out of AI loop
                if(Vector3.Distance(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position) > this.MaxTargetAcquisitionRange * 2f)
                {
                    this.CurrentTarget = null;
                    return;
                }
                // Stop wandering as currently have target
                this.ShouldWander = false;
                // If ship should accelerate
                if(this.CanAccelerate == true)
                {
                    // Use AI to figure out if ship should accelerate
                    if(this.ShouldShipAccelerate(this.CanStrafe, this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
                    {
                        this.ImpulseInput = true;
                    }
                    else
                    {
                        this.ImpulseInput = false;
                    }
                }
                // If ship should strafe
                if(this.CanStrafe == true)
                {
                    // Use AI to figure out if ship should strafe target, resets strafe direction each time strafing is cancelled
                    if(this.ShouldShipStrafe(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
                    {
                        this.StrafeInput = true;
                        this.ResetStrafeDirection = false;
                    }
                    else
                    {
                        this.StrafeInput = false;
                        this.ResetStrafeDirection = true;
                    }
                }
                // If ship should fire guns
                if(this.CanFireGuns == true)
                {
                    // Use AI to figure out if ship should fire weapons
                    if(this.ShouldShipFireGun(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxWeaponsRange) == true)
                    {
                        this.MainGunInput = true;
                    }
                    else
                    {
                        this.MainGunInput = false;
                    }
                }
                // If ship should use ability
                if(this.CanUseAbilities == true)
                {
                    // Use AI to figure out if ship should use ability
                    if(this.ShouldShipUseAbility(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxAbilityRange))
                    {
                        this.AbilityInput[0] = true;
                    }
                    else
                    {
                        this.AbilityInput[0] = false;
                    }
                }
            }
        }
        // Drone AI
        else if(this.AItype == Ship.AIType.Drone)
        {
            // If ship is drone and it is past its leash distance from parent
            if((this.AItype == Ship.AIType.Drone && this.Parent.Alive == true && Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) > this.MaxLeashDistance) || this.Parent.WarpEngineInput == true)
            {
                this.CanFollowParent = true;
            }
            // If there is no current target or current target is dead
            if((this.CurrentTarget == null || this.CurrentTarget.Alive == false) && this.CanFollowParent == false)
            {
                // Acquire any target within distance
                this.CurrentTarget = Targeting.AcquireTarget(this.ShipObject.transform.position, this.IFF, this.MaxTargetAcquisitionRange);
                // If no target was acquired
                if(this.CurrentTarget == null)
                {
                    this.CanFollowParent = true;
                }
            }
            // If there is a current target and it is alive
            else if(this.CurrentTarget != null && this.CurrentTarget.Alive == true && this.CanFollowParent == false)
            {
                // If target ship is greater than twice maximum target acquisition distance away then untarget and break out of AI loop
                if(Vector3.Distance(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position) > this.MaxTargetAcquisitionRange * 2f)
                {
                    this.CurrentTarget = null;
                    return;
                }
                // If ship should accelerate
                if(this.CanAccelerate == true)
                {
                    // Use AI to figure out if ship should accelerate
                    if(this.ShouldShipAccelerate(this.CanStrafe, this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
                    {
                        this.ImpulseInput = true;
                    }
                    else
                    {
                        this.ImpulseInput = false;
                    }
                }
                // If ship should strafe
                if(this.CanStrafe == true)
                {
                    // Use AI to figure out if ship should strafe target, resets strafe direction each time strafing is cancelled
                    if(this.ShouldShipStrafe(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
                    {
                        this.StrafeInput = true;
                        this.ResetStrafeDirection = false;
                    }
                    else
                    {
                        this.StrafeInput = false;
                        this.ResetStrafeDirection = true;
                    }
                }
                // If ship should fire guns
                if(this.CanFireGuns == true)
                {
                    // Use AI to figure out if ship should fire weapons
                    if(this.ShouldShipFireGun(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxWeaponsRange) == true)
                    {
                        this.MainGunInput = true;
                    }
                    else
                    {
                        this.MainGunInput = false;
                    }
                }
            }
        }
    }

    // Called when a ship has no target and nothing else to do
    protected void Wander()
    {
        // If should wander is true
        if(this.ShouldWander == true)
        {
            // Stop shooting
            this.MainGunInput = false;
            // If not moving(wandering) and not currently waiting around
            if(this.IsWanderMove == false && this.IsWaiting == false)
            {
                // Start waiting for random amount of time between 0-10 seconds
                this.IsWaiting = true;
                this.StartedWaitingTime = Time.time;
                this.TimeToWait = GameController.RandomNumGen.Next(0, 11);
                // Turn off engines
                this.ImpulseInput = false;
            }
            // If done waiting, stop waiting
            if(Time.time - this.StartedWaitingTime > this.TimeToWait)
            {
                this.IsWaiting = false;
            }
            // If done waiting and not yet moving
            if(this.IsWaiting == false && this.IsWanderMove == false)
            {
                // Start moving for random amount of time between 0-10 seconds and rotate some random direction
                this.IsWanderMove = true;
                this.StartedWanderMoveTime = Time.time;
                this.TimeToWanderMove = GameController.RandomNumGen.Next(0, 11);
                this.IntendedRotation = Quaternion.Euler(0, GameController.RandomNumGen.Next(0, 360), 0);
            }
            // If moving, set impulse to true which causes ship to accelerate forward
            if(this.IsWanderMove == true)
            {
                this.ImpulseInput = true;
            }
            // If done moving, stop moving
            if(Time.time - this.StartedWanderMoveTime > this.TimeToWanderMove)
            {
                this.IsWanderMove = false;
            }
        }
    }

    // Called when drones need to follow their parent ship
    protected void FollowParent()
    {
        // If should follow parent is true
        if(this.CanFollowParent == true && this.Parent.Alive == true)
        {
            // Stop shooting
            this.MainGunInput = false;
            // If distance to parent ship is greater than max leash distance multiplied by 1.5
            if(Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) > this.MaxLeashDistance * 1.5f)
            {
                // Teleport directly to parent
                this.ShipObject.transform.position = this.Parent.ShipObject.transform.position;
                // Rotate same rotation as parent
                this.IntendedRotation = this.Parent.ShipObject.transform.rotation;
            }
            // If distance to parent ship is greater than orbit parent range
            else if(Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) > this.OrbitParentRange)
            {
                // Reset targetting
                this.CurrentTarget = null;
                // Rotate to face parent
                this.IntendedRotation = Targeting.GetRotationToTarget(this.ShipObject.transform, this.Parent.ShipObject.transform.position);
                // Accelerate forward
                this.ImpulseInput = true;
            }
            // If distance to parent is less than or equal to orbit parent range
            else if(Vector3.Distance(this.ShipObject.transform.position, this.Parent.ShipObject.transform.position) <= this.OrbitParentRange)
            {
                // Turn off engine
                this.ImpulseInput = false;
                // Set velocity to parent velocity
                this.ShipRigidbody.velocity = this.Parent.ShipRigidbody.velocity;
                // Set rotation to parent rotation
                this.IntendedRotation = this.Parent.ShipObject.transform.rotation;
                // Stop following parent
                this.CanFollowParent = false;
            }
        }
    }

    // Get intended rotation
    public void GetIntendedRotationAI()
    {
        if(this.AItype == Ship.AIType.Broadside)
        {
            // If there is a current target and it is alive and not currently strafing
            if(this.CurrentTarget != null && this.CurrentTarget.Alive == true && this.StrafeInput == false)
            {
                // Get rotation to face target
                this.IntendedRotation = Targeting.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
            }
            // If there is a current target and it is alive and we are currenlty strafing
            else if(this.CurrentTarget != null && this.CurrentTarget.Alive == true && this.StrafeInput == true)
            {
                // Get rotation to face target
                this.IntendedRotation = Targeting.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
                // If strafe right
                if(this.StrafeRight == true)
                {
                    // Add 90 degrees to y rotation axis
                    this.IntendedRotation = Quaternion.Euler(this.IntendedRotation.eulerAngles.x, this.IntendedRotation.eulerAngles.y + 90, this.IntendedRotation.eulerAngles.z);
                }
                // If strafe left
                else
                {
                    // Subtract 90 degrees from y rotation axis
                    this.IntendedRotation = Quaternion.Euler(this.IntendedRotation.eulerAngles.x, this.IntendedRotation.eulerAngles.y - 90, this.IntendedRotation.eulerAngles.z);
                }
            }
        }
        else
        {
            // If there is a current target that is alive
            if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
            {
                // Get rotation to face target
                this.IntendedRotation = Targeting.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
            }
        }
    }

    // Checks if the ship should accelerate based on its distance to its target
    public bool ShouldShipAccelerate(bool shouldStrafe, Vector3 userPosition, Vector3 targetPosition, float maxOrbitRange)
    {
        if(shouldStrafe == true)
        {
            if(Vector3.Distance(userPosition, targetPosition) > maxOrbitRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(shouldStrafe == false)
        {
            return true;
        }
        return false;
    }

    // Checks if the ship should strafe based on its distance to its target
    public bool ShouldShipStrafe(Vector3 userPosition, Vector3 targetPosition, float maxOrbitRange)
    {
        if(Vector3.Distance(userPosition, targetPosition) <= maxOrbitRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Checks if the ship should fire main weapons based on its distance to its target
    public bool ShouldShipFireGun(Vector3 userPosition, Vector3 targetPosition, float maxWeaponsRange)
    {
        if(Vector3.Distance(userPosition, targetPosition) < maxWeaponsRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Checks if the ship should use an ability
    public bool ShouldShipUseAbility(Vector3 userPosition, Vector3 targetPosition, float abilityUsageRange)
    {
        if(Vector3.Distance(userPosition, targetPosition) < abilityUsageRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

// Targeting methods for AI
public static class Targeting
{
    // Acquires a target with opposite IFF
    public static Ship AcquireTarget(Vector3 userPosition, GameController.IFF iff, float maxTargetingDistance)
    {
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != iff && ship.Value.Alive == true && ship.Value.AItype != Ship.AIType.Drone && Vector3.Distance(ship.Value.ShipObject.transform.position, userPosition) < maxTargetingDistance)
            {
                // Returns the target
                return ship.Value;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return null;
    }

    // Acquires closest target
    public static Ship AcquireClosestTarget(Vector3 userPosition, GameController.IFF iff, float maxTargetingDistance)
    {
        Ship CurrentTarget = null;
        Ship FinalTarget = null;
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != iff && ship.Value.Alive == true && Vector3.Distance(ship.Value.ShipObject.transform.position, userPosition) < maxTargetingDistance)
            {
                CurrentTarget = ship.Value;
                if(FinalTarget == null)
                {
                    FinalTarget = CurrentTarget;
                }
            }
            if(FinalTarget != null && CurrentTarget != null && Vector3.Distance(CurrentTarget.ShipObject.transform.position, userPosition) < Vector3.Distance(FinalTarget.ShipObject.transform.position, userPosition))
            {
                FinalTarget = CurrentTarget;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return FinalTarget;
    }

    // Acquires a target with opposite IFF that is forward
    public static Ship AcquireForwardTarget(Transform userTransform, GameController.IFF iff, float maxTargetingDistance, float sightCone)
    {
        // Loops through all ships in game
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // Checks if ship is opposite IFF, is alive, and is within max targeting distance
            if(ship.Value.IFF != iff && ship.Value.Alive == true && Vector3.Distance(ship.Value.ShipObject.transform.position, userTransform.position) < maxTargetingDistance && Vector3.Dot(userTransform.forward, (ship.Value.ShipObject.transform.position - userTransform.position).normalized) > sightCone)
            {
                // Returns the target
                return ship.Value;
            }
        }
        // If there is no ship of opposite IFF, alive, and within distance return null
        return null;
    }

    // Gets the intended rotation to face toward given target
    public static Quaternion GetRotationToTarget(Transform userTransform, Vector3 targetPosition)
    {
        return Quaternion.LookRotation(targetPosition - userTransform.position);
    }
}
