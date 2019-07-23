using System.Collections.Generic;

using UnityEngine;

public static class UIControllerNew
{
    // UIScreens
    private static Dictionary<string, UIScreen> UIScreens = new Dictionary<string, UIScreen>();
    private static UIScreen CurrentScreen;

    // Screen names
    private const string MainMenu = "Main Menu Screen";
    private const string NewGameMenu = "New Game Menu Screen";
    private const string LoadGameMenu = "Load Game Menu Screen";
    private const string SettingsMenu = "Settings Menu Screen";
    private const string PlayerUI = "Player UI Screen";

    // Initialize
    public static void Initialize()
    {
        foreach(GameObject go in UIHandler.Instance.UIScreenObjects)
        {
            UIScreens.Add(go.name, go.GetComponent<UIScreen>());
        }
        CurrentScreen = UIScreens[UIHandler.Instance.UIScreenObjects[0].name];
        ChangeScreen(CurrentScreen);
    }

    // Update is called once per frame
    public static void Update()
    {
        if(CurrentScreen == UIScreens[MainMenu])
        {

        }
    }

    // Change screen
    public static void ChangeScreen(UIScreen _newScreen)
    {
        CurrentScreen.SlideScreen(UIScreen.InOutEnum.Out);
        CurrentScreen = _newScreen;
        CurrentScreen.SlideScreen(UIScreen.InOutEnum.In);
    }
}
