using System.Collections.Generic;

using TMPro;

using UnityEngine;

// Controls the UI
public static class UIControllerNew
{
    // UIScreens and UIPopUps
    private static Dictionary<string, UIScreen> UIScreens = new Dictionary<string, UIScreen>();
    private static Dictionary<string, UIPopUp> UIPopUps = new Dictionary<string, UIPopUp>();
    private static UIScreen CurrentScreen;

    // Error Text
    private static TextMeshProUGUI ErrorText;
    private static bool ErrorTextVisible = false;
    private static float ErrorTextLastChangedTime;
    private const float ErrorTextDuration = 4f;
    private const float ErrorTextFadeRate = 0.2f;

    // Constant names
    // Screen names
    private const string MainMenu = "Main Menu Screen";
    private const string NewGameMenu = "New Game Menu Screen";
    private const string LoadGameMenu = "Load Game Menu Screen";
    private const string SettingsMenu = "Settings Menu Screen";
    private const string PlayerUI = "Player UI Screen";
    // Pop Up names
    private const string QuitConfirmPopUP = "Quit Confirm PopUp";


    // Initialize
    public static void Initialize()
    {
        // Loop through all screens from UIHandler
        foreach(UIScreen screen in UIHandler.Instance.UIScreens)
        {
            // Add all UIScreens to dictionary
            UIScreens.Add(screen.name, screen.GetComponent<UIScreen>());
        }
        // Set current screen to default screen
        CurrentScreen = UIHandler.Instance.DefaultScreen;
        // Change screen to current screen, maybe redundant
        ChangeScreen(CurrentScreen);
        // Loop through all screens
        foreach(KeyValuePair<string, UIScreen> screen in UIScreens)
        {
            // If screen is not current screen
            if(screen.Key != CurrentScreen.gameObject.name)
            {
                // Deactivate screen
                screen.Value.gameObject.SetActive(false);
            }
        }
        // Loop through all pop ups from UIHandler
        foreach(UIPopUp popUp in UIHandler.Instance.UIPopUps)
        {
            // Add all UIPopUps to dictionary
            UIPopUps.Add(popUp.name, popUp);
        }
        // Loop through all pop ups
        foreach(KeyValuePair<string, UIPopUp> popUp in UIPopUps)
        {
            // If pop up is not marked default active
            if(popUp.Value.DefaultActive != true)
            {
                // Deactivate pop up
                popUp.Value.gameObject.SetActive(false);
            }
        }
        // Get error text reference
        ErrorText = UIHandler.Instance.ErrorText.GetComponent<TextMeshProUGUI>();
        // Initalize error text to fully transparent
        ErrorText.alpha = 0f;
    }

    // Update is called once per frame
    public static void Update()
    {
        UpdateErrorText();
        // If current screen is Main Menu
        if(CurrentScreen == UIScreens[MainMenu])
        {

        }
        // If current screen is New Game Menu
        else if(CurrentScreen == UIScreens[NewGameMenu])
        {

        }
        // If current screen is Settings Menu
        else if(CurrentScreen == UIScreens[SettingsMenu])
        {

        }
        // If current screen is Player UI
        else if(CurrentScreen == UIScreens[PlayerUI])
        {

        }
    }

    // Update error text
    public static void UpdateErrorText()
    {
        // If error text is visible and has been for longer than error text duration
        if(ErrorTextVisible == true && Time.time - ErrorTextLastChangedTime >= ErrorTextDuration)
        {
            // Fade out error text
            FadeOutErrorText();
            // If error text is fully transparent
            if(ErrorText.alpha == 0f)
            {
                // Mark error text as not visible
                ErrorTextVisible = false;
            }
        }
    }

    // Change screen
    public static void ChangeScreen(UIScreen _newScreen)
    {
        // Slide out current screen
        CurrentScreen.SetupSlideScreen(UIScreen.InOutEnum.Out);
        // Set current screen to new screen
        CurrentScreen = _newScreen;
        // Slide in new screen
        CurrentScreen.SetupSlideScreen(UIScreen.InOutEnum.In);
    }

    // Back
    public static void Back()
    {
        // Change screen to the back screen of the current screen
        ChangeScreen(CurrentScreen.BackScreen);
    }

    // Open Popup
    public static void OpenPopUp(UIPopUp _newPopUp)
    {
        // Activate new pop up
        _newPopUp.gameObject.SetActive(true);
        // If pop up is supposed to be only pop up on screen
        if(_newPopUp.OnlyPopUp == true)
        {
            // Loop through all pop ups on this screen
            foreach(KeyValuePair<string, UIPopUp> popUp in UIPopUps)
            {
                // If pop up is not the new pop up
                if(popUp.Value != _newPopUp)
                {
                    // Deactivate pop up
                    popUp.Value.gameObject.SetActive(false);
                }
            }
        }
    }

    // Close Popup
    public static void ClosePopUp(UIPopUp _popUp)
    {
        // Deactivate pop up
        _popUp.gameObject.SetActive(false);
    }

    // Change error text
    public static void ChangeErrorText(string _newText)
    {
        // Update text
        ErrorText.text = _newText;
        // Update alpha to 1
        ErrorText.alpha = 1f;
        // Error text is visible
        ErrorTextVisible = true;
        // Get last changed time
        ErrorTextLastChangedTime = Time.time;
    }

    // Fade out error text
    public static void FadeOutErrorText()
    {
        // Lerp alpha toward 0
        ErrorText.alpha = Mathf.Lerp(ErrorText.alpha, 0f, ErrorTextFadeRate);
    }

    // Start new game
    public static void StartNewGame()
    {
        // TODO: Initialize new game UI stuff here
    }
}
