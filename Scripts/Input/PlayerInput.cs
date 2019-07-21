﻿using System;
using System.Collections.Generic;

using UnityEngine;

// Reads and stores inputs from player
public static class PlayerInput
{
    // Input mode
    public enum InputModeEnum
    {
        KBM,
        Controller
    }
    public static InputModeEnum InputMode = InputModeEnum.KBM;

    // Movement style
    public enum MovementStyleEnum
    {
        ScreenSpace,
        Tank
    }
    public static MovementStyleEnum MovementStyle = MovementStyleEnum.Tank;

    // Input dictionary
    public static Dictionary<InputBinding.GameInputsEnum, float> CurrentInputValues { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, float>();

    // KB&M input bindings
    public static Dictionary<InputBinding.GameInputsEnum, InputBinding> InputBindingsKBM { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, InputBinding>
    {
        {InputBinding.GameInputsEnum.Submit, new InputBinding(InputBinding.GameInputsEnum.Submit, InputBinding.InputTypeEnum.Key, KeyCode.Return)},
        {InputBinding.GameInputsEnum.Cancel, new InputBinding(InputBinding.GameInputsEnum.Cancel, InputBinding.InputTypeEnum.Key, KeyCode.Backspace)},
        {InputBinding.GameInputsEnum.Pause, new InputBinding(InputBinding.GameInputsEnum.Pause, InputBinding.InputTypeEnum.Key, KeyCode.Escape)},
        {InputBinding.GameInputsEnum.MainGun, new InputBinding(InputBinding.GameInputsEnum.MainGun, InputBinding.InputTypeEnum.MouseButton, MouseLeftClick)},
        {InputBinding.GameInputsEnum.Ability1, new InputBinding(InputBinding.GameInputsEnum.Ability1, InputBinding.InputTypeEnum.Key, KeyCode.E)},
        {InputBinding.GameInputsEnum.Ability2, new InputBinding(InputBinding.GameInputsEnum.Ability2, InputBinding.InputTypeEnum.Key, KeyCode.Q)},
        {InputBinding.GameInputsEnum.Ability3, new InputBinding(InputBinding.GameInputsEnum.Ability3, InputBinding.InputTypeEnum.MouseButton, MouseRightClick)},
        {InputBinding.GameInputsEnum.Warp, new InputBinding(InputBinding.GameInputsEnum.Warp, InputBinding.InputTypeEnum.Key, KeyCode.LeftShift)},
        {InputBinding.GameInputsEnum.MoveInputXPos, new InputBinding(InputBinding.GameInputsEnum.MoveInputXPos, InputBinding.InputTypeEnum.Key, KeyCode.D)},
        {InputBinding.GameInputsEnum.MoveInputXNeg, new InputBinding(InputBinding.GameInputsEnum.MoveInputXNeg, InputBinding.InputTypeEnum.Key, KeyCode.A)},
        {InputBinding.GameInputsEnum.MoveInputYPos, new InputBinding(InputBinding.GameInputsEnum.MoveInputYPos, InputBinding.InputTypeEnum.Key, KeyCode.W)},
        {InputBinding.GameInputsEnum.MoveInputYNeg, new InputBinding(InputBinding.GameInputsEnum.MoveInputYNeg, InputBinding.InputTypeEnum.Key, KeyCode.S)},
    };
    // Controller input bindings
    public static Dictionary<InputBinding.GameInputsEnum, InputBinding> InputBindingsController { get; private set; } = new Dictionary<InputBinding.GameInputsEnum, InputBinding>
    {
        {InputBinding.GameInputsEnum.Submit, new InputBinding(InputBinding.GameInputsEnum.Submit, InputBinding.InputTypeEnum.Button, ControllerButton0Input)},
        {InputBinding.GameInputsEnum.Cancel, new InputBinding(InputBinding.GameInputsEnum.Cancel, InputBinding.InputTypeEnum.Button, ControllerButton1Input)},
        {InputBinding.GameInputsEnum.Pause, new InputBinding(InputBinding.GameInputsEnum.Pause, InputBinding.InputTypeEnum.Button, ControllerButton7Input)},
        {InputBinding.GameInputsEnum.MainGun, new InputBinding(InputBinding.GameInputsEnum.MainGun, InputBinding.InputTypeEnum.Button, ControllerButton0Input)},
        {InputBinding.GameInputsEnum.Ability1, new InputBinding(InputBinding.GameInputsEnum.Ability1, InputBinding.InputTypeEnum.Button, ControllerButton2Input)},
        {InputBinding.GameInputsEnum.Ability2, new InputBinding(InputBinding.GameInputsEnum.Ability2, InputBinding.InputTypeEnum.Button, ControllerButton3Input)},
        {InputBinding.GameInputsEnum.Ability3, new InputBinding(InputBinding.GameInputsEnum.Ability3, InputBinding.InputTypeEnum.Button, ControllerButton1Input)},
        {InputBinding.GameInputsEnum.Warp, new InputBinding(InputBinding.GameInputsEnum.Warp, InputBinding.InputTypeEnum.Trigger, ControllerLeftTriggerInput)},
        {InputBinding.GameInputsEnum.MoveInputXAxis, new InputBinding(InputBinding.GameInputsEnum.MoveInputXAxis, InputBinding.InputTypeEnum.Axis, ControllerLeftStickHorizontalInput)},
        {InputBinding.GameInputsEnum.MoveInputYAxis, new InputBinding(InputBinding.GameInputsEnum.MoveInputYAxis, InputBinding.InputTypeEnum.Axis, ControllerLeftStickVerticalInput)},
        {InputBinding.GameInputsEnum.AimInputXAxis, new InputBinding(InputBinding.GameInputsEnum.AimInputXAxis, InputBinding.InputTypeEnum.Axis, ControllerRightStickHorizontalInput)},
        {InputBinding.GameInputsEnum.AimInputYAxis, new InputBinding(InputBinding.GameInputsEnum.AimInputYAxis, InputBinding.InputTypeEnum.Axis, ControllerRightStickVerticalInput)},
    };

    // Rebinding input fields
    public static bool RebindingInputs = false;
    public static InputBinding.GameInputsEnum InputToRebind;
    public static float RebindingStartedTime = 0f;
    private const float MaxRebindingInputDuration = 5f;

    // Input names in editor
    private const string ControllerLeftStickHorizontalInput = "Controller Left Stick Horizontal";
    private const string ControllerLeftStickVerticalInput = "Controller Left Stick Vertical";
    private const string ControllerRightStickHorizontalInput = "Controller Right Stick Horizontal";
    private const string ControllerRightStickVerticalInput = "Controller Right Stick Vertical";
    private const string ControllerDPadHorizontalInput = "Controller DPad Horizontal";
    private const string ControllerDPadVerticalInput = "Controller DPad Vertical";
    private const string ControllerButton0Input = "Controller Button 0";
    private const string ControllerButton1Input = "Controller Button 1";
    private const string ControllerButton2Input = "Controller Button 2";
    private const string ControllerButton3Input = "Controller Button 3";
    private const string ControllerButton4Input = "Controller Button 4";
    private const string ControllerButton5Input = "Controller Button 5";
    private const string ControllerButton6Input = "Controller Button 6";
    private const string ControllerButton7Input = "Controller Button 7";
    private const string ControllerButton8Input = "Controller Button 8";
    private const string ControllerButton9Input = "Controller Button 9";
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

    private static readonly List<string> ControllerButtons = new List<string>
    {
        ControllerButton0Input,
        ControllerButton1Input,
        ControllerButton2Input,
        ControllerButton3Input,
        ControllerButton4Input,
        ControllerButton5Input,
        ControllerButton6Input,
        ControllerButton7Input,
        ControllerButton8Input,
        ControllerButton9Input,
        ControllerButton10Input,
        ControllerButton11Input
    };


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
            InputBindingsController[InputToRebind].UpdateInputButton(GetPressedButton());
            RebindingInputs = false;
            Logger.Log($@"Input binding for {InputBindingsController[InputToRebind].InputName} is currently {InputBindingsController[InputToRebind].InputButton}");
        }
        else if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            RebindingInputs = false;
            UIController.UpdateSettingsErrorText($@"Input Rebinding has been cancelled");
            Logger.Log($@"Input Rebinding has been cancelled");
        }
        else
        {
            UIController.UpdateSettingsErrorText($@"Press Input you wish to rebind to {InputBindingsController[InputToRebind].InputName}. Press Esc to cancel. Cancelling in 5 seconds...");
        }
    }

    // Is any button pressed
    private static bool IsAnyButtonPressed()
    {
        foreach(string button in ControllerButtons)
        {
            if(Input.GetButton(button) == true)
            {
                return true;
            }
        }
        return false;
    }

    // Get pressed button
    private static string GetPressedButton()
    {
        foreach(string button in ControllerButtons)
        {
            if(Input.GetButton(button) == true)
            {
                return button;
            }
        }
        return "";
    }

    // Reads the inputs and stores them
    private static void ProcessInputs()
    {
        if(InputMode == InputModeEnum.Controller)
        {
            // TODO: Set up controller inputs process using KBM version below as template
        }
        else if(InputMode == InputModeEnum.KBM)
        {
            foreach(InputBinding.GameInputsEnum input in (InputBinding.GameInputsEnum[]) Enum.GetValues(typeof(InputBinding.GameInputsEnum)))
            {
                // If input key has already been added, update value
                if(CurrentInputValues.ContainsKey(input))
                {
                    if(input == InputBinding.GameInputsEnum.MoveInputXAxis)
                    {
                        float value = 0f;
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputXPos] == 1f)
                        {
                            value++;
                        }
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputXNeg] == 1f)
                        {
                            value--;
                        }
                        CurrentInputValues[input] = value;
                    }
                    else if(input == InputBinding.GameInputsEnum.MoveInputYAxis)
                    {
                        float value = 0f;
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputYPos] == 1f)
                        {
                            value++;
                        }
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputYNeg] == 1f)
                        {
                            value--;
                        }
                        CurrentInputValues[input] = value;
                    }
                    else if(input == InputBinding.GameInputsEnum.AimInputXAxis)
                    {
                        int halfWidth = Screen.width / 2;
                        CurrentInputValues[input] = (Input.mousePosition.x - halfWidth) / halfWidth;
                    }
                    else if(input == InputBinding.GameInputsEnum.AimInputYAxis)
                    {
                        int halfHeight = Screen.height / 2;
                        CurrentInputValues[input] = -(Input.mousePosition.y - halfHeight) / halfHeight;
                    }
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.Key)
                    {
                        CurrentInputValues[input] = (Input.GetKey(InputBindingsKBM[input].InputKey) == true) ? 1f : 0f;
                    }
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.MouseButton)
                    {
                        CurrentInputValues[input] = (Input.GetMouseButton(InputBindingsKBM[input].InputMouseButton) == true) ? 1f : 0f;
                    }
                }
                // If input key hasn't been added, add it with the correct value
                else
                {
                    if(input == InputBinding.GameInputsEnum.MoveInputXAxis)
                    {
                        float value = 0f;
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputXPos] == 1f)
                        {
                            value++;
                        }
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputXNeg] == 1f)
                        {
                            value--;
                        }
                        CurrentInputValues.Add(input, value);
                    }
                    else if(input == InputBinding.GameInputsEnum.MoveInputYAxis)
                    {
                        float value = 0f;
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputYPos] == 1f)
                        {
                            value++;
                        }
                        if(CurrentInputValues[InputBinding.GameInputsEnum.MoveInputYNeg] == 1f)
                        {
                            value--;
                        }
                        CurrentInputValues.Add(input, value);
                    }
                    else if(input == InputBinding.GameInputsEnum.AimInputXAxis)
                    {
                        int halfWidth = Screen.width / 2;
                        CurrentInputValues.Add(input, (Input.mousePosition.x - halfWidth) / halfWidth);
                    }
                    else if(input == InputBinding.GameInputsEnum.AimInputYAxis)
                    {
                        int halfHeight = Screen.height / 2;
                        CurrentInputValues.Add(input, -(Input.mousePosition.y - halfHeight) / halfHeight);
                    }
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.Key)
                    {
                        CurrentInputValues.Add(input, (Input.GetKey(InputBindingsKBM[input].InputKey) == true) ? 1f : 0f);
                    }
                    else if(InputBindingsKBM[input].InputType == InputBinding.InputTypeEnum.MouseButton)
                    {
                        CurrentInputValues.Add(input, (Input.GetMouseButton(InputBindingsKBM[input].InputMouseButton) == true) ? 1f : 0f);
                    }
                }
            }
        }
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
        Button,
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
        MoveInputXPos,
        MoveInputXNeg,
        MoveInputYPos,
        MoveInputYNeg,
        MoveInputXAxis,
        MoveInputYAxis,
        AimInputXAxis,
        AimInputYAxis
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
