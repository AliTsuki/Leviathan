using UnityEngine;
using UnityEngine.InputSystem;

// Controls UI menu input
public class InputHandler : MonoBehaviour
{
    // Main menu input
    public void MainMenuInput(string _input)
    {
        if(_input == "ContinueGame")
        {
            // TODO: Loads most recent save file
        }
        else if(_input == "NewGame")
        {
            // TODO: Start a new game
            GameController.CurrentGameState = GameController.GameState.Playing;
            UIController.ShowUI();
        }
        else if(_input == "LoadGame")
        {
            // TODO: Add a load game menu
        }
        else if(_input == "Settings")
        {
            // TODO: Add a settings screen, rebind keys, change graphics, etc
        }
        else if(_input == "Quit")
        {
            Application.Quit();
        }
        else if(_input == "Restart")
        {
            GameController.Restart();
        }
    }

    // InputActions
    //public InputAction AimInput;
    //public InputAction FireGunInput;
    //public InputAction FireBombInput;
    //public InputAction ActivateBarrierInput;
    //public InputAction ActivateBarrageInput;
    //public InputAction ImpulseEngineInput;
    //public InputAction WarpEngineInput;
    //public InputAction PauseInput;

    // Input fields
    //public static Vector2 CurrentAim;
    //public static bool FireGunActive = false;
    //public static bool FireBombActive = false;
    //public static bool ActivateBarrierActive = false;
    //public static bool ActivateBarrageActive = false;
    //public static bool ImpulseEngineActive = false;
    //public static bool WarpEngineActive = false;
    //public static bool PauseActive = false;

    //void Awake()
    //{
    //    this.AimInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/Stick");
    //    this.AimInput.performed += context => this.Aim();
    //    this.FireGunInput.continuous = true;
    //    this.FireGunInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button2");
    //    this.FireGunInput.performed += context => this.FireGun();
    //    this.FireBombInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button3");
    //    this.FireBombInput.performed += context => this.FireBomb();
    //    this.ActivateBarrierInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button1");
    //    this.ActivateBarrierInput.performed += context => this.ActivateBarrier();
    //    this.ActivateBarrageInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button4");
    //    this.ActivateBarrageInput.performed += context => this.ActivateBarrage();
    //    this.ImpulseEngineInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button8");
    //    this.ImpulseEngineInput.performed += context => this.Impulse();
    //    this.WarpEngineInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button7");
    //    this.WarpEngineInput.performed += context => this.Warp();
    //    this.PauseInput.AddBinding("<HID::Logitech Logitech Dual Action Joystick>/button10");
    //    this.PauseInput.performed += context => this.Pause();
    //}

    //public void OnEnable()
    //{
    //    this.AimInput.Enable();
    //    this.FireGunInput.Enable();
    //    this.FireBombInput.Enable();
    //    this.ActivateBarrierInput.Enable();
    //    this.ActivateBarrageInput.Enable();
    //    this.ImpulseEngineInput.Enable();
    //    this.WarpEngineInput.Enable();
    //    this.PauseInput.Enable();
    //}

    //public void OnDisable()
    //{
    //    this.AimInput.Disable();
    //    this.FireGunInput.Disable();
    //    this.FireBombInput.Disable();
    //    this.ActivateBarrierInput.Disable();
    //    this.ActivateBarrageInput.Disable();
    //    this.ImpulseEngineInput.Disable();
    //    this.WarpEngineInput.Disable();
    //    this.PauseInput.Disable();
    //}

    //public void ClearInputs()
    //{
    //    CurrentAim = new Vector2();
    //    FireGunActive = false;
    //    FireBombActive = false;
    //    ActivateBarrierActive = false;
    //    ActivateBarrageActive = false;
    //    ImpulseEngineActive = false;
    //    WarpEngineActive = false;
    //    PauseActive = false;
    //}

    //public void Update()
    //{
    //    this.ClearInputs();
    //}

    //public void Aim()
    //{
    //    //CurrentAim = AimInput.
    //}

    //public void FireGun()
    //{
    //    Debug.Log($@"Fire Gun Input Pressed");
    //    FireGunActive = true;
    //    //FireGunActive = this.FireGunInput.CallbackContext.ReadValue();
    //}

    //public void FireBomb()
    //{
    //    FireBombActive = true;
    //}

    //public void ActivateBarrier()
    //{
    //    ActivateBarrierActive = true;
    //}

    //public void ActivateBarrage()
    //{
    //    ActivateBarrageActive = true;
    //}

    //public void Impulse()
    //{
    //    ImpulseEngineActive = true;
    //}

    //public void Warp()
    //{
    //    WarpEngineActive = true;
    //}

    //public void Pause()
    //{
    //    PauseActive = true;
    //}
}
