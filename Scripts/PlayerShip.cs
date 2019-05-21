using UnityEngine;

// Controls the player ships
public class PlayerShip : Ship
{
    // Input script intialization
    private readonly PlayerInput playerInput = new PlayerInput();

    // Player-only GameObjects
    private readonly GameObject BarrierObject;
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
        this.Health = 200f;
        this.MaxHealth = 200f;
        this.Armor = 75f;
        this.Shields = 100f;
        this.MaxShields = 100f;
        this.ShieldRegenSpeed = 1f;
        // Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // Speed/Acceleration
        this.ImpulseAcceleration = 100f;
        this.WarpAccelerationMultiplier = 3f;
        this.MaxImpulseSpeed = 50f;
        this.MaxWarpSpeed = 150f;
        this.MaxRotationSpeed = 0.1f;
        // Weapon stats
        this.ProjectileType = 0;
        this.GunShotAmount = 1f;
        this.ShotCurvature = 0.05f;
        this.ShotDamage = 30f;
        this.GunShotAccuracy = 99f;
        this.ShotSpeed = 75f;
        this.ShotLifetime = 1f;
        this.BombCurvature = 0f;
        this.BombDamage = 120f;
        this.BombRadius = 35f;
        this.BombSpeed = 30f;
        this.BombLiftime = 3f;
        this.BombPrimerTime = 0.25f;
        // Cooldowns
        this.GunCooldownTime = 0.25f;
        this.ShieldCooldownTime = 3f;
        this.BarrierDuration = 5f;
        this.BarrierCooldownTime = 10f;
        this.BombCooldownTime = 15f;
        this.ScannerCooldownTime = 10f;
        // Energy cost
        this.WarpEnergyCost = 3f;
        this.GunEnergyCost = 17f;
        this.BarrierEnergyDrainCost = 10f;
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.PlayerPrefabName);
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.BarrierObject = this.ShipObject.transform.GetChild(0).Find(GameController.ShieldName).gameObject;
        this.ScannerObject = this.ShipObject.transform.GetChild(0).Find(GameController.ScannerName).gameObject;
        this.Initialize();
    }


    // Processes inputs
    public override void ProcessInputs()
    {
        this.playerInput.Update();
        this.HorizontalInput = this.playerInput.Horizontal;
        this.VerticalInput = this.playerInput.Vertical;
        this.ImpulseInput = this.playerInput.Impulse;
        this.WarpInput = this.playerInput.Warp;
        this.GunInput = this.playerInput.Fire;
        this.BombInput = this.playerInput.Bomb;
        this.BarrierInput = this.playerInput.Barrier;
        this.ScannerInput = this.playerInput.Scanner;
        this.PauseInput = this.playerInput.Pause;
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // If joystick is pointed in some direction
        if(this.HorizontalInput != 0 || this.VerticalInput != 0)
        {
            // Set intended angle to dual-argument arc-tangent of vertical input and horizontal input, finally multiply by constant to convert from rad to deg
            this.IntendedAngle = Mathf.Atan2(this.VerticalInput, this.HorizontalInput) * Mathf.Rad2Deg;
            // Set intended rotation to intended angle on y axis
            this.IntendedRotation = Quaternion.Euler(new Vector3(0, this.IntendedAngle, 0));
        }
    }

    // Uses abilities: fire weapons, bombs, use barrier and scanner
    public override void CheckAbilities()
    {
        this.CheckMainGun();
        this.CheckBomb();
        this.CheckBarrier();
        this.CheckScanner();
    }

    // Fires bombs
    public void CheckBomb()
    {
        // If bomb input is active, bomb is not on cooldown, and there is no bomb in flight
        if(this.BombInput == true && this.BombOnCooldown == false && this.BombInFlight == false)
        {
            // Spawn a bomb
            this.bomb = GameController.SpawnBomb(this, this.IFF, this.BombCurvature, this.BombDamage, this.BombRadius, this.GunBarrelObject.transform.position, this.GunBarrelObject.transform.rotation, this.ShipRigidbody.velocity, this.BombSpeed, this.BombLiftime);
            // Set bomb on cooldown
            this.BombOnCooldown = true;
            // Set bomb in flight
            this.BombInFlight = true;
            // Record bomb activated time
            this.LastBombActivatedTime = Time.time;
        }
        // If bomb is in flight, bomb input is pressed, and time since bomb was activated is more than the bomb primer time
        if(this.BombInFlight == true && this.BombInput == true && Time.time - this.LastBombActivatedTime > this.BombPrimerTime)
        {
            // Set bomb not in flight
            this.BombInFlight = false;
            // Detonate bomb
            this.bomb.Detonate();
        }
        // If bomb is on cooldown and time since bomb was last activated is greater than bomb cooldown time
        if(this.BombOnCooldown == true && Time.time - this.LastBombActivatedTime > this.BombCooldownTime)
        {
            // Take bomb off cooldown
            this.BombOnCooldown = false;
        }
    }

    // Activates barrier
    public void CheckBarrier()
    {
        // If barrier input activated and barrier is not currently on cooldown and barrier is not currently active
        if(this.playerInput.Barrier == true && this.BarrierOnCooldown == false && this.BarrierActive == false)
        {
            // Activate barrier object, set barrier active to true, and record time barrier was activated
            this.BarrierObject.SetActive(true);
            this.BarrierActive = true;
            this.LastBarrierActivatedTime = Time.time;
        }
        // If difference between current time and shield last activated time is greater than barrier duration
        if(this.BarrierActive == true && Time.time - this.LastBarrierActivatedTime > this.BarrierDuration)
        {
            // Disable barrier object, set barrier to not active, set barrier to on cooldown, and record time cooldown started
            this.BarrierObject.SetActive(false);
            this.BarrierActive = false;
            this.BarrierOnCooldown = true;
            this.LastBarrierCooldownStartedTime = Time.time;
        }
        // If difference between current time and barrier started cooldown time is greater than barrier cooldown time
        if(this.BarrierOnCooldown == true && Time.time - this.LastBarrierCooldownStartedTime > this.BarrierCooldownTime)
        {
            // Take barrier off cooldown
            this.BarrierOnCooldown = false;
        }
    }

    // Activates scanner
    public void CheckScanner()
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
