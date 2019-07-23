using UnityEngine;

public class UIHandler : MonoBehaviour
{
    // Singleton
    public static UIHandler Instance { get; private set; }

    // UI screen objects
    [SerializeField]
    public GameObject[] UIScreenObjects;

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
    }

    // Handle button input
    public void ButtonPress(GameObject _button)
    {
        switch(_button.name)
        {
            case "":
            {
                break;
            }
        }
    }

    // Change screen
    public void ChangeScreen(GameObject _newScreen)
    {
        UIControllerNew.ChangeScreen(_newScreen.GetComponent<UIScreen>());
    }
}
