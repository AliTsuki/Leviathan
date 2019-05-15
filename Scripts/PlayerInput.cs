using UnityEngine;

// Reads and stores inputs from player
public class PlayerInput
{
    // Inputs
    public float Horizontal;
    public float Vertical;
    public bool Impulse;
    public bool Warp;
    public bool Fire;
    public bool Bomb;
    public bool Shield;
    public bool Scanner;
    public bool Pause;

    // Update is called once per frame
    public void Update()
    {
        this.ClearInput();
        this.ProcessInputs();
    }

    // Clears the inputs to default state
    private void ClearInput()
    {
        this.Horizontal = 0f;
        this.Vertical = 0f;
        this.Impulse = false;
        this.Warp = false;
        this.Fire = false;
        this.Bomb = false;
        this.Shield = false;
        this.Scanner = false;
        this.Pause = false;
    }

    // Reads the inputs and stores them
    private void ProcessInputs()
    {
        this.Horizontal = Input.GetAxis("Horizontal");
        this.Vertical = Input.GetAxis("Vertical");
        this.Impulse = Input.GetButton("Impulse");
        this.Warp = Input.GetButton("Warp");
        this.Fire = Input.GetButton("Fire");
        this.Bomb = Input.GetButton("Bomb");
        this.Shield = Input.GetButton("Shield");
        this.Scanner = Input.GetButton("Scanner");
        this.Pause = Input.GetButton("Pause");
    }
}
