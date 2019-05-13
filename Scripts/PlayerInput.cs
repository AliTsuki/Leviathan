using UnityEngine;

// Reads and stores inputs from player
public class PlayerInput
{
    // Fields
    public float horizontal;
    public float vertical;
    public bool impulse;
    public bool warp;
    public bool fire;
    public bool bomb;
    public bool shield;
    public bool scanner;
    public bool pause;
    public bool readyToClear;

    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        this.ClearInput();
        this.ProcessInputs();
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        this.readyToClear = true;
    }

    // Clears the inputs to default state
    private void ClearInput()
    {
        if(!this.readyToClear)
        {
            return;
        }
        this.horizontal = 0f;
        this.vertical = 0f;
        this.impulse = false;
        this.warp = false;
        this.fire = false;
        this.bomb = false;
        this.shield = false;
        this.scanner = false;
        this.pause = false;
        this.readyToClear = false;
    }

    // Reads the inputs and stores them
    private void ProcessInputs()
    {
        this.horizontal = Input.GetAxis("Horizontal");
        this.vertical = Input.GetAxis("Vertical");
        this.impulse = Input.GetButton("Impulse");
        this.warp = Input.GetButton("Warp");
        this.fire = Input.GetButton("Fire");
        this.bomb = Input.GetButton("Bomb");
        this.shield = Input.GetButton("Shield");
        this.scanner = Input.GetButton("Scanner");
        this.pause = Input.GetButton("Pause");
    }
}
