using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// Reads and stores inputs from player
public static class PlayerInput
{
    // Input mode
    public enum InputModeEnum
    {
        KeyboardAndMouse,
        Controller
    }
    public static InputModeEnum InputMode { get; private set; } = InputModeEnum.KeyboardAndMouse;

    // Movement style
    public enum MovementStyleEnum
    {
        ScreenSpace,
        Tank
    }
    public static MovementStyleEnum MovementStyle { get; private set; } = MovementStyleEnum.ScreenSpace;

    // Input dictionary
    public static Dictionary<InputBinding.GameInputsEnum, float> CurrentInputValues { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, float>();

    // Default KB&M input bindings
    public static Dictionary<InputBinding.GameInputsEnum, InputBinding> InputBindingsKBMDefault { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, InputBinding>
    {
        {InputBinding.GameInputsEnum.Submit, new InputBinding(InputBinding.GameInputsEnum.Submit, InputBinding.InputTypeEnum.Key, KeyCode.Return)},
        {InputBinding.GameInputsEnum.Cancel, new InputBinding(InputBinding.GameInputsEnum.Cancel, InputBinding.InputTypeEnum.Key, KeyCode.Backspace)},
        {InputBinding.GameInputsEnum.Pause, new InputBinding(InputBinding.GameInputsEnum.Pause, InputBinding.InputTypeEnum.Key, KeyCode.Escape)},
        {InputBinding.GameInputsEnum.MainGun, new InputBinding(InputBinding.GameInputsEnum.MainGun, InputBinding.InputTypeEnum.MouseButton, MouseLeftClick)},
        {InputBinding.GameInputsEnum.Ability1, new InputBinding(InputBinding.GameInputsEnum.Ability1, InputBinding.InputTypeEnum.Key, KeyCode.E)},
        {InputBinding.GameInputsEnum.Ability2, new InputBinding(InputBinding.GameInputsEnum.Ability2, InputBinding.InputTypeEnum.Key, KeyCode.Q)},
        {InputBinding.GameInputsEnum.Ability3, new InputBinding(InputBinding.GameInputsEnum.Ability3, InputBinding.InputTypeEnum.MouseButton, MouseRightClick)},
        {InputBinding.GameInputsEnum.Warp, new InputBinding(InputBinding.GameInputsEnum.Warp, InputBinding.InputTypeEnum.Key, KeyCode.LeftShift)},
        {InputBinding.GameInputsEnum.MoveRight, new InputBinding(InputBinding.GameInputsEnum.MoveRight, InputBinding.InputTypeEnum.Key, KeyCode.D)},
        {InputBinding.GameInputsEnum.MoveLeft, new InputBinding(InputBinding.GameInputsEnum.MoveLeft, InputBinding.InputTypeEnum.Key, KeyCode.A)},
        {InputBinding.GameInputsEnum.MoveForward, new InputBinding(InputBinding.GameInputsEnum.MoveForward, InputBinding.InputTypeEnum.Key, KeyCode.W)},
        {InputBinding.GameInputsEnum.MoveBack, new InputBinding(InputBinding.GameInputsEnum.MoveBack, InputBinding.InputTypeEnum.Key, KeyCode.S)},
    };
    // Default controller input bindings
    public static Dictionary<InputBinding.GameInputsEnum, InputBinding> InputBindingsControllerDefault { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, InputBinding>
    {
        {InputBinding.GameInputsEnum.Submit, new InputBinding(InputBinding.GameInputsEnum.Submit, InputBinding.InputTypeEnum.ControllerButton, ControllerAButtonInput)},
        {InputBinding.GameInputsEnum.Cancel, new InputBinding(InputBinding.GameInputsEnum.Cancel, InputBinding.InputTypeEnum.ControllerButton, ControllerBButtonInput)},
        {InputBinding.GameInputsEnum.Pause, new InputBinding(InputBinding.GameInputsEnum.Pause, InputBinding.InputTypeEnum.ControllerButton, ControllerStartButtonInput)},
        {InputBinding.GameInputsEnum.MainGun, new InputBinding(InputBinding.GameInputsEnum.MainGun, InputBinding.InputTypeEnum.Trigger, ControllerRightTriggerInput)},
        {InputBinding.GameInputsEnum.Ability1, new InputBinding(InputBinding.GameInputsEnum.Ability1, InputBinding.InputTypeEnum.ControllerButton, ControllerLeftBumperInput)},
        {InputBinding.GameInputsEnum.Ability2, new InputBinding(InputBinding.GameInputsEnum.Ability2, InputBinding.InputTypeEnum.Trigger, ControllerLeftTriggerInput)},
        {InputBinding.GameInputsEnum.Ability3, new InputBinding(InputBinding.GameInputsEnum.Ability3, InputBinding.InputTypeEnum.ControllerButton, ControllerRightBumperInput)},
        {InputBinding.GameInputsEnum.Warp, new InputBinding(InputBinding.GameInputsEnum.Warp, InputBinding.InputTypeEnum.ControllerButton, ControllerLeftStickClickInput)},
        {InputBinding.GameInputsEnum.MoveXAxis, new InputBinding(InputBinding.GameInputsEnum.MoveXAxis, InputBinding.InputTypeEnum.Axis, ControllerLeftStickHorizontalInput)},
        {InputBinding.GameInputsEnum.MoveYAxis, new InputBinding(InputBinding.GameInputsEnum.MoveYAxis, InputBinding.InputTypeEnum.Axis, ControllerLeftStickVerticalInput)},
        {InputBinding.GameInputsEnum.AimXAxis, new InputBinding(InputBinding.GameInputsEnum.AimXAxis, InputBinding.InputTypeEnum.Axis, ControllerRightStickHorizontalInput)},
        {InputBinding.GameInputsEnum.AimYAxis, new InputBinding(InputBinding.GameInputsEnum.AimYAxis, InputBinding.InputTypeEnum.Axis, ControllerRightStickVerticalInput)},
    };

    // Current KB&M input bindings
    public static Dictionary<InputBinding.GameInputsEnum, InputBinding> InputBindingsKBM { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, InputBinding>();
    // Current controller input bindings
    public static Dictionary<InputBinding.GameInputsEnum, InputBinding> InputBindingsController { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, InputBinding>();

    // Pause check
    private static float LastPauseCheck = 0f;
    private const float PauseCheckCD = 1f;

    // Rebinding input fields
    private static bool RebindingInputs = false;
    private static InputBinding InputToRebind;
    private static InputModeEnum InputToRebindMode;
    private static float RebindingStartedTime = 0f;
    private const float MaxRebindingInputDuration = 5f;

    // Input names in editor
    private const string ControllerLeftStickHorizontalInput = "Controller Left Stick Horizontal";
    private const string ControllerLeftStickVerticalInput = "Controller Left Stick Vertical";
    private const string ControllerRightStickHorizontalInput = "Controller Right Stick Horizontal";
    private const string ControllerRightStickVerticalInput = "Controller Right Stick Vertical";
    private const string ControllerDPadHorizontalInput = "Controller DPad Horizontal";
    private const string ControllerDPadVerticalInput = "Controller DPad Vertical";
    private const string ControllerAButtonInput = "Controller Button 0";
    private const string ControllerBButtonInput = "Controller Button 1";
    private const string ControllerXButtonInput = "Controller Button 2";
    private const string ControllerYButtonInput = "Controller Button 3";
    private const string ControllerLeftBumperInput = "Controller Button 4";
    private const string ControllerRightBumperInput = "Controller Button 5";
    private const string ControllerBackButtonInput = "Controller Button 6";
    private const string ControllerStartButtonInput = "Controller Button 7";
    private const string ControllerLeftStickClickInput = "Controller Button 8";
    private const string ControllerRightStickClickInput = "Controller Button 9";
    private const string ControllerButton10Input = "Controller Button 10";
    private const string ControllerButton11Input = "Controller Button 11";
    private const string ControllerLeftTriggerInput = "Controller Left Trigger";
    private const string ControllerRightTriggerInput = "Controller Right Trigger";
    private const string HorizontalMenuInput = "Horizontal Menu";
    private const string VerticalMenuInput = "Vertical Menu";
    private const string SubmitButtonInput = "Submit";
    private const string CancelButtonInput = "Cancel";
    // Mouse
    private const int MouseLeftClick = 0;
    private const int MouseRightClick = 1;
    private const int MouseMiddleClick = 2;

    // Controller button names list
    private static readonly List<string> ControllerButtons = new List<string>
    {
        ControllerAButtonInput,
        ControllerBButtonInput,
        ControllerXButtonInput,
        ControllerYButtonInput,
        ControllerLeftBumperInput,
        ControllerRightBumperInput,
        ControllerBackButtonInput,
        ControllerStartButtonInput,
        ControllerLeftStickClickInput,
        ControllerRightStickClickInput,
        ControllerButton10Input,
        ControllerButton11Input
    };
    // Controller axis button names list
    private static readonly List<string> ControllerAxisButtons = new List<string>
    {
        //ControllerDPadHorizontalInput,
        //ControllerDPadVerticalInput,
        ControllerLeftTriggerInput,
        ControllerRightTriggerInput
    };


    // Initialize
    public static void Initialize()
    {
        SetDefaultBindings();
    }

    // Update is called once per frame
    public static void Update()
    {
        // If rebinding inputs is true
        if(RebindingInputs == true)
        {
            // Check rebind
            CheckRebind();
        }
        // If not rebinding inputs
        else
        {
            // Process inputs
            ProcessInputs();
        }
    }

    // Set up rebind keybind
    public static void SetupRebind(InputModeEnum _inputMode, string _inputToRebind)
    {
        // Set rebinding inputs to true
        RebindingInputs = true;
        // Get input rebind mode
        InputToRebindMode = _inputMode;
        // If input mode is controller
        if(InputToRebindMode == InputModeEnum.Controller)
        {
            // Set input to rebind to the controller input identified
            InputToRebind = InputBindingsController[(InputBinding.GameInputsEnum)Enum.Parse(typeof(InputBinding.GameInputsEnum), _inputToRebind)];
            // Get currently selected button
            UIController.GetOrSetSelectedButton(true);
        }
        // If input mode is KBM
        else if(InputToRebindMode == InputModeEnum.KeyboardAndMouse)
        {
            // Set input to rebind to the KBM input identified
            InputToRebind = InputBindingsKBM[(InputBinding.GameInputsEnum)Enum.Parse(typeof(InputBinding.GameInputsEnum), _inputToRebind)];
        }
        // Get rebind started time
        RebindingStartedTime = Time.time;
        Debug.Log($@"Starting input rebinding: {InputToRebind.InputName} for {InputToRebindMode.ToString()}.");
        Logger.Log($@"Starting input rebinding: {InputToRebind.InputName} for {InputToRebindMode.ToString()}.");
    }

    // Rebind input
    private static void CheckRebind()
    {
        // Update message with rebinding instructions
        UIController.ChangeErrorText($@"Press input you wish to rebind {InputToRebind.InputName} to for {InputToRebindMode.ToString()}.{Environment.NewLine}Press Esc to cancel. Auto-Cancelling in {(MaxRebindingInputDuration - (Time.time - RebindingStartedTime)).ToString("0.0")} seconds...");
        // If time since rebinding was started is greater than max rebinding allowed time
        if(Time.time - RebindingStartedTime >= MaxRebindingInputDuration)
        {
            // Time out rebinding
            RebindingInputs = false;
            // Set selected button to last selected
            UIController.GetOrSetSelectedButton(false);
            UIController.ChangeErrorText($@"Input rebinding has timed out.");
            Debug.Log($@"Input rebinding has timed out.");
            Logger.Log($@"Input rebinding has timed out.");
        }
        // If escape key is pressed
        else if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            // Cancel rebinding
            RebindingInputs = false;
            // Set selected button to last selected
            UIController.GetOrSetSelectedButton(false);
            UIController.ChangeErrorText($@"Input rebinding has been cancelled.");
            Debug.Log($@"Input rebinding has been cancelled.");
            Logger.Log($@"Input rebinding has been cancelled.");
        }
        // If input to rebind mode is controller
        else if(InputToRebindMode == InputModeEnum.Controller)
        {
            // If time since starting is longer than the last frame took (basically skip one frame before you do the below) and a button was pressed and rebound
            if(Time.time - RebindingStartedTime > Time.deltaTime && CheckPressedKeyButtonAndRebind() == true)
            {
                // End rebinding
                RebindingInputs = false;
                // Set selected button to last selected
                UIController.GetOrSetSelectedButton(false);
                Debug.Log($@"Input rebinding completed successfully.");
                Logger.Log($@"Input rebinding completed successfully.");
            }
        }
        // If any button or key was pressed and rebound
        else if(CheckPressedKeyButtonAndRebind() == true)
        {
            // End rebinding
            RebindingInputs = false;
            // Set selected button to last selected
            UIController.GetOrSetSelectedButton(false);
            Debug.Log($@"Input rebinding completed successfully.");
            Logger.Log($@"Input rebinding completed successfully.");
        }
    }

    // Get pressed key or button and rebind input to pressed key or button
    private static bool CheckPressedKeyButtonAndRebind()
    {
        // If input mode is controller
        if(InputToRebindMode == InputModeEnum.Controller)
        {
            // Loop through all controller buttons
            foreach(string button in ControllerButtons)
            {
                // If button is pressed
                if(Input.GetButtonDown(button) == true)
                {
                    // Update binding to pressed button
                    InputToRebind.UpdateInputButton(button);
                    InputToRebind.UpdateInputType(InputBinding.InputTypeEnum.ControllerButton);
                    UIController.ChangeErrorText($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Debug.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Logger.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    return true;
                }
            }
            // Loop through all controller axis buttons (triggers)
            foreach(string axisButton in ControllerAxisButtons)
            {
                // If axis button is pressed
                if(Input.GetAxis(axisButton) != 0f)
                {
                    // Update binding to pressed axis button
                    InputToRebind.UpdateInputButton(axisButton);
                    InputToRebind.UpdateInputType(InputBinding.InputTypeEnum.Trigger);
                    UIController.ChangeErrorText($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Debug.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Logger.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    return true;
                }
            }
        }
        // If input mode is KBM
        else if(InputToRebindMode == InputModeEnum.KeyboardAndMouse)
        {
            // Loop through all mouse buttons
            for(int mouseButton = 0; mouseButton < 3; mouseButton++)
            {
                // If mouse button is pressed
                if(Input.GetMouseButton(mouseButton) == true)
                {
                    // Update binding to pressed mouse button
                    InputToRebind.UpdateInputMouseButton(mouseButton);
                    InputToRebind.UpdateInputType(InputBinding.InputTypeEnum.MouseButton);
                    UIController.ChangeErrorText($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Debug.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Logger.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    return true;
                }
            }
            // Loop through all keys
            foreach(KeyCode key in (KeyCode[])Enum.GetValues(typeof(KeyCode)))
            {
                // If key is pressed
                if(Input.GetKey(key) == true)
                {
                    // Update binding to pressed key
                    InputToRebind.UpdateInputKey(key);
                    InputToRebind.UpdateInputType(InputBinding.InputTypeEnum.Key);
                    UIController.ChangeErrorText($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Debug.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    Logger.Log($@"{InputToRebind.InputName} for {InputToRebindMode.ToString()} is now bound to {GetStringForKeybind(InputToRebind)}.");
                    return true;
                }
            }
        }
        // If nothing is pressed return false
        return false;
    }

    // Reads the inputs and stores them
    private static void ProcessInputs()
    {
        // If input mode is controller
        if(InputMode == InputModeEnum.Controller)
        {
            // Loop through all input binding types
            foreach(InputBinding.GameInputsEnum input in (InputBinding.GameInputsEnum[])Enum.GetValues(typeof(InputBinding.GameInputsEnum)))
            {
                // Checks if input is contained in input bindings controller
                if(InputBindingsController.ContainsKey(input))
                {
                    // If input key has already been added, update value
                    if(CurrentInputValues.ContainsKey(input))
                    {
                        // If input is type axis, report value directly
                        if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.Axis)
                        {
                            CurrentInputValues[input] = Input.GetAxis(InputBindingsController[input].InputButton);
                        }
                        // If input is type button, set value to 1 for pressed and 0 for unpressed
                        else if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.ControllerButton)
                        {
                            CurrentInputValues[input] = (Input.GetButton(InputBindingsController[input].InputButton) == true) ? 1f : 0f;
                        }
                        // If input is type trigger, set value to 1 for pressed and 0 for unpressed
                        else if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.Trigger)
                        {
                            CurrentInputValues[input] = (Input.GetAxis(InputBindingsController[input].InputButton) >= 0.5f || Input.GetAxis(InputBindingsController[input].InputButton) <= -0.5f) ? 1f : 0f;
                        }
                        // If input is type DPad, ....
                        else if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.DPad)
                        {
                            // TODO: set up dpad input scheme, not sure how I wanna set this up yet, also do below in add section
                        }
                        // Invert y input on move axis
                        CurrentInputValues[InputBinding.GameInputsEnum.MoveYAxis] *= -1;
                    }
                    // If input key hasn't been added, add it with the correct value
                    else
                    {
                        // If input is type axis, report value directly
                        if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.Axis)
                        {
                            CurrentInputValues.Add(input, Input.GetAxis(InputBindingsController[input].InputButton));
                        }
                        // If input is type button, set value to 1 for pressed and 0 for unpressed
                        else if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.ControllerButton)
                        {
                            CurrentInputValues.Add(input, (Input.GetButton(InputBindingsController[input].InputButton) == true) ? 1f : 0f);
                        }
                        // If input is type trigger, set value to 1 for pressed and 0 for unpressed
                        else if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.Trigger)
                        {
                            CurrentInputValues.Add(input, (Input.GetAxis(InputBindingsController[input].InputButton) >= 0.5f || Input.GetAxis(InputBindingsController[input].InputButton) <= -0.5f) ? 1f : 0f);
                        }
                        // If input is type DPad, ....
                        else if(InputBindingsController[input].InputType == InputBinding.InputTypeEnum.DPad)
                        {
                            // Not sure how I wanna set this up yet
                        }
                        // Invert y input on move axis
                        CurrentInputValues[InputBinding.GameInputsEnum.MoveYAxis] *= -1;
                    }
                }
            }
        }
        // If input mode is keyboard and mouse
        else if(InputMode == InputModeEnum.KeyboardAndMouse)
        {
            // Loop through all input binding types
            foreach(InputBinding.GameInputsEnum input in (InputBinding.GameInputsEnum[]) Enum.GetValues(typeof(InputBinding.GameInputsEnum)))
            {
                // If input key has already been added, update value
                if(CurrentInputValues.ContainsKey(input))
                {
                    // If input is move x axis, get value
                    if(input == InputBinding.GameInputsEnum.MoveXAxis)
                    {
                        // Default value to 0, get xPos and xNeg values from input
                        float value = 0f;
                        float xPosValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveRight];
                        float xNegValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveLeft];
                        // If xPos is not 0, add it to value, if xNeg is not 0, subtract it from value
                        if(xPosValue != 0f)
                        {
                            value += xPosValue;
                        }
                        if(xNegValue != 0f)
                        {
                            value -= xNegValue;
                        }
                        // Set value
                        CurrentInputValues[input] = value;
                    }
                    // If input is move y axis, get value
                    else if(input == InputBinding.GameInputsEnum.MoveYAxis)
                    {
                        // Default value to 0, get yPos and yNeg values from input
                        float value = 0f;
                        float yPosValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveForward];
                        float yNegValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveBack];
                        // If yPos is not 0, add it to value, if yNeg is not 0, subtract it from value
                        if(yPosValue != 0f)
                        {
                            value += yPosValue;
                        }
                        if(yNegValue != 0f)
                        {
                            value -= yNegValue;
                        }
                        // Set value
                        CurrentInputValues[input] = value;
                    }
                    // If input is aim x axis, get value
                    else if(input == InputBinding.GameInputsEnum.AimXAxis)
                    {
                        // Get half the screen width, use to transform mouse position to a value of -1 for left and 1 for right
                        int halfWidth = Screen.width / 2;
                        CurrentInputValues[input] = (Input.mousePosition.x - halfWidth) / halfWidth;
                    }
                    // If input is aim y axis special case
                    else if(input == InputBinding.GameInputsEnum.AimYAxis)
                    {
                        // Get half the screen height, use to transform mouse position to a value of -1 for top and 1 for bottom
                        int halfHeight = Screen.height / 2;
                        CurrentInputValues[input] = -(Input.mousePosition.y - halfHeight) / halfHeight;
                    }
                    // If input is type key, set value to 1 for pressed and 0 for unpressed
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.Key)
                    {
                        CurrentInputValues[input] = (Input.GetKey(InputBindingsKBM[input].InputKey) == true) ? 1f : 0f;
                    }
                    // If input is type mouse button, set value to 1 for pressed and 0 for unpressed
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.MouseButton)
                    {
                        CurrentInputValues[input] = (Input.GetMouseButton(InputBindingsKBM[input].InputMouseButton) == true) ? 1f : 0f;
                    }
                }
                // If input key hasn't been added, add it with the correct value
                else
                {
                    // If input is move x axis, get value
                    if(input == InputBinding.GameInputsEnum.MoveXAxis)
                    {
                        // Default value to 0, get xPos and xNeg values from input
                        float value = 0f;
                        float xPosValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveRight];
                        float xNegValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveLeft];
                        // If xPos is not 0, add it to value, if xNeg is not 0, subtract it from value
                        if(xPosValue != 0f)
                        {
                            value += xPosValue;
                        }
                        if(xNegValue != 0f)
                        {
                            value -= xNegValue;
                        }
                        // Add key and set value
                        CurrentInputValues.Add(input, value);
                    }
                    // If input is move y axis, get value
                    else if(input == InputBinding.GameInputsEnum.MoveYAxis)
                    {
                        // Default value to 0, get yPos and yNeg values from input
                        float value = 0f;
                        float yPosValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveForward];
                        float yNegValue = CurrentInputValues[InputBinding.GameInputsEnum.MoveBack];
                        // If yPos is not 0, add it to value, if yNeg is not 0, subtract it from value
                        if(yPosValue != 0f)
                        {
                            value += yPosValue;
                        }
                        if(yNegValue != 0f)
                        {
                            value -= yNegValue;
                        }
                        // Add key and set value
                        CurrentInputValues.Add(input, value);
                    }
                    // If input is aim x axis, get value
                    else if(input == InputBinding.GameInputsEnum.AimXAxis)
                    {
                        // Get half the screen width, use to transform mouse position to a value of -1 for left and 1 for right
                        int halfWidth = Screen.width / 2;
                        CurrentInputValues.Add(input, (Input.mousePosition.x - halfWidth) / halfWidth);
                    }
                    // If input is aim y axis special case
                    else if(input == InputBinding.GameInputsEnum.AimYAxis)
                    {
                        // Get half the screen height, use to transform mouse position to a value of -1 for top and 1 for bottom
                        int halfHeight = Screen.height / 2;
                        CurrentInputValues.Add(input, -(Input.mousePosition.y - halfHeight) / halfHeight);
                    }
                    // If input is type key, set value to 1 for pressed and 0 for unpressed
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.Key)
                    {
                        CurrentInputValues.Add(input, (Input.GetKey(InputBindingsKBM[input].InputKey) == true) ? 1f : 0f);
                    }
                    // If input is type mouse button, set value to 1 for pressed and 0 for unpressed
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.MouseButton)
                    {
                        CurrentInputValues.Add(input, (Input.GetMouseButton(InputBindingsKBM[input].InputMouseButton) == true) ? 1f : 0f);
                    }
                }
            }
        }
        PauseCheck();
    }

    // Pause check
    private static void PauseCheck()
    {
        // If pause is pressed and it has been long enough since last checked
        if(CurrentInputValues[InputBinding.GameInputsEnum.Pause] == 1 && Time.time - LastPauseCheck > PauseCheckCD)
        {
            // Pause game if pause is pressed
            if(CurrentInputValues[InputBinding.GameInputsEnum.Pause] == 1)
            {
                // If game state is playing
                if(GameController.CurrentGameState == GameController.GameState.Playing)
                {
                    GameController.ChangeGameState(GameController.GameState.Paused);
                    UIController.OpenPausePopUp(true);
                    CurrentInputValues[InputBinding.GameInputsEnum.Pause] = 0;
                    LastPauseCheck = Time.time;
                }
                // If game state is paused
                else if(GameController.CurrentGameState == GameController.GameState.Paused)
                {
                    GameController.ChangeGameState(GameController.GameState.Playing);
                    UIController.OpenPausePopUp(false);
                    CurrentInputValues[InputBinding.GameInputsEnum.Pause] = 0;
                    LastPauseCheck = Time.time;
                }
            }
        }
    }

    // Change movement style
    public static void ChangeMovementStyle(MovementStyleEnum _newMovementStyle)
    {
        MovementStyle = _newMovementStyle;
    }

    // Change input type
    public static void ChangeInputType(InputModeEnum _newInputType)
    {
        InputMode = _newInputType;
    }

    // Get keybind name for ability
    public static string GetKeybindNameForInput(InputBinding.GameInputsEnum _input)
    {
        // If input mode is controller
        if(InputMode == InputModeEnum.Controller)
        {
            // Get string for keybind for controller input for ability 1
            return GetStringForKeybind(InputBindingsController[_input]);
        }
        // If input mode is KB&M
        else if(InputMode == InputModeEnum.KeyboardAndMouse)
        {
            // Get string for keybind for KB&M input for ability 1
            return GetStringForKeybind(InputBindingsKBM[_input]);
        }
        // Default return
        return "N/A";
    }

    // Get string for keybind
    public static string GetStringForKeybind(InputBinding _keybind)
    {
        // If input type is trigger
        if(_keybind.InputType == InputBinding.InputTypeEnum.Trigger)
        {
            // Check list of input button triggers and return appropriate name
            if(_keybind.InputButton == ControllerRightTriggerInput)
            {
                return "Right Trigger";
            }
            else if(_keybind.InputButton == ControllerLeftTriggerInput)
            {
                return "Left Trigger";
            }
        }
        // If input type is button
        else if(_keybind.InputType == InputBinding.InputTypeEnum.ControllerButton)
        {
            // Check list of input buttons and return appropriate name
            if(_keybind.InputButton == ControllerAButtonInput)
            {
                return "A";
            }
            else if(_keybind.InputButton == ControllerBButtonInput)
            {
                return "B";
            }
            else if(_keybind.InputButton == ControllerXButtonInput)
            {
                return "X";
            }
            else if(_keybind.InputButton == ControllerYButtonInput)
            {
                return "Y";
            }
            else if(_keybind.InputButton == ControllerRightBumperInput)
            {
                return "Right Bumper";
            }
            else if(_keybind.InputButton == ControllerLeftBumperInput)
            {
                return "Left Bumper";
            }
            else if(_keybind.InputButton == ControllerRightStickClickInput)
            {
                return "Right Stick Click";
            }
            else if(_keybind.InputButton == ControllerLeftStickClickInput)
            {
                return "Left Stick Click";
            }
            else if(_keybind.InputButton == ControllerBackButtonInput)
            {
                return "Back";
            }
            else if(_keybind.InputButton == ControllerStartButtonInput)
            {
                return "Start";
            }
            else if(_keybind.InputButton == ControllerButton10Input)
            {
                return "Button 10";
            }
            else if(_keybind.InputButton == ControllerButton11Input)
            {
                return "Button 11";
            }
        }
        // If input type is axis
        else if(_keybind.InputType == InputBinding.InputTypeEnum.Axis)
        {
            // Return stick name
            if(_keybind.InputButton == ControllerLeftStickHorizontalInput || _keybind.InputButton == ControllerLeftStickVerticalInput)
            {
                return "Left Stick";
            }
            else if(_keybind.InputButton == ControllerRightStickHorizontalInput || _keybind.InputButton == ControllerRightStickVerticalInput)
            {
                return "Right Stick";
            }
        }
        // If input type is key
        else if(_keybind.InputType == InputBinding.InputTypeEnum.Key)
        {
            // Return key name (with spaces between words)
            // Borrowed from https://stackoverflow.com/questions/9964467/create-space-between-capital-letters-and-skip-space-between-consecutive
            string key = _keybind.InputKey.ToString();
            key = string.Join(
                    string.Empty,
                    key.Select((x, i) => (
                         char.IsUpper(x) && i > 0 &&
                         (char.IsLower(key[i - 1]) || (i < key.Count() - 1 && char.IsLower(key[i + 1])))
                    ) ? " " + x : x.ToString()));
            return key;
        }
        // If input type is mouse button
        else if(_keybind.InputType == InputBinding.InputTypeEnum.MouseButton)
        {
            // Check list of input mouse buttons and return appropriate name
            if(_keybind.InputMouseButton == MouseLeftClick)
            {
                return "Left Click";
            }
            else if(_keybind.InputMouseButton == MouseRightClick)
            {
                return "Right Click";
            }
            else if(_keybind.InputMouseButton == MouseMiddleClick)
            {
                return "Middle Click";
            }
        }
        // Default return
        return "N/A";
    }

    // Set bindings to default settings
    public static void SetDefaultBindings()
    {
        // Clear input bindings for KBM
        InputBindingsKBM.Clear();
        foreach(KeyValuePair<InputBinding.GameInputsEnum, InputBinding> binding in InputBindingsKBMDefault)
        {
            // If input type is key
            if(binding.Value.InputType == InputBinding.InputTypeEnum.Key)
            {
                // Add a new binding of key type to input bindings dictionary
                InputBindingsKBM.Add(binding.Key, new InputBinding(binding.Key, binding.Value.InputType, binding.Value.InputKey));
            }
            // If input type is mouse button
            else if(binding.Value.InputType == InputBinding.InputTypeEnum.MouseButton)
            {
                // Add a new binding of mouse button type to input bindings dictionary
                InputBindingsKBM.Add(binding.Key, new InputBinding(binding.Key, binding.Value.InputType, binding.Value.InputMouseButton));
            }
        }
        // Set controller bindngs to new dictionary, clear it, and add each value from default to new dictionary
        InputBindingsController.Clear();
        foreach(KeyValuePair<InputBinding.GameInputsEnum, InputBinding> binding in InputBindingsControllerDefault)
        {
            // If input type is controller button, trigger, or axis
            if(binding.Value.InputType == InputBinding.InputTypeEnum.ControllerButton || binding.Value.InputType == InputBinding.InputTypeEnum.Trigger || binding.Value.InputType == InputBinding.InputTypeEnum.Axis)
            {
                // Add a new binding of controller button type to input bindings dictionary
                InputBindingsController.Add(binding.Key, new InputBinding(binding.Key, binding.Value.InputType, binding.Value.InputButton));
            }
        }
        Debug.Log($@"Input bindings set to defaults.");
        Logger.Log($@"Input bindings set to defaults.");
    }
}


