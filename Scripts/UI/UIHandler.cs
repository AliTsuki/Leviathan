using UnityEngine;

// Contains list of all screens and button behaviours for those screens
public class UIHandler : MonoBehaviour
{
    // Singleton
    public static UIHandler Instance { get; private set; }

    // UI screen objects
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

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
    }

    // Change error text
    public void ChangeErrorText(string _newErrorText)
    {
        UIControllerNew.ChangeErrorText(_newErrorText);
    }

    // Select player ship type
    public void SelectPlayerShipType(PlayerShip.PlayerShipType _type)
    {
        GameController.ChangePlayerShipType(_type);
        UIControllerNew.ViewNewShipType(_type);
    }

    // Change screen
    public void ChangeScreen(UIScreen _newScreen)
    {
        UIControllerNew.ChangeScreen(_newScreen);
    }

    // Back
    public void Back()
    {
        UIControllerNew.Back();
    }

    // Open PopUp
    public void OpenPopUp(UIPopUp _popUpToOpen)
    {
        UIControllerNew.OpenPopUp(_popUpToOpen);
    }

    // Close PopUp
    public void ClosePopUp(UIPopUp _popUpToClose)
    {
        UIControllerNew.ClosePopUp(_popUpToClose);
    }

    // Start game
    public void StartNewGame()
    {
        UIControllerNew.StartNewGame();
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}
