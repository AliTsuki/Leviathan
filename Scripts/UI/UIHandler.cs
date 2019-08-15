using UnityEngine;

// Contains list of all screens and button behaviours
public class UIHandler : MonoBehaviour
{
    // Singleton
    public static UIHandler Instance { get; private set; }

    // UI screen objects
    [Header("Aim Cursor")]
    [SerializeField]
    public Texture2D AimCursor;
    [Header("Screens")]
    [SerializeField]
    public UIScreen[] UIScreens;
    [SerializeField]
    public UIScreen DefaultScreen;
    [Header("PopUps")]
    [SerializeField]
    public UIPopUp[] UIPopUps;
    [Header("Error Text")]
    [SerializeField]
    public GameObject ErrorText;
    [Header("Menu Background")]
    [SerializeField]
    public GameObject MenuBackground;
    [Header("NPC UI Prefab")]
    [SerializeField]
    public GameObject NPCUIPrefab;
    [Header("Ship Models")]
    [SerializeField]
    public GameObject[] ShipModels;


    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
    }

    // Change error text
    public void ChangeErrorText(string newErrorText)
    {
        UIController.ChangeErrorText(newErrorText);
    }

    // Get selected toggle
    public void GetShipSelectToggle()
    {
        UIController.GetShipSelectToggle();
    }

    // Movement style toggle
    public void MovementStyleToggle()
    {
        UIController.GetMovementStyleToggle();
    }

    // Input type toggle
    public void InputTypeToggle()
    {
        UIController.GetInputTypeToggle();
    }

    // Rebind keybind KBM
    public void RebindKeybindKBM(string inputToRebind)
    {
        PlayerInput.SetupRebind(PlayerInput.InputModeEnum.KeyboardAndMouse, inputToRebind);
    }

    // Rebind keybind controller
    public void RebindKeybindController(string inputToRebind)
    {
        PlayerInput.SetupRebind(PlayerInput.InputModeEnum.Controller, inputToRebind);
    }

    // Set bindings to default
    public void SetDefaultBindings()
    {
        PlayerInput.SetDefaultBindings();
        UIController.ChangeErrorText($@"Input bindings set to defaults.");
    }

    // Save settings to file
    public void SaveSettingsToFile()
    {
        FileOps.WriteSettingsToFile();
    }

    // Change game state
    public void ChangeGameState(int newGameState)
    {
        // int 0: menus, 1: playing, 2: paused
        switch(newGameState)
        {
            case 0:
            {
                GameController.ChangeGameState(GameController.GameState.Menus);
                break;
            }
            case 1:
            {
                GameController.ChangeGameState(GameController.GameState.Playing);
                break;
            }
            case 2:
            {
                GameController.ChangeGameState(GameController.GameState.Paused);
                break;
            }
            default:
            {
                Debug.Log($@"Invalid Game State: {newGameState}");
                Logger.Log($@"Invalid Game State: {newGameState}");
                break;
            }
        }
    }

    // Change screen
    public void ChangeScreen(UIScreen newScreen)
    {
        UIController.ChangeScreen(newScreen);
    }

    // Back
    public void Back()
    {
        UIController.Back();
    }

    // Open PopUp
    public void OpenPopUp(UIPopUp popUpToOpen)
    {
        UIController.OpenPopUp(popUpToOpen);
    }

    // Close PopUp
    public void ClosePopUp(UIPopUp popUpToClose)
    {
        UIController.ClosePopUp(popUpToClose);
    }

    // Close all PopUps
    public void CloseAllPopUps()
    {
        UIController.CloseAllPopUps();
    }

    // Clear all
    public void ClearAll()
    {
        GameController.ClearAll();
    }

    // Start game
    public void StartNewGame()
    {
        GameController.StartNewGame();
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}
