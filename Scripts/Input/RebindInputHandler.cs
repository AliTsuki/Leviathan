using UnityEngine;

// Controls rebind menu input
public class RebindInputHandler : MonoBehaviour
{
    // Main menu input
    public void RebindInput(string _input)
    {
        if(_input == "Main Gun")
        {
            PlayerInput.RebindingInputs = true;
            PlayerInput.RebindingStartedTime = Time.time;
            PlayerInput.InputToRebind = InputBinding.GameInputsEnum.MainGun;
            UIController.UpdateSettingsErrorText($@"Rebinding Main Gun");
            Logger.Log($@"Rebinding Main Gun");
        }
        else if(_input == "Ability 1")
        {
            PlayerInput.RebindingInputs = true;
            PlayerInput.RebindingStartedTime = Time.time;
            PlayerInput.InputToRebind = InputBinding.GameInputsEnum.Ability1;
            UIController.UpdateSettingsErrorText($@"Rebinding Ability 1");
            Logger.Log($@"Rebinding Ability 1");
        }
        else if(_input == "Ability 2")
        {
            PlayerInput.RebindingInputs = true;
            PlayerInput.RebindingStartedTime = Time.time;
            PlayerInput.InputToRebind = InputBinding.GameInputsEnum.Ability2;
            UIController.UpdateSettingsErrorText($@"Rebinding Ability 2");
            Logger.Log($@"Rebinding Ability 2");
        }
        else if(_input == "Ability 3")
        {
            PlayerInput.RebindingInputs = true;
            PlayerInput.RebindingStartedTime = Time.time;
            PlayerInput.InputToRebind = InputBinding.GameInputsEnum.Ability3;
            UIController.UpdateSettingsErrorText($@"Rebinding Ability 3");
            Logger.Log($@"Rebinding Ability 3");
        }
    }
}
