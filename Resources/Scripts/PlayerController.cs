using UnityEngine;

// Player Controller class takes stored Player Inputs and applies them to the Player controlled ship
public class PlayerController : MonoBehaviour
{
    public GameObject ship;
    private PlayerInput playerInput;
    public Vector3 intendedRotation;
    public Vector3 currentRotation;
    public Vector3 nextRotation;
    public float intendedAngle = 0f;
    public float maxRotationSpeed = 5f;
    public float currentSpeed = 0f;
    public float impulseSpeed = 1f;
    public float warpMulti = 2f;
    public float energy = 100f;

    // Start is called before the first frame update
    private void Start()
    {
         playerInput = ship.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    // Fixed Update is called a fixed number of times per second
    private void FixedUpdate()
    {
        ApplyInput();
    }

    private void ApplyInput()
    {
        // if stick is being pushed in any direction, get the intended angle to rotate toward
        if(playerInput.horizontal != 0 || playerInput.vertical != 0)
        {
            intendedAngle = MathOps.Modulo((Mathf.Atan2(playerInput.vertical, playerInput.horizontal) * Mathf.Rad2Deg), 360);
            intendedRotation = new Vector3(0, intendedAngle, 0);
            // the intended rotation is stored as 0°-359.99° where 0 is right, 90 is down, 180 is left, 270 is up
        }
        // get the current rotation of the ship
        currentRotation = ship.transform.rotation.eulerAngles;
        // if the absolute value of the current rotation minus the intended rotation is greater than the maximum rotation speed
        // then rotate by the maximum rotation speed in the direction of the intended rotation
        // TODO: something below is behaving incorrectly over the 0°/360° barrier
        if(Mathf.Abs(currentRotation.y - intendedRotation.y) >= maxRotationSpeed)
        {
            // if the difference in current rotation and intended rotation is NEGATIVE then advance clockwise toward the intended rotation
            // in increments of maximum rotation speed
            if(currentRotation.y - intendedRotation.y < 0)
            {
                nextRotation.y = MathOps.Modulo((currentRotation.y + maxRotationSpeed), 360);
                ship.transform.rotation = Quaternion.Euler(nextRotation);
            }
            // otherwise, if the difference is POSITIVE then advance counterclockwise toward the intended rotation
            // in increments of maximum rotation speed
            else if(currentRotation.y - intendedRotation.y >= 0)
            {
                nextRotation.y = MathOps.Modulo((currentRotation.y - maxRotationSpeed), 360);
                ship.transform.rotation = Quaternion.Euler(nextRotation);
            }
        }
        // if the intended rotation is within distance of the maximum rotation from the current rotation then just apply the intended rotation
        else
        {
            ship.transform.rotation = Quaternion.Euler(intendedRotation);
        }
    }
}
