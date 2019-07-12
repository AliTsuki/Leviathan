using System.Collections.Generic;

using UnityEngine;

// Reads and stores inputs from player
public static class PlayerInput
{
    // Inputs
    public static Float2 AimInput { get; private set; } = new Float2();
    public static float ImpulseEngineInput { get; private set; }
    public static float WarpEngineInput { get; private set; }
    public static bool MainGunInput { get; private set; }
    public static bool Ability1Input { get; private set; }
    public static bool Ability2Input { get; private set; }
    public static bool Ability3Input { get; private set; }
    public static bool PauseButtonInput { get; private set; }

    // Input bindings
    public static Dictionary<string, InputBinding> InputBindings { get; private set; } = new Dictionary<string, InputBinding>
    {
        {"Submit", new InputBinding("Submit", InputBinding.InputTypeEnum.Button, "Button 0")},
        {"Cancel", new InputBinding("Cancel", InputBinding.InputTypeEnum.Button, "Button 1")},
        {"Pause Input", new InputBinding("Pause Input", InputBinding.InputTypeEnum.Button, "Button 7")},
        {"Main Gun Input", new InputBinding("Main Gun Input", InputBinding.InputTypeEnum.Button, "Button 0")},
        {"Ability 1 Input", new InputBinding("Ability 1 Input", InputBinding.InputTypeEnum.Button, "Button 2")},
        {"Ability 2 Input", new InputBinding("Ability 2 Input", InputBinding.InputTypeEnum.Button, "Button 3")},
        {"Ability 3 Input", new InputBinding("Ability 3 Input", InputBinding.InputTypeEnum.Button, "Button 1")},
        {"Impulse Engine Input", new InputBinding("Impulse Engine Input", InputBinding.InputTypeEnum.Trigger, "Right Trigger")},
        {"Warp Engine Input", new InputBinding("Warp Engine Input", InputBinding.InputTypeEnum.Trigger, "Left Trigger")},
        {"Aim Input Horizontal", new InputBinding("Aim Input Horizontal", InputBinding.InputTypeEnum.Axis, "Left Stick Horizontal")},
        {"Aim Input Vertical", new InputBinding("Aim Input Vertical", InputBinding.InputTypeEnum.Axis, "Left Stick Vertical")},
    };

    // Rebinding input fields
    public static bool RebindingInputs = false;
    public static string InputToRebind = "";
    public static float RebindingStartedTime = 0f;
    private const float MaxRebindingInputDuration = 5f;

    // Update is called once per frame
    public static void Update()
    {
        if(RebindingInputs == true)
        {
            Rebind();
        }
        else
        {
            ProcessInputs();
        }
    }

