using UnityEngine;

// Controls the player ships
public class PlayerShip : IEntity
{
    // Input script intialization
    private PlayerInput playerInput = new PlayerInput();

    // GameObjects and Components
    public GameObject ShipObject { get; set; }
    private GameObject playerPrefab;
    private Rigidbody shipRigidbody;
    private GameObject impulseEngineObject;
    private GameObject warpEngineObject;
    private GameObject gunBarrelObject;
    private GameObject gunBarrelLightsObject;
    private GameObject shieldObject;
    private GameObject scannerObject;
    private ParticleSystem impulseParticleSystem;
    private ParticleSystem warpParticleSystem;
    private ParticleSystem.MainModule impulseParticleMain;
    private ParticleSystem.MainModule warpParticleMain;
    private AudioSource impulseAudio;
    private AudioSource warpAudio;
    private AudioSource gunAudio;

    // Fields not modified by player equipment/level
    private Vector3 intendedRotation;
    private Vector3 currentRotation;
    private Vector3 nextRotation;
    private Vector3 tiltRotation;
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
    private float impulseEngineAudioStep = 0.05f;
    private float impulseEngineAudioMinVol = 0.1f;
    private float impulseEngineAudioMaxVol = 0.5f;
    private float warpEngineAudioStep = 0.05f;
    private float warpEngineAudioMinVol = 0f;
    private float warpEngineAudioMaxVol = 1f;

    // Ship stats
    // Health/Armor/Shields
    private float health = 0f;
    private float maxHealth = 100f;
    private float armor = 0f;
    private float maxArmor = 100f;
    private float shields = 0f;
    private float maxShields = 100f;
    // Current/Max energy
    private float energy = 0f;
    private float maxEnergy = 100f;
    // Speed/Acceleration
    private float impulseAcceleration = 25f;
    private float warpAccelMultiplier = 3f;
    private float maxImpulseSpeed = 50f;
    private float maxWarpSpeed = 150f;
    // Weapon stats
    private float shotDamage = 10f;
    private float shotSpeed = 10f;
    private float shotLifetime = 2.5f;
    private float shotCurvature = 0f;
    // Cooldowns
    private float shotCooldownTime = 0.25f;
    private float shieldCooldownTime = 10f;
    private float bombCooldownTime = 10f;
    private float scannerCooldownTime = 10f;
    // Energy cost
    private float warpEnergyCost = 5f;
    private float shotEnergyCost = 5f;

    // Entity Interface fields
    public uint ID { get; set; }
    public GameController.IFF IFF { get; set; }
    public bool Alive { get; set; }

    // Player ship constructor
    public PlayerShip(uint _id)
    {
        this.ID = _id;
        this.IFF = GameController.IFF.friend;
        this.Start();
    }


