using UnityEngine;

// Controls the player ships
public class PlayerShip : Ship
{
    // Player ship types
    public enum PlayerShipType
    {
        Bomber,
        Engineer,
        Scout
    }


    // Processes inputs
    public override void ProcessInputs()
    {
        // Get inputs from PlayerInput
        this.AimInput = PlayerInput.AimInput;
        this.ImpulseInput = PlayerInput.ImpulseEngineInput;
        this.WarpEngineInput = PlayerInput.WarpEngineInput;
        this.MainGunInput = PlayerInput.MainGunInput;
        this.Ability1Input = PlayerInput.Ability1Input;
        this.Ability2Input = PlayerInput.Ability2Input;
        this.Ability3Input = PlayerInput.Ability3Input;
        this.PauseInput = PlayerInput.PauseButtonInput;
        // Pause game if pause is pressed
        if(this.PauseInput == true)
        {
            GameController.CurrentGameState = GameController.GameState.Paused;
        }
    }

    // Gets intended rotation
    public override void GetIntendedRotation()
    {
        // If joystick is pointed in some direction
        if(this.AimInput.x != 0 || this.AimInput.y != 0)
        {
            // Set intended angle to dual-argument arc-tangent of vertical input and horizontal input, finally multiply by constant to convert from rad to deg, add 90 to compensate for rotation of camera
            this.IntendedAngle = (Mathf.Atan2(this.AimInput.y, this.AimInput.x) * Mathf.Rad2Deg) + 90;
            // Set intended rotation to intended angle on y axis
            this.IntendedRotation = Quaternion.Euler(new Vector3(0, this.IntendedAngle, 0));
        }
    }
}