    // Rebind input
    private static void Rebind()
    {
        if(Time.time - RebindingStartedTime >= MaxRebindingInputDuration)
        {
            RebindingInputs = false;
            UIController.UpdateSettingsErrorText($@"Input Rebinding has timed out");
            Logger.Log($@"Input Rebinding has timed out");
        }
        else if(IsAnyButtonPressed() == true)
        {
            Logger.Log($@"Input detected: {GetPressedButton()}");
            UIController.UpdateSettingsErrorText($@"Input detected: {GetPressedButton()}");
            InputBindings[InputToRebind].UpdateInputButton(GetPressedButton());
            RebindingInputs = false;
            Logger.Log($@"Input binding for {InputBindings[InputToRebind].InputName} is currently {InputBindings[InputToRebind].InputButton}");
        }
        else if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            RebindingInputs = false;
            UIController.UpdateSettingsErrorText($@"Input Rebinding has been cancelled");
            Logger.Log($@"Input Rebinding has been cancelled");
        }
        else
        {
            UIController.UpdateSettingsErrorText($@"Press Input you wish to rebind to {InputBindings[InputToRebind].InputName}. Press Esc to cancel. Cancelling in 5 seconds...");
        }
    }

    // Is any button pressed
    private static bool IsAnyButtonPressed()
    {
        if(Input.GetButton("Button 0") == true 
            || Input.GetButtonDown("Button 1") == true 
            || Input.GetButtonDown("Button 2") == true 
            || Input.GetButtonDown("Button 3") == true 
            || Input.GetButtonDown("Button 4") == true 
            || Input.GetButtonDown("Button 5") == true 
            || Input.GetButtonDown("Button 6") == true 
            || Input.GetButtonDown("Button 7") == true 
            || Input.GetButtonDown("Button 8") == true 
            || Input.GetButtonDown("Button 9") == true
            || Input.GetButtonDown("Button 10") == true
            || Input.GetButtonDown("Button 11") == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Get pressed key
    private static string GetPressedButton()
    {
        if(Input.GetButtonDown("Button 0") == true)
        {
            return "Button 0";
        }
        else if(Input.GetButtonDown("Button 1") == true)
        {
            return "Button 1";
        }
        else if(Input.GetButtonDown("Button 2") == true)
        {
            return "Button 2";
        }
        else if(Input.GetButtonDown("Button 3") == true)
        {
            return "Button 3";
        }
        else if(Input.GetButtonDown("Button 4") == true)
        {
            return "Button 4";
        }
        else if(Input.GetButtonDown("Button 5") == true)
        {
            return "Button 5";
        }
        else if(Input.GetButtonDown("Button 6") == true)
        {
            return "Button 6";
        }
        else if(Input.GetButtonDown("Button 7") == true)
        {
            return "Button 7";
        }
        else if(Input.GetButtonDown("Button 8") == true)
        {
            return "Button 8";
        }
        else if(Input.GetButtonDown("Button 9") == true)
        {
            return "Button 9";
        }
        else if(Input.GetButtonDown("Button 10") == true)
        {
            return "Button 10";
        }
        else if(Input.GetButtonDown("Button 11") == true)
        {
            return "Button 11";
        }
        return "";
    }

    // Reads the inputs and stores them
    private static void ProcessInputs()
    {
        //if()
        //{
        //    AimInput.Set(Input.GetAxis("Left Stick Horizontal"), Input.GetAxis("Left Stick Vertical"));
        //    ImpulseEngineInput = Input.GetButton("Button 7") == true ? 1f : 0f;
        //    WarpEngineInput = Input.GetButton("Button 6") == true ? 1f : 0f;
        //    MainGunInput = Input.GetButton("Button 1");
        //    Ability1Input = Input.GetButton("Button 0");
        //    Ability2Input = Input.GetButton("Button 3");
        //    Ability3Input = Input.GetButton("Button 2");
        //    PauseButtonInput = Input.GetButtonDown("Button 9");
        //}
        AimInput.x = Input.GetAxis(InputBindings["Aim Input Horizontal"].InputButton);
        AimInput.y = Input.GetAxis(InputBindings["Aim Input Vertical"].InputButton);
        ImpulseEngineInput = Input.GetAxis(InputBindings["Impulse Engine Input"].InputButton);
        WarpEngineInput = Input.GetAxis(InputBindings["Warp Engine Input"].InputButton);
        MainGunInput = Input.GetButton(InputBindings["Main Gun Input"].InputButton);
        Ability1Input = Input.GetButton(InputBindings["Ability 1 Input"].InputButton);
        Ability2Input = Input.GetButton(InputBindings["Ability 2 Input"].InputButton);
        Ability3Input = Input.GetButton(InputBindings["Ability 3 Input"].InputButton);
        PauseButtonInput = Input.GetButtonDown(InputBindings["Pause Input"].InputButton);
    }

    // Zero out inputs
    public static void ZeroInputs()
    {
        ImpulseEngineInput = 0f;
        WarpEngineInput = 0f;
        MainGunInput = false;
        Ability1Input = false;
        Ability2Input = false;
        Ability3Input = false;
        PauseButtonInput = false;
    }
}

// Container for input bindings
public class InputBinding
{
    // Fields
    public enum InputTypeEnum
    {
        Button,
        Trigger,
        Axis
    }
    public string InputName { get; private set; }
    public InputTypeEnum InputType { get; private set; }
    public string InputButton { get; private set; }

    // Constructor
    public InputBinding(string _inputName, InputTypeEnum _inputType, string _inputButton)
    {
        this.InputName = _inputName;
        this.InputType = _inputType;
        this.InputButton = _inputButton;
    }

    // Update button method
    public void UpdateInputButton(string _newInputButton)
    {
        Logger.Log($@"Updating Input for {this.InputName} from {this.InputButton} to {_newInputButton}");
        this.InputButton = _newInputButton;
        Logger.Log($@"Input for {this.InputName} has been successfully updated to {this.InputButton}");
    }

    // Update input type method
    public void UpdateInputType(InputTypeEnum _newInputType)
    {
        this.InputType = _newInputType;
    }
}
