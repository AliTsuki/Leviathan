using UnityEngine;

// Reads and stores inputs from player
public static class PlayerInput
{
    // Controller layout types
    public enum ControllerType
    {
        GenericGamepad,
        XboxController,
        PSController
    }
    public static ControllerType Controller;

    // Inputs
    public static Vector2 AimInput = new Vector2();
    public static float ImpulseEngineInput;
    public static float WarpEngineInput;
    public static bool MainGunInput;
    public static bool Ability1Input;
    public static bool Ability2Input;
    public static bool Ability3Input;
    public static bool PauseButtonInput;

    // Update is called once per frame
    public static void Update()
    {
        ProcessInputs();
    }

    // Reads the inputs and stores them
    private static void ProcessInputs()
    {
        if(Controller == ControllerType.GenericGamepad)
        {
            AimInput.Set(Input.GetAxis("Left Stick Horizontal"), Input.GetAxis("Left Stick Vertical"));
            ImpulseEngineInput = Input.GetButton("Button 7") == true ? 1f : 0f;
            WarpEngineInput = Input.GetButton("Button 6") == true ? 1f : 0f;
            MainGunInput = Input.GetButton("Button 1");
            Ability1Input = Input.GetButton("Button 2");
            Ability2Input = Input.GetButton("Button 0");
            Ability3Input = Input.GetButton("Button 3");
            PauseButtonInput = Input.GetButtonDown("Button 9");
        }
        else if(Controller == ControllerType.XboxController)
        {
            AimInput.Set(Input.GetAxis("Left Stick Horizontal"), Input.GetAxis("Left Stick Vertical"));
            ImpulseEngineInput = Input.GetAxis("Right Trigger Xbox");
            WarpEngineInput = Input.GetAxis("Left Trigger Xbox");
            MainGunInput = Input.GetButton("Button 0");
            Ability1Input = Input.GetButton("Button 1");
            Ability2Input = Input.GetButton("Button 2");
            Ability3Input = Input.GetButton("Button 3");
            PauseButtonInput = Input.GetButtonDown("Button 7");
        }
        else if(Controller == ControllerType.PSController)
        {

        }
    }
}
