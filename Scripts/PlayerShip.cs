using UnityEngine;

// Controls the player ships
public class PlayerShip //: Ship
{
    private static GameController controller = GameManager.instance;

    public GameObject ship;
    private GameObject PlayerParent;
    private GameObject PlayerPrefab;
    private PlayerInput playerInput;
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
    private float energy = 0f;

    private float impulseAcceleration = 30f;
    private float warpAccelMultiplier = 3f;
    private float maxImpulseSpeed = 100f;
    private float maxWarpSpeed = 200f;
    private float warpEnergyCost = 5f;
    private float shotEnergyCost = 5f;
    private float shotCooldownTime = 1f;
    private float shotSpeed = 50f;
    private float shotLifetime = 2.5f;
    private float maxEnergy = 100f;
    
    private int ID;

    public PlayerShip(int _id)
    {
        this.ID = _id;
    }


    // Start is called before the first frame update
    public void Start()
    {
        PlayerParent = GameObject.Find(controller.PlayerParentName);
        PlayerPrefab = Resources.Load<GameObject>(controller.PlayerPrefabName);
        ship = GameObject.Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        ship.name = $@"Player: {ID}";
        ship.transform.parent = PlayerParent.transform;
        playerInput = ship.GetComponent<PlayerInput>();
        shipRigidbody = ship.GetComponent<Rigidbody>();
        impulseEngine = ship.transform.Find("Impulse Engine").gameObject;
        warpEngine = ship.transform.Find("Warp Engine").gameObject;
        gunBarrel = ship.transform.Find("Gun Barrel").gameObject;
        gunBarrelLights = ship.transform.Find("Gun Barrel Lights").gameObject;
        shield = ship.transform.Find("Shield").gameObject;
        scanner = ship.transform.Find("Scanner").gameObject;
        impulseParticleSystem = impulseEngine.GetComponent<ParticleSystem>();
        impulseParticleMain = impulseParticleSystem.main;
        warpParticleSystem = warpEngine.GetComponent<ParticleSystem>();
        warpParticleMain = warpParticleSystem.main;
        recentRotations = new float[recentRotationsIndexMax];

        playerInput.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        playerInput.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        playerInput.FixedUpdate();

        UpdateShipState();
    }

    // Updates the state of the ship, turning, accelerating, using weapons etc.
    private void UpdateShipState()
    {
        RotateShip();
        AccelerateShip();
        UseAbilities();
    }

    // Rotates the ship according to player input
    private void RotateShip()
    {
        TurnShip();
        LeanShip();
    }

    // Turns the ship
    private void TurnShip()
    {
        // if joystick is pointed in some direction, set a new intended rotation
        if(playerInput.horizontal != 0 || playerInput.vertical != 0)
        {
            intendedAngle = MathOps.Modulo((Mathf.Atan2(playerInput.vertical, playerInput.horizontal) * Mathf.Rad2Deg), 360);
            intendedRotation = new Vector3(0, intendedAngle, 0);
        }
        // get current rotation
        currentRotation = ship.transform.rotation.eulerAngles;
        currentRotation.y += 360;
        // Check if the turn should be clockwise or anti-cw
        differenceAngle = MathOps.Modulo((currentRotation.y - intendedRotation.y), 360);
        if(differenceAngle > 180)
        {
            differenceAngle -= 180;
            intendedRotationClockwise = true;
        }
        else
        {
            intendedRotationClockwise = false;
        }
        // if turning angle is greater than max rotation speed allows, rotate by max rot speed
        if(differenceAngle >= maxRotationSpeed)
        {
            // if clockwise, add rotation speed
            if(intendedRotationClockwise == true)
            {
                nextRotation.y = MathOps.Modulo((currentRotation.y + maxRotationSpeed), 360);
                ship.transform.rotation = Quaternion.Euler(nextRotation);
                recentRotations[recentRotationsIndex] = maxRotationSpeed;
            }
            // if anticlockwise, subtract rotation speed
            else
            {
                nextRotation.y = MathOps.Modulo((currentRotation.y - maxRotationSpeed), 360);
                ship.transform.rotation = Quaternion.Euler(nextRotation);
                recentRotations[recentRotationsIndex] = -maxRotationSpeed;
            }
        }
        // if turning angle is less than max rotation speed limit, immediately rotate to intended rotation
        else if(differenceAngle < maxRotationSpeed && differenceAngle > 0)
        {
            ship.transform.rotation = Quaternion.Euler(intendedRotation);
            // if
            if(currentRotation.y - intendedRotation.y - 180 < 0)
            {
                recentRotations[recentRotationsIndex] = 2.5f;
            }
            else
            {
                recentRotations[recentRotationsIndex] = -2.5f;
            }
        }
        else
        {
            recentRotations[recentRotationsIndex] = 0;
        }
        recentRotationsIndex++;
        if(recentRotationsIndex == recentRotationsIndexMax)
        {
            recentRotationsIndex = 0;
        }
    }

    // Lean the ship during turns
    private void LeanShip()
    {
        recentRotationsAverage = 0;
        for(int i = 0; i < recentRotationsIndexMax; i++)
        {
            recentRotationsAverage += recentRotations[i];
        }
        recentRotationsAverage /= recentRotationsIndexMax;
        currentRotation = ship.transform.rotation.eulerAngles;
        tiltAngle = -(recentRotationsAverage * 5);
        tiltRotation = new Vector3(currentRotation.x, currentRotation.y, tiltAngle);
        ship.transform.rotation = Quaternion.Euler(tiltRotation);
    }

    // Accelerates the ship
    private void AccelerateShip()
    {
        if(playerInput.impulse)
        {
            if(shipRigidbody.velocity.magnitude < maxImpulseSpeed)
            {
                shipRigidbody.AddRelativeForce(new Vector3(0, 0, impulseAcceleration));
                impulseParticleMain.startSpeed = 2.8f;
                warpParticleMain.startLifetime = 0f;
            }
        }
        else if(playerInput.warp)
        {
            if(shipRigidbody.velocity.magnitude < maxWarpSpeed)
            {
                shipRigidbody.AddRelativeForce(new Vector3(0, 0, impulseAcceleration * warpAccelMultiplier));
                impulseParticleMain.startSpeed = 5f;
                warpParticleMain.startSpeed = 10f;
                warpParticleMain.startLifetime = 1f;
            }
        }
        else
        {
            impulseParticleMain.startSpeed = 1f;
            warpParticleMain.startSpeed = 1f;
            warpParticleMain.startLifetime = 0f;
        }
    }

    // Uses abilities: fire weapons, bombs, use shield and scanner
    private void UseAbilities()
    {
        FireMainGun();
        ActivateShields();
        ActivateScanner();
    }

    // Fires main guns
    private void FireMainGun()
    {
        if(playerInput.fire)
        {
            gunBarrelLights.SetActive(true);
            controller.SpawnProjectile(gunBarrel.transform.position, gunBarrel.transform.rotation, shotSpeed, shotLifetime);
        }
        else
        {
            gunBarrelLights.SetActive(false);
        }
    }

    // Activates shields
    private void ActivateShields()
    {
        if(playerInput.shield)
        {
            shield.SetActive(true);
        }
        else
        {
            shield.SetActive(false);
        }
    }

    // Activates scanner
    private void ActivateScanner()
    {
        if(playerInput.scanner)
        {
            scanner.SetActive(true);
        }
        else
        {
            scanner.SetActive(false);
        }
    }
}
