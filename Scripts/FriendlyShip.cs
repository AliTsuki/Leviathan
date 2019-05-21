using UnityEngine;

// Controls the friendly ships
public class FriendlyShip : Ship
{
    // Enemy ship constructor
    public FriendlyShip(uint _id)
    {
        this.ID = _id;
        this.StartingPosition = new Vector3(0, 0, 0);
        this.IFF = GameController.IFF.Friend;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 0f;
        this.MaxHealth = 100f;
        this.Armor = 0f;
        this.Shields = 0f;
        this.MaxShields = 100f;
        // Current/Max energy
        this.Energy = 0f;
        this.MaxEnergy = 100f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 40f;
        this.WarpAccelerationMultiplier = 3f;
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 150f;
        // Weapon stats
        this.ShotDamage = 10f;
        this.ShotSpeed = 10f;
        this.ShotLifetime = 2.5f;
        this.ShotCurvature = 0f;
        // Cooldowns
        this.GunCooldownTime = 0.25f;
        this.BarrierCooldownTime = 10f;
        this.BombCooldownTime = 10f;
        this.ScannerCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 5f;
        this.GunEnergyCost = 5f;
        // GameObject Instantiation
        //this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyPrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.identity);
        this.Initialize();
    }


    // Processes inputs
    public override void ProcessInputs()
    {
        // Do AI stuff
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // Do AI stuff
    }
}
