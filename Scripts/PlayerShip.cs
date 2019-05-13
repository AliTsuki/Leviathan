using UnityEngine;

// Controls the player ships
public class PlayerShip //: Ship
{
    // GameObjects and Components
    public GameObject Ship;
    private GameObject playerPrefab;
    private PlayerInput playerInput = new PlayerInput();
    private Rigidbody shipRigidbody;
    private GameObject impulseEngine;
    private GameObject warpEngine;
    private GameObject gunBarrel;
    private GameObject gunBarrelLights;
    private GameObject shield;
    private GameObject scanner;
    private ParticleSystem impulseParticleSystem;
    private ParticleSystem warpParticleSystem;
    private ParticleSystem.MainModule impulseParticleMain;
    private ParticleSystem.MainModule warpParticleMain;
    private AudioSource impulseAudio;
    private AudioSource warpAudio;
    private AudioSource gunAudio;
    private Vector3 intendedRotation;
    private Vector3 currentRotation;
    private Vector3 nextRotation;
    private Vector3 tiltRotation;
    
    // Fields not modified by player equipment/level
    private int recentRotationsIndex = 0;
    private static int recentRotationsIndexMax = 30;
    private float[] recentRotations;
    private float recentRotationsAverage = 0f;
    private float tiltAngle = 0f;
    private bool intendedRotationClockwise = false;
    private float differenceAngle = 0f;
    private float intendedAngle = 0f;
    private float maxRotationSpeed = 5f;
    private float lastShotFireTime = 0f;
    private bool weaponOnCooldown = false;

    // Ship stats
    // Speed/Acceleration
    private float impulseAcceleration = 50f;
    private float warpAccelMultiplier = 3f;
    private float maxImpulseSpeed = 100f;
    private float maxWarpSpeed = 200f;
    // Weapon stats
    private float shotSpeed = 25f;
    private float shotLifetime = 2.5f;
    // Cooldowns
    private float shotCooldownTime = 0.25f;
    // Energy cost
    private float warpEnergyCost = 5f;
    private float shotEnergyCost = 5f;
    // Current/Max energy
    private float energy = 0f;
    private float maxEnergy = 100f;
    
    // Constuctor criteria
    private int id;

    // Alive flag
    public bool Alive = false;

    // Player ship constructor
    public PlayerShip(int _id)
    {
        this.id = _id;
        this.Start();
    }