// Container for input bindings
public class InputBinding
{
    // Input types
    public enum InputTypeEnum
    {
        Key,
        MouseButton,
        ControllerButton,
        DPad,
        Trigger,
        Axis
    }
    // Game input names
    public enum GameInputsEnum
    {
        Submit,
        Cancel,
        Pause,
        MainGun,
        Ability1,
        Ability2,
        Ability3,
        Warp,
        MoveRight,
        MoveLeft,
        MoveForward,
        MoveBack,
        MoveXAxis,
        MoveYAxis,
        AimXAxis,
        AimYAxis
    }
    // Constructor fields
    public GameInputsEnum InputName { get; private set; }
    public InputTypeEnum InputType { get; private set; }
    public string InputButton { get; private set; }
    public KeyCode InputKey { get; private set; }
    public int InputMouseButton { get; private set; }

    // Button Constructor
    public InputBinding(GameInputsEnum _inputName, InputTypeEnum _inputType, string _inputButton)
    {
        this.InputName = _inputName;
        this.InputType = _inputType;
        this.InputButton = _inputButton;
    }
    // Key Contstructor
    public InputBinding(GameInputsEnum _inputName, InputTypeEnum _inputType, KeyCode _inputKey)
    {
        this.InputName = _inputName;
        this.InputType = _inputType;
        this.InputKey = _inputKey;
    }
    // Mouse button Constructor
    public InputBinding(GameInputsEnum _inputName, InputTypeEnum _inputType, int _mouseButton)
    {
        this.InputName = _inputName;
        this.InputType = _inputType;
        this.InputMouseButton = _mouseButton;
    }


    // Update button
    public void UpdateInputButton(string _newInputButton)
    {
        Logger.Log($@"Updating Input for {this.InputName} from {this.InputButton} to {_newInputButton}");
        this.InputButton = _newInputButton;
    }

    // Update key
    public void UpdateInputKey(KeyCode _newInputKey)
    {
        Logger.Log($@"Updating Input for {this.InputName} from {this.InputKey} to {_newInputKey}");
        this.InputKey = _newInputKey;
    }

    // Update mouse button
    public void UpdateInputMouseButton(int _newMouseButton)
    {
        Logger.Log($@"Updating Input for {this.InputName} from {this.InputMouseButton} to {_newMouseButton}");
        this.InputMouseButton = _newMouseButton;
    }

    // Update input type
    public void UpdateInputType(InputTypeEnum _newInputType)
    {
        Logger.Log($@"Updating Input Type for {this.InputName} from {this.InputType} to {_newInputType}");
        this.InputType = _newInputType;
    }
}
