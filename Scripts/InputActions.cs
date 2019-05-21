// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Leviathan.inputactions'

using System.Collections;
using System.Collections.Generic;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputActions : IInputActionCollection
{
    private InputActionAsset asset;
    public InputActions()
    {
        this.asset = InputActionAsset.FromJson(@"{
    ""name"": ""Leviathan"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""22f889b6-1cf9-4a09-b6a4-a5d70181a491"",
            ""actions"": [
                {
                    ""name"": ""Aim"",
                    ""id"": ""7224a5d2-30a9-4f28-afe4-37708f40b23f"",
                    ""expectedControlLayout"": ""Vector2"",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""FireGun"",
                    ""id"": ""f6c75d0d-c9b0-411f-851b-abbaf35ee412"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""FireBomb"",
                    ""id"": ""2a5a75b8-8e45-4175-b0cb-192b95da8654"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""ActivateBarrier"",
                    ""id"": ""c4b6bbb7-17a7-4141-b15c-837ae131813f"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""ActivateScanner"",
                    ""id"": ""de6c26f9-bd4e-4b7f-8c2c-ee7c2da9ab44"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""ImpulseEngine"",
                    ""id"": ""76ac3142-f7a8-4211-a1d7-a7d01b35f9fe"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""WarpEngine"",
                    ""id"": ""aea0853b-56b3-4401-906d-c90447276950"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Pause"",
                    ""id"": ""8b83b777-5829-41e5-80b1-fb8880080647"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""978bfe49-cc26-4a3d-ab7b-7d7a29327403"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""143bb1cd-cc10-4eca-a2f0-a3664166fe91"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""FireGun"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""56c63ce9-9df5-4125-9e8a-6f1367670a3d"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""FireBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""a6e0d8f9-2fd8-4771-90ef-a70aebddb800"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""ActivateBarrier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""e3472de3-26a1-4db7-b870-f957c683d1fb"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""ActivateScanner"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""7e39a500-f41d-4245-9fca-fd59963daf59"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""ImpulseEngine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""5035c1d9-f036-46c2-9edc-82d43efe064b"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""WarpEngine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""6b9487c8-3813-4d85-8a60-09e1a5e362fe"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""cd133347-4b70-4776-9751-b781a1bd04c7"",
            ""actions"": [
                {
                    ""name"": ""Navigate"",
                    ""id"": ""ac34dc53-d692-4e7d-b590-d8f3926decbb"",
                    ""expectedControlLayout"": ""Vector2"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Submit"",
                    ""id"": ""3f18ca7a-5e3d-410d-ba13-1eec27dcf242"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Cancel"",
                    ""id"": ""6670ec2a-a8e6-442d-af5b-8ebac567eff7"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Point"",
                    ""id"": ""60056d68-e5c9-449b-9498-913afe5fd42a"",
                    ""expectedControlLayout"": ""Vector2"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Click"",
                    ""id"": ""c2303ce0-ee96-41e7-9663-c7d35b460cb3"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""ScrollWheel"",
                    ""id"": ""e995842c-c9d1-405b-b72e-d90726680103"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""MiddleClick"",
                    ""id"": ""b5ba6f82-a45b-4e9e-a48e-f643a7502e60"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""RightClick"",
                    ""id"": ""47fbab2b-4f16-4100-8a4f-546787b3a3a7"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""809f371f-c5e2-4e7a-83a1-d867598f40dd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""up"",
                    ""id"": ""14a5d6e8-4aaf-4119-a9ef-34b8c2c548bf"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2db08d65-c5fb-421b-983f-c71163608d67"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8ba04515-75aa-45de-966d-393d9bbd1c14"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fcd248ae-a788-4676-a12e-f4d81205600b"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""9e92bb26-7e3b-4ec4-b06b-3c8f8e498ddc"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""82627dcc-3b13-4ba9-841d-e4b746d6553e"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""c52c8e0b-8179-41d3-b8a1-d149033bbe86"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""4faf7dc9-b979-4210-aa8c-e808e1ef89f5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""38c99815-14ea-4617-8627-164d27641299"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""ScrollWheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""24066f69-da47-44f3-a07e-0015fb02eb2e"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""4c191405-5738-4d4b-a523-c6a301dbf754"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""basedOn"": """",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""basedOn"": """",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        this.m_Player = this.asset.GetActionMap("Player");
        this.m_Player_Aim = this.m_Player.GetAction("Aim");
        this.m_Player_FireGun = this.m_Player.GetAction("FireGun");
        this.m_Player_FireBomb = this.m_Player.GetAction("FireBomb");
        this.m_Player_ActivateBarrier = this.m_Player.GetAction("ActivateBarrier");
        this.m_Player_ActivateScanner = this.m_Player.GetAction("ActivateScanner");
        this.m_Player_ImpulseEngine = this.m_Player.GetAction("ImpulseEngine");
        this.m_Player_WarpEngine = this.m_Player.GetAction("WarpEngine");
        this.m_Player_Pause = this.m_Player.GetAction("Pause");
        // UI
        this.m_UI = this.asset.GetActionMap("UI");
        this.m_UI_Navigate = this.m_UI.GetAction("Navigate");
        this.m_UI_Submit = this.m_UI.GetAction("Submit");
        this.m_UI_Cancel = this.m_UI.GetAction("Cancel");
        this.m_UI_Point = this.m_UI.GetAction("Point");
        this.m_UI_Click = this.m_UI.GetAction("Click");
        this.m_UI_ScrollWheel = this.m_UI.GetAction("ScrollWheel");
        this.m_UI_MiddleClick = this.m_UI.GetAction("MiddleClick");
        this.m_UI_RightClick = this.m_UI.GetAction("RightClick");
    }

    ~InputActions()
    {
        UnityEngine.Object.Destroy(this.asset);
    }

    public InputBinding? bindingMask
    {
        get => this.asset.bindingMask;
        set => this.asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => this.asset.devices;
        set => this.asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes
    {
        get => this.asset.controlSchemes;
    }

    public bool Contains(InputAction action)
    {
        return this.asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return this.asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void Enable()
    {
        this.asset.Enable();
    }

    public void Disable()
    {
        this.asset.Disable();
    }

    // Player
    private InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private InputAction m_Player_Aim;
    private InputAction m_Player_FireGun;
    private InputAction m_Player_FireBomb;
    private InputAction m_Player_ActivateBarrier;
    private InputAction m_Player_ActivateScanner;
    private InputAction m_Player_ImpulseEngine;
    private InputAction m_Player_WarpEngine;
    private InputAction m_Player_Pause;
    public struct PlayerActions
    {
        private InputActions m_Wrapper;
        public PlayerActions(InputActions wrapper) { this.m_Wrapper = wrapper; }
        public InputAction @Aim { get { return this.m_Wrapper.m_Player_Aim; } }
        public InputAction @FireGun { get { return this.m_Wrapper.m_Player_FireGun; } }
        public InputAction @FireBomb { get { return this.m_Wrapper.m_Player_FireBomb; } }
        public InputAction @ActivateBarrier { get { return this.m_Wrapper.m_Player_ActivateBarrier; } }
        public InputAction @ActivateScanner { get { return this.m_Wrapper.m_Player_ActivateScanner; } }
        public InputAction @ImpulseEngine { get { return this.m_Wrapper.m_Player_ImpulseEngine; } }
        public InputAction @WarpEngine { get { return this.m_Wrapper.m_Player_WarpEngine; } }
        public InputAction @Pause { get { return this.m_Wrapper.m_Player_Pause; } }
        public InputActionMap Get() { return this.m_Wrapper.m_Player; }
        public void Enable() { this.Get().Enable(); }
        public void Disable() { this.Get().Disable(); }
        public bool enabled { get { return this.Get().enabled; } }
        public InputActionMap Clone() { return this.Get().Clone(); }
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if(this.m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                this.Aim.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                this.Aim.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                this.Aim.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                this.FireGun.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnFireGun;
                this.FireGun.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnFireGun;
                this.FireGun.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnFireGun;
                this.FireBomb.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnFireBomb;
                this.FireBomb.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnFireBomb;
                this.FireBomb.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnFireBomb;
                this.ActivateBarrier.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnActivateBarrier;
                this.ActivateBarrier.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnActivateBarrier;
                this.ActivateBarrier.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnActivateBarrier;
                this.ActivateScanner.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnActivateScanner;
                this.ActivateScanner.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnActivateScanner;
                this.ActivateScanner.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnActivateScanner;
                this.ImpulseEngine.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnImpulseEngine;
                this.ImpulseEngine.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnImpulseEngine;
                this.ImpulseEngine.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnImpulseEngine;
                this.WarpEngine.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnWarpEngine;
                this.WarpEngine.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnWarpEngine;
                this.WarpEngine.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnWarpEngine;
                this.Pause.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                this.Pause.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                this.Pause.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
            }
            this.m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if(instance != null)
            {
                this.Aim.started += instance.OnAim;
                this.Aim.performed += instance.OnAim;
                this.Aim.canceled += instance.OnAim;
                this.FireGun.started += instance.OnFireGun;
                this.FireGun.performed += instance.OnFireGun;
                this.FireGun.canceled += instance.OnFireGun;
                this.FireBomb.started += instance.OnFireBomb;
                this.FireBomb.performed += instance.OnFireBomb;
                this.FireBomb.canceled += instance.OnFireBomb;
                this.ActivateBarrier.started += instance.OnActivateBarrier;
                this.ActivateBarrier.performed += instance.OnActivateBarrier;
                this.ActivateBarrier.canceled += instance.OnActivateBarrier;
                this.ActivateScanner.started += instance.OnActivateScanner;
                this.ActivateScanner.performed += instance.OnActivateScanner;
                this.ActivateScanner.canceled += instance.OnActivateScanner;
                this.ImpulseEngine.started += instance.OnImpulseEngine;
                this.ImpulseEngine.performed += instance.OnImpulseEngine;
                this.ImpulseEngine.canceled += instance.OnImpulseEngine;
                this.WarpEngine.started += instance.OnWarpEngine;
                this.WarpEngine.performed += instance.OnWarpEngine;
                this.WarpEngine.canceled += instance.OnWarpEngine;
                this.Pause.started += instance.OnPause;
                this.Pause.performed += instance.OnPause;
                this.Pause.canceled += instance.OnPause;
            }
        }
    }
    public PlayerActions @Player
    {
        get
        {
            return new PlayerActions(this);
        }
    }

    // UI
    private InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private InputAction m_UI_Navigate;
    private InputAction m_UI_Submit;
    private InputAction m_UI_Cancel;
    private InputAction m_UI_Point;
    private InputAction m_UI_Click;
    private InputAction m_UI_ScrollWheel;
    private InputAction m_UI_MiddleClick;
    private InputAction m_UI_RightClick;
    public struct UIActions
    {
        private InputActions m_Wrapper;
        public UIActions(InputActions wrapper) { this.m_Wrapper = wrapper; }
        public InputAction @Navigate { get { return this.m_Wrapper.m_UI_Navigate; } }
        public InputAction @Submit { get { return this.m_Wrapper.m_UI_Submit; } }
        public InputAction @Cancel { get { return this.m_Wrapper.m_UI_Cancel; } }
        public InputAction @Point { get { return this.m_Wrapper.m_UI_Point; } }
        public InputAction @Click { get { return this.m_Wrapper.m_UI_Click; } }
        public InputAction @ScrollWheel { get { return this.m_Wrapper.m_UI_ScrollWheel; } }
        public InputAction @MiddleClick { get { return this.m_Wrapper.m_UI_MiddleClick; } }
        public InputAction @RightClick { get { return this.m_Wrapper.m_UI_RightClick; } }
        public InputActionMap Get() { return this.m_Wrapper.m_UI; }
        public void Enable() { this.Get().Enable(); }
        public void Disable() { this.Get().Disable(); }
        public bool enabled { get { return this.Get().enabled; } }
        public InputActionMap Clone() { return this.Get().Clone(); }
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if(this.m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                this.Navigate.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                this.Navigate.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                this.Navigate.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                this.Submit.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                this.Submit.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                this.Submit.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                this.Cancel.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                this.Cancel.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                this.Cancel.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                this.Point.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                this.Point.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                this.Point.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                this.Click.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                this.Click.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                this.Click.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                this.ScrollWheel.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                this.ScrollWheel.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                this.ScrollWheel.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                this.MiddleClick.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                this.MiddleClick.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                this.MiddleClick.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                this.RightClick.started -= this.m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                this.RightClick.performed -= this.m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                this.RightClick.canceled -= this.m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
            }
            this.m_Wrapper.m_UIActionsCallbackInterface = instance;
            if(instance != null)
            {
                this.Navigate.started += instance.OnNavigate;
                this.Navigate.performed += instance.OnNavigate;
                this.Navigate.canceled += instance.OnNavigate;
                this.Submit.started += instance.OnSubmit;
                this.Submit.performed += instance.OnSubmit;
                this.Submit.canceled += instance.OnSubmit;
                this.Cancel.started += instance.OnCancel;
                this.Cancel.performed += instance.OnCancel;
                this.Cancel.canceled += instance.OnCancel;
                this.Point.started += instance.OnPoint;
                this.Point.performed += instance.OnPoint;
                this.Point.canceled += instance.OnPoint;
                this.Click.started += instance.OnClick;
                this.Click.performed += instance.OnClick;
                this.Click.canceled += instance.OnClick;
                this.ScrollWheel.started += instance.OnScrollWheel;
                this.ScrollWheel.performed += instance.OnScrollWheel;
                this.ScrollWheel.canceled += instance.OnScrollWheel;
                this.MiddleClick.started += instance.OnMiddleClick;
                this.MiddleClick.performed += instance.OnMiddleClick;
                this.MiddleClick.canceled += instance.OnMiddleClick;
                this.RightClick.started += instance.OnRightClick;
                this.RightClick.performed += instance.OnRightClick;
                this.RightClick.canceled += instance.OnRightClick;
            }
        }
    }
    public UIActions @UI
    {
        get
        {
            return new UIActions(this);
        }
    }
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if(this.m_KeyboardMouseSchemeIndex == -1)
            {
                this.m_KeyboardMouseSchemeIndex = this.asset.GetControlSchemeIndex("Keyboard&Mouse");
            }
            return this.asset.controlSchemes[this.m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if(this.m_GamepadSchemeIndex == -1)
            {
                this.m_GamepadSchemeIndex = this.asset.GetControlSchemeIndex("Gamepad");
            }
            return this.asset.controlSchemes[this.m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnAim(InputAction.CallbackContext context);
        void OnFireGun(InputAction.CallbackContext context);
        void OnFireBomb(InputAction.CallbackContext context);
        void OnActivateBarrier(InputAction.CallbackContext context);
        void OnActivateScanner(InputAction.CallbackContext context);
        void OnImpulseEngine(InputAction.CallbackContext context);
        void OnWarpEngine(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnNavigate(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
        void OnScrollWheel(InputAction.CallbackContext context);
        void OnMiddleClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
    }
}
