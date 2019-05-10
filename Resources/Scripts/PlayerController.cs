using UnityEngine;

// Player Controller class takes stored Player Inputs and applies them to the Player controlled ship
public class PlayerController : MonoBehaviour
{
    public GameObject ship;
    private PlayerInput playerInput;
    private Rigidbody shipRigidbody;
    public GameObject impulseEngine;
    public GameObject warpEngine;
    private ParticleSystem impulseParticleSystem;
    private ParticleSystem warpParticleSystem;
    private ParticleSystem.MainModule impulseParticleMain;
    private ParticleSystem.MainModule warpParticleMain;
    public Vector3 intendedRotation;
    public Vector3 currentRotation;
    public Vector3 nextRotation;
    public Vector3 tiltRotation;
    private bool intendedRotationClockwise = false;
    public float differenceAngle = 0f;
    private float intendedAngle = 0f;
    private float maxRotationSpeed = 5f;
    private float impulseMaxSpeed = 15f;
    private float warpSpeedMultiplier = 3f;
    private float warpEnergyCost = 5f;
    public float energy = 100f;
    private int recentRotationsIndex = 0;
    private int recentRotationsIndexMax = 60;
    private float[] recentRotations;
    public float recentRotationsAverage = 0f;
    public float tiltAngle = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        playerInput = ship.GetComponent<PlayerInput>();
        shipRigidbody = ship.GetComponent<Rigidbody>();
        impulseParticleSystem = impulseEngine.GetComponent<ParticleSystem>();
        impulseParticleMain = impulseParticleSystem.main;
        warpParticleSystem = warpEngine.GetComponent<ParticleSystem>();
        warpParticleMain = warpParticleSystem.main;
        recentRotations = new float[recentRotationsIndexMax];
    }

    // Update is called once per frame
    private void Update()
    {

    }

    // Fixed Update is called a fixed number of times per second
    private void FixedUpdate()
    {
        UpdateShipState();
    }

    // Updates the state of the ship, turning, accelerating, using weapons etc.
    private void UpdateShipState()
    {
        TurnShip();
        LeanShip();
        AccelerateShip();
        UseAbilities();
    }

    // Turns the ship
    private void TurnShip()
    {
        if(playerInput.horizontal != 0 || playerInput.vertical != 0)
        {
            intendedAngle = MathOps.Modulo((Mathf.Atan2(playerInput.vertical, playerInput.horizontal) * Mathf.Rad2Deg), 360);
            intendedRotation = new Vector3(0, intendedAngle, 0);
        }
        currentRotation = ship.transform.rotation.eulerAngles;
        currentRotation.y += 360;
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
        if(differenceAngle >= maxRotationSpeed)
        {
            if(intendedRotationClockwise == true)
            {
                nextRotation.y = MathOps.Modulo((currentRotation.y + maxRotationSpeed), 360);
                ship.transform.rotation = Quaternion.Euler(nextRotation);
                recentRotations[recentRotationsIndex] = maxRotationSpeed;
            }
            else
            {
                nextRotation.y = MathOps.Modulo((currentRotation.y - maxRotationSpeed), 360);
                ship.transform.rotation = Quaternion.Euler(nextRotation);
                recentRotations[recentRotationsIndex] = -maxRotationSpeed;
            }
        }
        else if(differenceAngle < maxRotationSpeed && differenceAngle > 0)
        {
            ship.transform.rotation = Quaternion.Euler(intendedRotation);
            if(currentRotation.y - intendedRotation.y - 180 < 0)
            {
                recentRotations[recentRotationsIndex] = -2.5f;
            }
            else
            {
                recentRotations[recentRotationsIndex] = 2.5f;
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
            shipRigidbody.AddRelativeForce(new Vector3(0, 0, impulseMaxSpeed));
            impulseParticleMain.startSpeed = 2.8f;
            warpParticleMain.startLifetime = 0f;
        }
        else if(playerInput.warp)
        {
            shipRigidbody.AddRelativeForce(new Vector3(0, 0, impulseMaxSpeed * warpSpeedMultiplier));
            impulseParticleMain.startSpeed = 5f;
            warpParticleMain.startSpeed = 10f;
            warpParticleMain.startLifetime = 1f;
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

    }
}
