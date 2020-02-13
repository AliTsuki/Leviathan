using UnityEngine;

// Broadside multi cannon enemy ship
public class ESBroadside : EnemyShip
{
    // Enemy ship constructor
    public ESBroadside(uint id, Vector3 startingPosition)
    {
        this.ID = id;
        this.StartingPosition = startingPosition;
        this.Type = EnemyShipType.Broadside;
        this.AItype = AIType.Broadside;
        this.IFF = GameController.IFF.Enemy;
        this.IsPlayer = false;
        // Ship stats
        this.Stats = new ShipStats
        {
            // --Health/Armor/Shields
            Health = 125f,
            MaxHealth = 125f,
            Armor = 80f,
            Shields = 40f,
            MaxShields = 40f,
            ShieldRegenSpeed = 1f,
            ShieldCooldownTime = 3f,
            // --Current/Max energy
            Energy = 100f,
            MaxEnergy = 100f,
            EnergyRegenSpeed = 1.5f,
            // --Energy costs
            GunEnergyCost = 17f,
            // --Acceleration
            EngineCount = 2,
            ImpulseAcceleration = 40f,
            StrafeAcceleration = 20f,
            // --Max Speed
            MaxImpulseSpeed = 40f,
            MaxStrafeSpeed = 20f,
            MaxRotationSpeed = 0.1f,
            // --Weapon stats
            // ----Main gun
            GunBarrelCount = 6,
            GunShotProjectileType = 4,
            GunCooldownTime = 1f,
            GunShotAmount = 2,
            GunShotCurvature = 0f,
            GunShotSightCone = 0f,
            GunShotDamage = 3f,
            GunShotAccuracy = 75f,
            GunShotSpeed = 100f,
            GunShotLifetime = 2f,
        };
        // AI fields
        this.AIAimAssist = false;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 75f;
        this.MaxWeaponsRange = 75f;
        // Experience
        this.XP = (uint)(this.Stats.MaxHealth + this.Stats.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.RandomNumGen.Next(0, 360), 0));
        this.Initialize();
    }

    // Strafe ship if within weapons range, note: strafe input is only set by NPCs
    protected override void StrafeShip()
    {
        // If strafe direction should be reset
        if(this.ResetStrafeDirection == true)
        {
            // Get random number 0 or 1, if 1 strafe right, if 0 strafe left
            if(GameController.RandomNumGen.Next(0, 2) == 1)
            {
                this.StrafeRight = true;
            }
            else
            {
                this.StrafeRight = false;
            }
        }
        // If strafe is activated by AI and ship is below max strafing speed
        if(this.StrafeInput == true && this.ShipRigidbody.velocity.magnitude < this.Stats.MaxStrafeSpeed)
        {
            // Accelerate forward
             this.ShipRigidbody.AddRelativeForce(new Vector3(0, 0, this.Stats.StrafeAcceleration));
        }
    }
}
