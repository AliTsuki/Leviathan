using UnityEngine;

// Controls UI menu input
public class UIInputHandler : MonoBehaviour
{
    // Main menu input
    public void UIInput(string _input)
    {
        if(_input == "ContinueGame")
        {
            // TODO: Loads most recent save file
        }
        else if(_input == "NewGame")
        {
            UIController.enteredNewState = false;
            GameController.CurrentGameState = GameController.GameState.NewGameMenu;
        }
        else if(_input == "SelectBomber")
        {
            GameController.PlayerShipType = PlayerShip.PlayerShipType.Bomber;
        }
        else if(_input == "SelectEngineer")
        {
            GameController.PlayerShipType = PlayerShip.PlayerShipType.Engineer;
        }
        else if(_input == "StartGame")
        {
            UIController.enteredNewState = false;
            GameController.Restart();
            GameController.CurrentGameState = GameController.GameState.Playing;
        }
        else if(_input == "NewGameBackButton")
        {
            UIController.enteredNewState = false;
            UIController.ExitNewGameMenu();
        }
        else if(_input == "LoadGame")
        {
            // TODO: Add a load game menu
        }
        else if(_input == "Settings")
        {
            UIController.enteredNewState = false;
            GameController.CurrentGameState = GameController.GameState.SettingsMenu;
        }
        else if(_input == "SettingsBackButton")
        {
            UIController.enteredNewState = false;
            UIController.ExitSettingsMenu();
        }
        else if(_input == "Quit")
        {
            Application.Quit();
        }
        else if(_input == "Restart")
        {
            UIController.enteredNewState = false;
            GameController.Restart();
        }
        else if(_input == "Resume")
        {
            UIController.enteredNewState = false;
            GameController.CurrentGameState = GameController.GameState.Playing;
        }
        else if(_input == "QuitToMainMenu")
        {
            UIController.enteredNewState = false;
            GameController.Restart();
            GameController.CurrentGameState = GameController.GameState.MainMenu;
        }
    }
}
