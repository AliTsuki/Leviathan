using UnityEngine;

// Player Input class reads and stores inputs from Player
public class PlayerInput : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float impulse;
    public float warp;
    public bool fire;
    public bool bomb;
    public bool shield;
    public bool scanner;
    public bool pause;
    public bool readyToClear;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        ClearInput();
        ProcessInputs();
    }

    // Fixed Update is called a fixed number of times per second
    private void FixedUpdate()
    {
        readyToClear = true;
    }

    // Clears the inputs to default state
    private void ClearInput()
    {
        if(!readyToClear)
            return;
        horizontal = 0f;
        vertical = 0f;
        impulse = 0f;
        warp = 0f;
        fire = false;
        bomb = false;
        shield = false;
        scanner = false;
        pause = false;
        readyToClear = false;
    }

    // Reads the inputs and stores them
    private void ProcessInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        impulse = Input.GetAxis("Impulse");
        warp = Input.GetAxis("Warp");
        fire = Input.GetButton("Fire");
        bomb = Input.GetButton("Bomb");
        shield = Input.GetButton("Shield");
        scanner = Input.GetButton("Scanner");
        pause = Input.GetButton("Pause");
    }
}