    // Start is called before the first frame update
    public void Start()
    {
        this.playerPrefab = Resources.Load(GameController.PlayerPrefabName, typeof(GameObject)) as GameObject;
        this.ShipObject = GameObject.Instantiate(this.playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.ShipObject.name = $@"{this.ID}";
        this.shipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.impulseEngineObject = this.ShipObject.transform.Find("Impulse Engine").gameObject;
        this.warpEngineObject = this.ShipObject.transform.Find("Warp Engine").gameObject;
        this.gunBarrelObject = this.ShipObject.transform.Find("Gun Barrel").gameObject;
        this.gunBarrelLightsObject = this.gunBarrelObject.transform.Find("Gun Barrel Lights").gameObject;
        this.shieldObject = this.ShipObject.transform.Find("Shield").gameObject;
        this.scannerObject = this.ShipObject.transform.Find("Scanner").gameObject;
        this.impulseParticleSystem = this.impulseEngineObject.GetComponent<ParticleSystem>();
        this.impulseParticleMain = this.impulseParticleSystem.main;
        this.warpParticleSystem = this.warpEngineObject.GetComponent<ParticleSystem>();
        this.warpParticleMain = this.warpParticleSystem.main;
        this.impulseAudio = this.impulseEngineObject.GetComponent<AudioSource>();
        this.warpAudio = this.warpEngineObject.GetComponent<AudioSource>();
        this.gunAudio = this.gunBarrelObject.GetComponent<AudioSource>();
        this.recentRotations = new float[recentRotationsIndexMax];
        this.Alive = true;
        this.health = this.maxHealth;
        this.armor = this.maxArmor;
        this.shields = this.maxShields;
        this.energy = this.maxEnergy;
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
        if(this.health <= 0)
        {
            this.Kill();
        }
    }

    // Called when receiving collision
    public void ReceivedCollision(float _damage)
    {
        this.health -= _damage;
    }

    // Called when entity is destroyed
    private void Kill()
    {
        this.Alive = false;
        GameObject.Destroy(this.ShipObject);
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
        this.currentRotation = this.ShipObject.transform.rotation.eulerAngles;
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
                this.ShipObject.transform.rotation = Quaternion.Euler(this.nextRotation);
                this.recentRotations[this.recentRotationsIndex] = this.maxRotationSpeed;
            }
            // if anticlockwise, subtract rotation speed
            else
            {
                this.nextRotation.y = MathOps.Modulo(this.currentRotation.y - this.maxRotationSpeed, 360);
                this.ShipObject.transform.rotation = Quaternion.Euler(this.nextRotation);
                this.recentRotations[this.recentRotationsIndex] = -this.maxRotationSpeed;
            }
        }
        // if turning angle is less than max rotation speed limit, immediately rotate to intended rotation
        else if(this.differenceAngle < this.maxRotationSpeed && this.differenceAngle > 0)
        {
            this.ShipObject.transform.rotation = Quaternion.Euler(this.intendedRotation);
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
        this.currentRotation = this.ShipObject.transform.rotation.eulerAngles;
        this.tiltAngle = -(this.recentRotationsAverage * 5);
        this.tiltRotation = new Vector3(this.currentRotation.x, this.currentRotation.y, this.tiltAngle);
        this.ShipObject.transform.rotation = Quaternion.Euler(this.tiltRotation);
    }

    // Accelerates the ship
    private void AccelerateShip()
    {
        if(this.playerInput.impulse && !this.playerInput.warp)
        {
            if(this.shipRigidbody.velocity.magnitude < this.maxImpulseSpeed)
            {
                this.shipRigidbody.AddRelativeForce(new Vector3(0, 0, this.impulseAcceleration));
                this.impulseParticleMain.startSpeed = 2.8f;
                this.warpParticleMain.startLifetime = 0f;
                AudioController.FadeIn(this.impulseAudio, this.impulseEngineAudioStep, this.impulseEngineAudioMaxVol);
                AudioController.FadeOut(this.warpAudio, this.warpEngineAudioStep, this.warpEngineAudioMinVol);
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
                AudioController.FadeIn(this.warpAudio, this.warpEngineAudioStep, this.warpEngineAudioMaxVol);
                AudioController.FadeOut(this.impulseAudio, this.impulseEngineAudioStep, this.impulseEngineAudioMinVol);
            }
        }
        else
        {
            this.impulseParticleMain.startSpeed = 1f;
            this.warpParticleMain.startSpeed = 1f;
            this.warpParticleMain.startLifetime = 0f;
            AudioController.FadeOut(this.impulseAudio, this.impulseEngineAudioStep, this.impulseEngineAudioMinVol);
            AudioController.FadeOut(this.warpAudio, this.warpEngineAudioStep, this.warpEngineAudioMinVol);
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
        if(Time.time - this.lastShotFireTime >= this.shotCooldownTime)
        {
            this.weaponOnCooldown = false;
        }
        if(this.playerInput.fire)
        {
            if(!this.weaponOnCooldown)
            {
                this.gunBarrelLightsObject.SetActive(true);
                GameController.SpawnProjectile(GameController.IFF.friend, this.shotDamage, this.gunBarrelObject.transform.position, this.gunBarrelObject.transform.rotation, this.shipRigidbody.velocity, this.shotSpeed, this.shotLifetime);
                this.lastShotFireTime = Time.time;
                this.weaponOnCooldown = true;
                this.gunAudio.Play();
            }
            else
            {
                this.gunBarrelLightsObject.SetActive(false);
            }
        }
        else
        {
            this.gunBarrelLightsObject.SetActive(false);
        }
    }

    // Activates shields
    private void ActivateShields()
    {
        if(this.playerInput.shield)
        {
            this.shieldObject.SetActive(true);
        }
        else
        {
            this.shieldObject.SetActive(false);
        }
    }

    // Activates scanner
    private void ActivateScanner()
    {
        if(this.playerInput.scanner)
        {
            this.scannerObject.SetActive(true);
        }
        else
        {
            this.scannerObject.SetActive(false);
        }
    }
}
