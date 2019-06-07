using UnityEngine;

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
            GameController.Restart();
            GameController.CurrentGameState = GameController.GameState.Playing;
        }
        else if(_input == "BackButton")
        {
            UIController.ExitNewGameMenu();
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
        else if(_input == "Resume")
        {
            GameController.CurrentGameState = GameController.GameState.Playing;
        }
        else if(_input == "QuitToMainMenu")
        {
            GameController.Restart();
            GameController.CurrentGameState = GameController.GameState.MainMenu;
        }
    }
}
