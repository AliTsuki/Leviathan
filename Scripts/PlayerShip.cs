using UnityEngine;

// Controls the player ships
public class PlayerShip : Ship
{
    // Input script intialization
    private readonly PlayerInput playerInput = new PlayerInput();

    // Player-only GameObjects
    private GameObject ShieldObject;
    private GameObject ScannerObject;

    // Player ship constructor
    public PlayerShip(uint _id)
    {
        this.ID = _id;
        this.StartingPosition = new Vector3(0, 0, 0);
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = true;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 100f;
        this.MaxHealth = 100f;
        this.Armor = 1f;
        this.Shields = 100f;
        this.MaxShields = 100f;
        this.ShieldRegenSpeed = 1f;
        // Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 40f;
        this.WarpAccelMultiplier = 3f;
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 150f;
        // Weapon stats
        this.ProjectileType = 0;
        this.ShotDamage = 35f;
        this.ShotAccuracy = 1f;
        this.ShotSpeed = 10f;
        this.ShotLifetime = 2.5f;
        this.ShotCurvature = 0f;
        // Cooldowns
        this.ShotCooldownTime = 0.25f;
        this.RegenShieldCooldownTime = 2.5f;
        this.ShieldCooldownTime = 10f;
        this.BombCooldownTime = 10f;
        this.ScannerCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 3f;
        this.ShotEnergyCost = 17f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load(GameController.PlayerPrefabName, typeof(GameObject)) as GameObject;
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.ShieldObject = this.ShipObject.transform.Find("Shield").gameObject;
        this.ScannerObject = this.ShipObject.transform.Find("Scanner").gameObject;
        this.Start();
    }


    // Processes inputs
    public override void ProcessInputs()
    {
        this.playerInput.Update();
        this.HorizontalInput = this.playerInput.Horizontal;
        this.VerticalInput = this.playerInput.Vertical;
        this.ImpulseInput = this.playerInput.Impulse;
        this.WarpInput = this.playerInput.Warp;
        this.FireInput = this.playerInput.Fire;
        this.BombInput = this.playerInput.Bomb;
        this.ShieldInput = this.playerInput.Shield;
        this.ScannerInput = this.playerInput.Scanner;
        this.PauseInput = this.playerInput.Pause;
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // If joystick is pointed in some direction, set a new intended rotation
        if(this.HorizontalInput != 0 || this.VerticalInput != 0)
        {
            this.IntendedAngle = Mathf.Atan2(this.VerticalInput, this.HorizontalInput) * Mathf.Rad2Deg;
            this.IntendedRotation = Quaternion.Euler(new Vector3(0, this.IntendedAngle, 0));
        }
    }

    // Uses abilities: fire weapons, bombs, use shield and scanner
    public override void UseAbilities()
    {
        this.FireMainGun();
        this.FireBomb();
        this.ActivateShields();
        this.ActivateScanner();
    }

    // Fires bombs
    public void FireBomb()
    {
        // TODO: Make this do something, maybe shoots out a projectile, and when the bomb button is pressed a second time it explodes for massive damage to wide area
    }

    // Activates shields
    public void ActivateShields()
    {
        // TODO: Make Shield actually do something, maybe it runs for a short time and blocks all incoming damage or something
        if(this.playerInput.Shield == true)
        {
            this.ShieldObject.SetActive(true);
        }
        else
        {
            this.ShieldObject.SetActive(false);
        }
    }

    // Activates scanner
    public void ActivateScanner()
    {
        // TODO: Make Scanner actually do something, maybe send out a circular wave and have it identify something about the enemies nearby
        if(this.playerInput.Scanner == true)
        {
            this.ScannerObject.SetActive(true);
        }
        else
        {
            this.ScannerObject.SetActive(false);
        }
    }

    // Called when entity is destroyed
    public override void Kill()
    {
        // TODO: Make a proper kill method for player, like set up a save system and respawn or something
        this.Alive = false;
        this.ShipRigidbody.velocity = Vector3.zero;
    }
}
