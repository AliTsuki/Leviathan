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
    public void ChangeErrorText(string _newErrorText)
    {
        UIController.ChangeErrorText(_newErrorText);
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
    public void RebindKeybindKBM(string _inputToRebind)
    {
        PlayerInput.SetupRebind(PlayerInput.InputModeEnum.KBM, _inputToRebind);
    }

    // Rebind keybind controller
    public void RebindKeybindController(string _inputToRebind)
    {
        PlayerInput.SetupRebind(PlayerInput.InputModeEnum.Controller, _inputToRebind);
    }

    // Change game state
    public void ChangeGameState(int _newGameState)
    {
        // int 0: menus, 1: playing, 2: paused
        switch(_newGameState)
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
                Debug.Log($@"Invalid Game State: {_newGameState}");
                Logger.Log($@"Invalid Game State: {_newGameState}");
                break;
            }
        }
    }

    // Change screen
    public void ChangeScreen(UIScreen _newScreen)
    {
        UIController.ChangeScreen(_newScreen);
    }

    // Back
    public void Back()
    {
        UIController.Back();
    }

    // Open PopUp
    public void OpenPopUp(UIPopUp _popUpToOpen)
    {
        UIController.OpenPopUp(_popUpToOpen);
    }

    // Close PopUp
    public void ClosePopUp(UIPopUp _popUpToClose)
    {
        UIController.ClosePopUp(_popUpToClose);
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
