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

    // Change game state
    public void ChangeGameState(GameController.GameState _newGameState)
    {
        GameController.ChangeGameState(_newGameState);
        UIController.ChangeGameState(_newGameState);
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

    // Start game
    public void StartNewGame()
    {
        UIController.StartNewGame();
    }

    // Restart
    public void Restart()
    {
        GameController.Restart();
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}
