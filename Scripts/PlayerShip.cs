using UnityEngine;

// Controls the player ships
public class PlayerShip : Ship
{
    // Input script intialization
    private readonly PlayerInput playerInput = new PlayerInput();

    // Player-only GameObjects
    private readonly GameObject ShieldObject;
    private readonly GameObject ScannerObject;
    private Bomb bomb;

    // Player ship constructor
    public PlayerShip(uint _id)
    {
        this.ID = _id;
        this.StartingPosition = new Vector3(0, 0, 0);
        this.IFF = GameController.IFF.Friend;
        this.IsPlayer = true;
        // Ship stats
        // Health/Armor/Shields
        this.Health = 100;
        this.MaxHealth = 100;
        this.Armor = 75;
        this.Shields = 50;
        this.MaxShields = 50;
        this.ShieldRegenSpeed = 1f;
        // Current/Max energy
        this.Energy = 100;
        this.MaxEnergy = 100;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 50;
        this.WarpAccelMultiplier = 3;
        this.MaxImpulseSpeed = 50;
        this.MaxWarpSpeed = 150;
        this.MaxRotationSpeed = 0.1f;
        // Weapon stats
        this.ProjectileType = 0;
        this.ShotAmount = 1;
        this.ShotDamage = 30;
        this.ShotAccuracy = 99;
        this.ShotSpeed = 10;
        this.ShotLifetime = 2.5f;
        this.ShotCurvature = 0;
        this.BombDamage = 120;
        this.BombRadius = 35;
        this.BombSpeed = 2;
        this.BombLiftime = 3;
        this.BombPrimerTime = 0.25f;
        // Cooldowns
        this.ShotCooldownTime = 0.25f;
        this.RegenShieldCooldownTime = 3;
        this.ShieldDuration = 5;
        this.ShieldCooldownTime = 10;
        this.BombCooldownTime = 15;
        this.ScannerCooldownTime = 10;
        // Energy cost
        this.WarpEnergyCost = 3;
        this.ShotEnergyCost = 17;
        this.ShieldDrainCostMultiplier = 2;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load(GameController.PlayerPrefabName, typeof(GameObject)) as GameObject;
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.ShieldObject = this.ShipObject.transform.GetChild(0).Find("Shield").gameObject;
        this.ScannerObject = this.ShipObject.transform.GetChild(0).Find("Scanner").gameObject;
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
        if(this.BombInput == true && this.BombOnCooldown == false && this.BombInFlight == false)
        {
            this.bomb = GameController.SpawnBomb(this, this.IFF, this.BombDamage, this.BombRadius, this.GunBarrelObject.transform.position, this.GunBarrelObject.transform.rotation, this.ShipRigidbody.velocity, this.BombSpeed, this.BombLiftime);
            this.BombOnCooldown = true;
            this.BombInFlight = true;
            this.LastBombActivatedTime = Time.time;
        }
        if(this.BombInFlight && this.BombInput == true && Time.time - this.LastBombActivatedTime > this.BombPrimerTime)
        {
            this.BombInFlight = false;
            this.bomb.Detonate();
        }
        if(this.BombOnCooldown == true && Time.time - this.LastBombActivatedTime > this.BombCooldownTime)
        {
            this.BombOnCooldown = false;
        }
    }

    // Activates shields
    public void ActivateShields()
    {
        // If shield input activated and shield is not currently on cooldown and shield is not currently active
        if(this.playerInput.Shield == true && this.ShieldOnCooldown == false && this.ShieldActive == false)
        {
            // Activate shield object, set shield active to true, and record time shield was activated
            this.ShieldObject.SetActive(true);
            this.ShieldActive = true;
            this.LastShieldActivatedTime = Time.time;
        }
        // If difference between current time and shield last activated time is greater than shield duration
        if(this.ShieldActive == true && Time.time - this.LastShieldActivatedTime > this.ShieldDuration)
        {
            // Disable shield object, set shield to not active, set shield to on cooldown, and record time cooldown started
            this.ShieldObject.SetActive(false);
            this.ShieldActive = false;
            this.ShieldOnCooldown = true;
            this.LastShieldCooldownStartedTime = Time.time;
        }
        // If difference between current time adn shield started cooldown time is greater than shield cooldown time
        if(this.ShieldOnCooldown == true && Time.time - this.LastShieldCooldownStartedTime > this.ShieldCooldownTime)
        {
            // Take shield off cooldown
            this.ShieldOnCooldown = false;
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
}
