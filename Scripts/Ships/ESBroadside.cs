using UnityEngine;

// Broadside multi cannon enemy ship
public class ESBroadside : EnemyShip
{
    // Enemy ship constructor
    public ESBroadside(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.IFF = GameController.IFF.Enemy;
        this.AItype = AIType.Standard;
        this.IsPlayer = false;
        // Ship stats
        // --Health/Armor/Shields
        this.Health = 100f;
        this.MaxHealth = 100f;
        this.Armor = 80f;
        this.Shields = 25f;
        this.MaxShields = 25f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.GunEnergyCost = 17f;
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 40f;
        this.StrafeAcceleration = 20f;
        // --Max Speed
        this.MaxImpulseSpeed = 40f;
        this.MaxStrafeSpeed = 20f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 6;
        this.GunShotProjectileType = 4;
        this.GunCooldownTime = 1f;
        this.GunShotAmount = 2;
        this.GunShotCurvature = 0f;
        this.GunShotSightCone = 0f;
        this.GunShotDamage = 4f;
        this.GunShotAccuracy = 75f;
        this.GunShotSpeed = 100f;
        this.GunShotLifetime = 2f;
        // AI fields
        this.AIAimAssist = false;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 75f;
        this.MaxWeaponsRange = 75f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyBroadsidePrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.Initialize();
    }

    // Strafe ship if within weapons range, note: strafe input is only set by NPCs
    public override void StrafeShip()
    {
        // If strafe direction should be reset
        if(this.ResetStrafeDirection == true)
        {
            // Get random number 0 or 1, if 1 strafe right, if 0 strafe left
            if(GameController.r.Next(0, 2) == 1)
            {
                this.StrafeRight = true;
            }
            else
            {
                this.StrafeRight = false;
            }
        }
        // If strafe is activated by AI and ship is below max strafing speed
        if(this.StrafeInput == true && this.ShipRigidbody.velocity.magnitude < this.MaxStrafeSpeed)
        {
            // Accelerate forward
             this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.StrafeAcceleration));
        }
    }
}