    // Start is called before the first frame update
    public void Start()
    {
        this.playerPrefab = Resources.Load(GameController.PlayerPrefabName, typeof(GameObject)) as GameObject;
        this.Ship = GameObject.Instantiate(this.playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.Ship.name = $@"Player: {this.id}";
        this.shipRigidbody = this.Ship.GetComponent<Rigidbody>();
        this.impulseEngine = this.Ship.transform.Find("Impulse Engine").gameObject;
        this.warpEngine = this.Ship.transform.Find("Warp Engine").gameObject;
        this.gunBarrel = this.Ship.transform.Find("Gun Barrel").gameObject;
        this.gunBarrelLights = this.gunBarrel.transform.Find("Gun Barrel Lights").gameObject;
        this.shield = this.Ship.transform.Find("Shield").gameObject;
        this.scanner = this.Ship.transform.Find("Scanner").gameObject;
        this.impulseParticleSystem = this.impulseEngine.GetComponent<ParticleSystem>();
        this.impulseParticleMain = this.impulseParticleSystem.main;
        this.warpParticleSystem = this.warpEngine.GetComponent<ParticleSystem>();
        this.warpParticleMain = this.warpParticleSystem.main;
        this.impulseAudio = this.impulseEngine.GetComponent<AudioSource>();
        this.warpAudio = this.warpEngine.GetComponent<AudioSource>();
        this.gunAudio = this.gunBarrel.GetComponent<AudioSource>();
        this.recentRotations = new float[recentRotationsIndexMax];
        this.Alive = true;
        this.playerInput.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if(this.Alive)
        {
            this.playerInput.Update();
        }
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        if(this.Alive)
        {
            this.playerInput.FixedUpdate();
            this.UpdateShipState();
        }
    }

    // Updates the state of the ship, turning, accelerating, using weapons etc.
    private void UpdateShipState()
    {
        this.RotateShip();
        this.AccelerateShip();
        this.UseAbilities();
    }

    // Rotates the ship according to player input
    private void RotateShip()
    {
        this.TurnShip();
        this.LeanShip();
    }

    // Turns the ship
    private void TurnShip()
    {
        // if joystick is pointed in some direction, set a new intended rotation
        if(this.playerInput.horizontal != 0 || this.playerInput.vertical != 0)
        {
            this.intendedAngle = MathOps.Modulo(Mathf.Atan2(this.playerInput.vertical, this.playerInput.horizontal) * Mathf.Rad2Deg, 360);
            this.intendedRotation = new Vector3(0, this.intendedAngle, 0);
        }
        // get current rotation
        this.currentRotation = this.Ship.transform.rotation.eulerAngles;
        this.currentRotation.y += 360;
        // Check if the turn should be clockwise or anti-cw
        this.differenceAngle = MathOps.Modulo(this.currentRotation.y - this.intendedRotation.y, 360);
        if(this.differenceAngle > 180)
        {
            this.differenceAngle -= 180;
            this.intendedRotationClockwise = true;
        }
        else
        {
            this.intendedRotationClockwise = false;
        }
        // if turning angle is greater than max rotation speed allows, rotate by max rot speed
        if(this.differenceAngle >= this.maxRotationSpeed)
        {
            // if clockwise, add rotation speed
            if(this.intendedRotationClockwise == true)
            {
                this.nextRotation.y = MathOps.Modulo(this.currentRotation.y + this.maxRotationSpeed, 360);
                this.Ship.transform.rotation = Quaternion.Euler(this.nextRotation);
                this.recentRotations[this.recentRotationsIndex] = this.maxRotationSpeed;
            }
            // if anticlockwise, subtract rotation speed
            else
            {
                this.nextRotation.y = MathOps.Modulo(this.currentRotation.y - this.maxRotationSpeed, 360);
                this.Ship.transform.rotation = Quaternion.Euler(this.nextRotation);
                this.recentRotations[this.recentRotationsIndex] = -this.maxRotationSpeed;
            }
        }
        // if turning angle is less than max rotation speed limit, immediately rotate to intended rotation
        else if(this.differenceAngle < this.maxRotationSpeed && this.differenceAngle > 0)
        {
            this.Ship.transform.rotation = Quaternion.Euler(this.intendedRotation);
            // if
            if(this.currentRotation.y - this.intendedRotation.y - 180 < 0)
            {
                this.recentRotations[this.recentRotationsIndex] = 2.5f;
            }
            else
            {
                this.recentRotations[this.recentRotationsIndex] = -2.5f;
            }
        }
        else
        {
            this.recentRotations[this.recentRotationsIndex] = 0;
        }
        this.recentRotationsIndex++;
        if(this.recentRotationsIndex == recentRotationsIndexMax)
        {
            this.recentRotationsIndex = 0;
        }
    }

    // Lean the ship during turns
    private void LeanShip()
    {
        this.recentRotationsAverage = 0;
        for(int i = 0; i < recentRotationsIndexMax; i++)
        {
            this.recentRotationsAverage += this.recentRotations[i];
        }
        this.recentRotationsAverage /= recentRotationsIndexMax;
        this.currentRotation = this.Ship.transform.rotation.eulerAngles;
        this.tiltAngle = -(this.recentRotationsAverage * 5);
        this.tiltRotation = new Vector3(this.currentRotation.x, this.currentRotation.y, this.tiltAngle);
        this.Ship.transform.rotation = Quaternion.Euler(this.tiltRotation);
    }

    // Accelerates the ship
    private void AccelerateShip()
    {
        if(this.playerInput.impulse)
        {
            if(this.shipRigidbody.velocity.magnitude < this.maxImpulseSpeed)
            {
                this.shipRigidbody.AddRelativeForce(new Vector3(0, 0, this.impulseAcceleration));
                this.impulseParticleMain.startSpeed = 2.8f;
                this.warpParticleMain.startLifetime = 0f;
                this.impulseAudio.Play();
                this.warpAudio.Stop();
            }
        }
        else if(this.playerInput.warp)
        {
            if(this.shipRigidbody.velocity.magnitude < this.maxWarpSpeed)
            {
                this.shipRigidbody.AddRelativeForce(new Vector3(0, 0, this.impulseAcceleration * this.warpAccelMultiplier));
                this.impulseParticleMain.startSpeed = 5f;
                this.warpParticleMain.startSpeed = 10f;
                this.warpParticleMain.startLifetime = 1f;
                this.warpAudio.Play();
                this.impulseAudio.Stop();
            }
        }
        else
        {
            this.impulseParticleMain.startSpeed = 1f;
            this.warpParticleMain.startSpeed = 1f;
            this.warpParticleMain.startLifetime = 0f;
            this.impulseAudio.Stop();
            this.warpAudio.Stop();
        }
    }

    // Uses abilities: fire weapons, bombs, use shield and scanner
    private void UseAbilities()
    {
        this.FireMainGun();
        this.ActivateShields();
        this.ActivateScanner();
    }

    // Fires main guns
    private void FireMainGun()
    {
        if(Time.time - lastShotFireTime >= shotCooldownTime)
        {
            weaponOnCooldown = false;
        }
        if(this.playerInput.fire)
        {
            if(!weaponOnCooldown)
            {
                this.gunBarrelLights.SetActive(true);
                GameController.SpawnProjectile(GameController.IFF.friend, this.gunBarrel.transform.position, this.gunBarrel.transform.rotation, this.shipRigidbody.velocity, this.shotSpeed, this.shotLifetime);
                this.lastShotFireTime = Time.time;
                weaponOnCooldown = true;
                this.gunAudio.Play();
            }
            else
            {
                this.gunBarrelLights.SetActive(false);
                this.gunAudio.Stop();
            }
        }
        else
        {
            this.gunBarrelLights.SetActive(false);
            this.gunAudio.Stop();
        }
    }

    // Activates shields
    private void ActivateShields()
    {
        if(this.playerInput.shield)
        {
            this.shield.SetActive(true);
        }
        else
        {
            this.shield.SetActive(false);
        }
    }

    // Activates scanner
    private void ActivateScanner()
    {
        if(this.playerInput.scanner)
        {
            this.scanner.SetActive(true);
        }
        else
        {
            this.scanner.SetActive(false);
        }
    }
}
