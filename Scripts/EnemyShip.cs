using UnityEngine;

// Controls the enemy ships
public class EnemyShip : Ship
{
    // Processes inputs
    public override void ProcessInputs()
    {
        // If there is no current target or current target is dead
        if(this.CurrentTarget == null || this.CurrentTarget.Alive == false)
        {
            // Acquire new target
            this.CurrentTarget = AIController.AcquireTarget(this.ShipObject.transform.position, this.IFF, this.MaxTargetAcquisitionRange);
        }
        // If there is a current target and it is alive
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
        {
            // Stop wandering as currently have target
            this.IsWandering = false;
            // Use AI to figure out if ship should accelerate
            if(AIController.ShouldAccelerate(this.AItype, this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
            {
                this.ImpulseInput = true;
            }
            else
            {
                this.ImpulseInput = false;
            }
            // Use AI to figure out if ship should strafe target, resets strafe direction each time strafing is cancelled
            if(AIController.ShouldStrafe(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxOrbitRange) == true)
            {
                this.StrafeInput = true;
                this.ResetStrafeDirection = false;
            }
            else
            {
                this.StrafeInput = false;
                this.ResetStrafeDirection = true;
            }
            // Use AI to figure out if ship should fire weapons
            if(AIController.ShouldFireGun(this.ShipObject.transform.position, this.CurrentTarget.ShipObject.transform.position, this.MaxWeaponsRange) == true)
            {
                this.GunInput = true;
            }
            else
            {
                this.GunInput = false;
            }
        }
        // If unable to acquire target set wandering to true
        else
        {
            this.IsWandering = true;
        }
        // If wandering
        if(this.IsWandering == true)
        {
            // Wander
            this.Wander();
        }
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // If there is a current target
        if(this.CurrentTarget != null && this.CurrentTarget.Alive == true)
        {
            // Get rotation to face target
            this.IntendedRotation = AIController.GetRotationToTarget(this.ShipObject.transform, this.CurrentTarget.ShipObject.transform.position);
        }
    }
}
