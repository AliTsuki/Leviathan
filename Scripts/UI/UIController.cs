using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Controls the UI
public static class UIController
{
    // UIScreens and UIPopUps
    private static Dictionary<string, UIScreen> UIScreens = new Dictionary<string, UIScreen>();
    private static Dictionary<string, UIPopUp> UIPopUps = new Dictionary<string, UIPopUp>();
    private static UIScreen CurrentScreen;
    private static List<UIPopUp> ActivePopUps = new List<UIPopUp>();
    // Screen object references
    private static GameObject MainMenuScreen;
    private static GameObject ShipSelectMenuScreen;
    private static GameObject SettingsMenuScreen;
    private static GameObject PlayerUIScreen;
    // Screen Elements
    private static UIElementsMainMenu MainMenuElements;
    private static UIElementsShipSelect ShipSelectElements;
    private static UIElementsSettings SettingsElements;
    private static UIElementsPlayerUI PlayerUIElements;
    private static UIPlayerAbilityIcons PlayerAbilityIcons;
    // PopUps
    private static UIPopUp BomberInfoPopUp;
    private static UIPopUp EngineerInfoPopUp;
    private static UIPopUp GameOverPopUp;
    private static UIPopUp PausePopUp;
    private static UIPopUp QuitConfirmPopUp;
    private static UIPopUp MainMenuConfirmPopUp;
    private static UIPopUp RebindPopUp;
    // PopUp Elements
    private static UIElementsRebind RebindElements;

    // Universals
    // Event system
    private static EventSystem EventSys;
    private static GameObject LastSelectedButton;
    // Cursor
    private static Texture2D AimCursorTexture;
    private static Vector2 AimCursorHotspot;
    // Mouse position
    private static Vector2 MousePos = new Vector2();
    private static float TimeMouseLastMove = 0f;
    private const float MouseDisappearTimer = 3f;
    // Background
    private static GameObject MenuBackground;
    // Error text
    private static TextMeshProUGUI ErrorText;
    private static bool ErrorTextVisible = false;
    private static float ErrorTextLastChangedTime;
    private const float ErrorTextDuration = 3f;
    private const float ErrorTextFadeRate = 0.05f;

    // Main menu
    // Version text
    private static TextMeshProUGUI VersionText;

    // Ship select
    private static Dictionary<string, GameObject> ShipModels = new Dictionary<string, GameObject>();
    private static GameObject BomberModel;
    private static GameObject EngineerModel;

    // Player UI
    // Dictionary of Healthbar UIs
    private static Dictionary<uint, GameObject> Healthbars = new Dictionary<uint, GameObject>();
    // NPC UIs
    private static RectTransform PlayerUIRectTransform;
    private static GameObject NPCUIPrefab;
    private static GameObject HealthbarUI;
    private static Vector2 UIOffset;
    // Info label
    private static TextMeshProUGUI InfoLabel;
    private static float deltaTime = 0.0f;
    private static int FPS;
    private static int Seconds = 0;
    private static int Minutes = 0;
    private static int Hours = 0;
    private static string TimeString = "";
    // Minimap
    private static TextMeshProUGUI MinimapCoords;
    // Player stats
    private static Image PlayerHealthForeground;
    private static TextMeshProUGUI PlayerHealthText;
    private static Image PlayerShieldForeground;
    private static TextMeshProUGUI PlayerShieldText;
    private static Image PlayerEnergyForeground;
    private static TextMeshProUGUI PlayerEnergyText;
    private static Image PlayerAbility1Background;
    private static Image PlayerAbility1Icon;
    private static Image PlayerAbility1Cooldown;
    private static TextMeshProUGUI PlayerAbility1CDText;
    private static Image PlayerAbility2Background;
    private static Image PlayerAbility2Icon;
    private static Image PlayerAbility2Cooldown;
    private static TextMeshProUGUI PlayerAbility2CDText;
    private static Image PlayerAbility3Background;
    private static Image PlayerAbility3Icon;
    private static Image PlayerAbility3Cooldown;
    private static TextMeshProUGUI PlayerAbility3CDText;
    // FX
    private static GameObject ShieldDamageEffect;
    private static GameObject HealthDamageEffect;
    private static float ShieldDamageEffectStartTime = 0f;
    private static float HealthDamageEffectStartTime = 0f;
    private const float ShowDamageEffectDuration = 0.25f;

    // Constant names
    // Screen names
    private const string MainMenuName = "Main Menu Screen";
    private const string ShipSelectMenuName = "Ship Select Menu Screen";
    private const string SettingsMenuName = "Settings Menu Screen";
    private const string PlayerUIName = "Player UI Screen";
    // PopUp names
    private const string BomberInfoPopUpName = "Bomber Info PopUp";
    private const string EngineerInfoPopUpName = "Engineer Info PopUp";
    private const string GameOverPopUpName = "Game Over PopUp";
    private const string PausePopUpName = "Pause PopUp";
    private const string QuitConfirmPopUpName = "Quit Confirm PopUp";
    private const string MainMenuConfirmPopUpName = "Main Menu Confirm PopUp";
    private const string RebindPopUpName = "Rebind PopUp";
    // Model names
    private const string BomberModelName = "Bomber Model";
    private const string EngineerModelName = "Engineer Model";


    // Initialize
    public static void Initialize()
    {
        // Initialize all references
        AddScreens();
        AddPopUps();
        InitializeUIObjectReferences();
        // Initialize all states
        InitializeScreenState();
        InitializePopUpState();
        // Confine cursor to screen
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    public static void Update()
    {
        UpdateErrorText();
        // If current screen is Main Menu
        if(CurrentScreen == UIScreens[MainMenuName])
        {

        }
        // If current screen is New Game Menu
        else if(CurrentScreen == UIScreens[ShipSelectMenuName])
        {
            
        }
        // If current screen is Settings Menu
        else if(CurrentScreen == UIScreens[SettingsMenuName])
        {
            // If rebind popup is active
            if(ActivePopUps.Contains(RebindPopUp))
            {
                UpdateRebindButtonText();
            }
        }
        // If current screen is Player UI
        else if(CurrentScreen == UIScreens[PlayerUIName])
        {
            UpdateMinimapCoords();
            UpdateInfoLabel();
            UpdateNPCHealthbars();
            UpdatePlayerUI();
        }
        // If input mode is KBM
        if(PlayerInput.InputMode == PlayerInput.InputModeEnum.KeyboardAndMouse)
        {
            // Deselect any selected items
            EventSys.SetSelectedGameObject(null);
        }
        // If input mode is controller
        else
        {
            // If mouse has been moved since last frame
            if(Vector2.Distance(new Vector2(Input.mousePosition.x, Input.mousePosition.y), MousePos) > 0.01)
            {
                // Show cursor
                Cursor.visible = true;
                // Set time mouse moved last
                TimeMouseLastMove = Time.time;
            }
            // If mouse hasn't been moved, and it has been longer than mouse disappear timer length in time passed
            else if(Time.time - TimeMouseLastMove > MouseDisappearTimer)
            {
                // Hide cursor
                Cursor.visible = false;
            }
            // Get mouse position
            MousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    // Add screens
    private static void AddScreens()
    {
        // Loop through all screens from UIHandler
        foreach(UIScreen screen in UIHandler.Instance.UIScreens)
        {
            // Add all UIScreens to dictionary
            UIScreens.Add(screen.name, screen.GetComponent<UIScreen>());
        }
    }

    // Add popups
    private static void AddPopUps()
    {
        // Loop through all popups from UIHandler
        foreach(UIPopUp popUp in UIHandler.Instance.UIPopUps)
        {
            // Add all UIPopUps to dictionary
            UIPopUps.Add(popUp.name, popUp);
        }
    }

    // Initialize UI object references
    private static void InitializeUIObjectReferences()
    {
        // Get event system reference
        EventSys = EventSystem.current;
        // Cursor
        AimCursorTexture = UIHandler.Instance.AimCursor;
        AimCursorHotspot = new Vector2(AimCursorTexture.width / 2, AimCursorTexture.height / 2);
        // Background
        MenuBackground = UIHandler.Instance.MenuBackground;
        // Get error text reference
        ErrorText = UIHandler.Instance.ErrorText.GetComponent<TextMeshProUGUI>();
        // Initalize error text to fully transparent
        ErrorText.alpha = 0f;
        // Screen Objects
        MainMenuScreen = UIScreens[MainMenuName].gameObject;
        ShipSelectMenuScreen = UIScreens[ShipSelectMenuName].gameObject;
        SettingsMenuScreen = UIScreens[SettingsMenuName].gameObject;
        PlayerUIScreen = UIScreens[PlayerUIName].gameObject;
        // Elements
        MainMenuElements = MainMenuScreen.GetComponent<UIElementsMainMenu>();
        ShipSelectElements = ShipSelectMenuScreen.GetComponent<UIElementsShipSelect>();
        SettingsElements = SettingsMenuScreen.GetComponent<UIElementsSettings>();
        PlayerUIElements = PlayerUIScreen.GetComponent<UIElementsPlayerUI>();
        PlayerAbilityIcons = PlayerUIScreen.GetComponent<UIPlayerAbilityIcons>();
        // PopUps
        BomberInfoPopUp = UIPopUps[BomberInfoPopUpName];
        EngineerInfoPopUp = UIPopUps[EngineerInfoPopUpName];
        GameOverPopUp = UIPopUps[GameOverPopUpName];
        PausePopUp = UIPopUps[PausePopUpName];
        QuitConfirmPopUp = UIPopUps[QuitConfirmPopUpName];
        MainMenuConfirmPopUp = UIPopUps[MainMenuConfirmPopUpName];
        RebindPopUp = UIPopUps[RebindPopUpName];
        // PopUp Elements
        RebindElements = RebindPopUp.GetComponent<UIElementsRebind>();
        // Main menu
        VersionText = MainMenuElements.VersionText;
        // Set version text
        VersionText.text = GameController.Version;
        // Ship select
        foreach(GameObject model in UIHandler.Instance.ShipModels)
        {
            ShipModels.Add(model.name, model);
        }
        BomberModel = ShipModels[BomberModelName];
        EngineerModel = ShipModels[EngineerModelName];
        HideAllShipModels();
        // NPC UI
        NPCUIPrefab = UIHandler.Instance.NPCUIPrefab;
        // Player UI
        PlayerUIRectTransform = PlayerUIScreen.GetComponent<RectTransform>();
        InfoLabel = PlayerUIElements.InfoLabel;
        MinimapCoords = PlayerUIElements.MinimapCoords;
        PlayerHealthForeground = PlayerUIElements.HealthForeground;
        PlayerHealthText = PlayerUIElements.HealthText;
        PlayerShieldForeground = PlayerUIElements.ShieldForeground;
        PlayerShieldText = PlayerUIElements.ShieldText;
        PlayerEnergyForeground = PlayerUIElements.EnergyForeground;
        PlayerEnergyText = PlayerUIElements.EnergyText;
        PlayerAbility1Background = PlayerUIElements.Ability1Background;
        PlayerAbility1Icon = PlayerUIElements.Ability1Icon;
        PlayerAbility1Cooldown = PlayerUIElements.Ability1CD;
        PlayerAbility1CDText = PlayerUIElements.Ability1CDText;
        PlayerAbility2Background = PlayerUIElements.Ability2Background;
        PlayerAbility2Icon = PlayerUIElements.Ability2Icon;
        PlayerAbility2Cooldown = PlayerUIElements.Ability2CD;
        PlayerAbility2CDText = PlayerUIElements.Ability2CDText;
        PlayerAbility3Background = PlayerUIElements.Ability3Background;
        PlayerAbility3Icon = PlayerUIElements.Ability3Icon;
        PlayerAbility3Cooldown = PlayerUIElements.Ability3CD;
        PlayerAbility3CDText = PlayerUIElements.Ability3CDText;
        ShieldDamageEffect = PlayerUIElements.ShieldDamageEffect;
        HealthDamageEffect = PlayerUIElements.HealthDamageEffect;
    }

    // Initialize screen state
    private static void InitializeScreenState()
    {
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
    }

    // Initialize popup state
    private static void InitializePopUpState()
    {
        // Loop through all popups
        foreach(KeyValuePair<string, UIPopUp> popUp in UIPopUps)
        {
            // If pop up is not marked default active
            if(popUp.Value.DefaultActive != true)
            {
                // Deactivate popup
                popUp.Value.gameObject.SetActive(false);
            }
        }
    }

    // Update rebind button text
    private static void UpdateRebindButtonText()
    {
        RebindElements.UpdateRebindButtonText();
    }

    // Initialize ability icons
    private static void InitializeAbilityIcons()
    {
        // If player ship is bomber
        if(GameController.PlayerShipType == PlayerShip.PlayerShipType.Bomber)
        {
            PlayerAbility1Icon.overrideSprite = PlayerAbilityIcons.BomberAbilityIcons[0];
            PlayerAbility2Icon.overrideSprite = PlayerAbilityIcons.BomberAbilityIcons[1];
            PlayerAbility3Icon.overrideSprite = PlayerAbilityIcons.BomberAbilityIcons[2];
        }
        // If player ship is engineer
        else if(GameController.PlayerShipType == PlayerShip.PlayerShipType.Engineer)
        {
            PlayerAbility1Icon.overrideSprite = PlayerAbilityIcons.EngineerAbilityIcons[0];
            PlayerAbility2Icon.overrideSprite = PlayerAbilityIcons.EngineerAbilityIcons[1];
            PlayerAbility3Icon.overrideSprite = PlayerAbilityIcons.EngineerAbilityIcons[2];
        }
        InitializeAbilityKeybindText();
    }

    // Initialize ability keybind text
    private static void InitializeAbilityKeybindText()
    {
        PlayerAbilityIcons.Ability1Keybind.text = PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.Ability1);
        PlayerAbilityIcons.Ability2Keybind.text = PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.Ability2);
        PlayerAbilityIcons.Ability3Keybind.text = PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.Ability3);
        PlayerAbilityIcons.MainGunKeybind.text = PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.MainGun);
        PlayerAbilityIcons.WarpKeybind.text = PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.Warp);
        PlayerAbilityIcons.MoveKeybind.text = (PlayerInput.InputMode == PlayerInput.InputModeEnum.Controller) ? $@"{PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.MoveXAxis)}" : 
            $@"{PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.MoveForward)}{Environment.NewLine}{PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.MoveLeft)}{PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.MoveBack)}{PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.MoveRight)}";
        PlayerAbilityIcons.AimKeybind.text = (PlayerInput.InputMode == PlayerInput.InputModeEnum.Controller) ? $@"{PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.AimXAxis)}" : "Mouse";
        PlayerAbilityIcons.PauseKeybind.text = PlayerInput.GetKeybindNameForInput(InputBinding.GameInputsEnum.Pause);
    }

    // Update error text
    private static void UpdateErrorText()
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

    // Update minimap coords
    private static void UpdateMinimapCoords()
    {
        // Get player position
        Vector3 PlayerPosition = GameController.Player.ShipObject.transform.position;
        // Update minimap coord text to display position
        MinimapCoords.text = $@"X: {Mathf.RoundToInt(PlayerPosition.x)}{Environment.NewLine}Z: {Mathf.RoundToInt(PlayerPosition.z)}";
    }

    // Update info label
    private static void UpdateInfoLabel()
    {
        // Get delta time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // Calculate FPS
        FPS = Mathf.FloorToInt(1.0f / deltaTime);
        // Get hours, minutes, and seconds from time
        Seconds = Mathf.FloorToInt((Time.time - GameController.TimeStarted) % 60);
        Minutes = Mathf.FloorToInt((Time.time - GameController.TimeStarted) / 60 % 60);
        Hours = Mathf.FloorToInt((Time.time - GameController.TimeStarted) / 3600);
        // Format time string
        TimeString = $@"{Hours}:{(Minutes < 10 ? "0" + Minutes.ToString() : Minutes.ToString())}:{(Seconds < 10 ? "0" + Seconds.ToString() : Seconds.ToString())}";
        // Update info label with FPS, Time, and Score
        InfoLabel.text = $@"FPS: {FPS}{Environment.NewLine}Timer: {TimeString}{Environment.NewLine}Score: {GameController.Score}";
    }

    // Update NPC healthbars
    private static void UpdateNPCHealthbars()
    {
        // Get UIOffset, subtract 60 relative pixels from y to have healthbar appear above ship
        UIOffset = new Vector2(PlayerUIRectTransform.sizeDelta.x / 2f, (PlayerUIRectTransform.sizeDelta.y / 2f) - 60);
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship doesn't currently have a healthbar and is alive
            if(Healthbars.ContainsKey(ship.Key) == false && ship.Value.Alive == true)
            {
                // If ship is NPC
                if(ship.Value.IsPlayer == false)
                {
                    // Add a new healthbar UI for npc ship
                    Healthbars.Add(ship.Key, GameObject.Instantiate(NPCUIPrefab));
                    // Set the parent and name of healthbar object
                    HealthbarUI = Healthbars[ship.Key];
                    HealthbarUI.transform.SetParent(PlayerUIScreen.transform, false);
                    HealthbarUI.name = $@"Healthbar: {ship.Key}";
                    // If ship is drone
                    if(ship.Value.AItype == Ship.AIType.Drone)
                    {
                        // Lower the scale of healthbar
                        HealthbarUI.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                }
            }
            // If ship has a healthbar and is alive
            if(Healthbars.ContainsKey(ship.Key) == true && ship.Value.Alive == true)
            {
                // Get current healthbar
                HealthbarUI = Healthbars[ship.Key];
                // If healthbar exists
                if(HealthbarUI != null)
                {
                    // Set the position of the healthbar UI
                    Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(ship.Value.ShipObject.transform.position);
                    Vector2 ProportionalPosition = new Vector2(ViewportPosition.x * PlayerUIRectTransform.sizeDelta.x, ViewportPosition.y * PlayerUIRectTransform.sizeDelta.y);
                    // Move healthbar to appear where ship appears on the screen
                    HealthbarUI.GetComponent<RectTransform>().localPosition = ProportionalPosition - UIOffset;
                    // Fill the healthbar relative to ship's health value
                    Image ShieldbarFillImageBackground = HealthbarUI.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                    Image ShieldbarFillImage = HealthbarUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
                    Image HealthbarFillImageBackground = HealthbarUI.transform.GetChild(0).GetChild(2).GetComponent<Image>();
                    Image HealthbarFillImage = HealthbarUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>();
                    ShieldbarFillImageBackground.fillAmount = 1 - (ship.Value.Stats.Shields / ship.Value.Stats.MaxShields);
                    ShieldbarFillImage.fillAmount = ship.Value.Stats.Shields / ship.Value.Stats.MaxShields;
                    HealthbarFillImageBackground.fillAmount = 1 - (ship.Value.Stats.Health / ship.Value.Stats.MaxHealth);
                    HealthbarFillImage.fillAmount = ship.Value.Stats.Health / ship.Value.Stats.MaxHealth;
                    // If health is below full
                    if(HealthbarFillImage.fillAmount < 1 || ShieldbarFillImage.fillAmount < 1)
                    {
                        // Show healthbar
                        HealthbarUI.SetActive(true);
                    }
                    // If health is full
                    else if(HealthbarFillImage.fillAmount == 1 && ShieldbarFillImage.fillAmount == 1)
                    {
                        // Hide healthbar
                        HealthbarUI.SetActive(false);
                    }
                }
            }
            // Remove healthbar if healthbar currently exists on a ship that is dead or has no GameObject
            else if(Healthbars.ContainsKey(ship.Key) == true && (ship.Value.Alive == false || ship.Value.ShipObject == null))
            {
                RemoveHealthbar(ship.Key);
            }
        }
    }

    // Update player UI
    private static void UpdatePlayerUI()
    {
        // Get reference to player
        Ship Player = GameController.Player;
        // Fill shield bar accordingly
        PlayerShieldForeground.fillAmount = Player.Stats.Shields / Player.Stats.MaxShields;
        PlayerShieldText.text = $@"{Player.Stats.Shields.ToString("0")} / {Player.Stats.MaxShields.ToString("0")}";
        // Fill health bar accordingly
        PlayerHealthForeground.fillAmount = Player.Stats.Health / Player.Stats.MaxHealth;
        PlayerHealthText.text = $@"{Player.Stats.Health.ToString("0")} / {Player.Stats.MaxHealth.ToString("0")}";
        // Fill energy bar accordingly
        PlayerEnergyForeground.fillAmount = Player.Stats.Energy / Player.Stats.MaxEnergy;
        PlayerEnergyText.text = $@"{Player.Stats.Energy.ToString("0")} / {Player.Stats.MaxEnergy.ToString("0")}";
        // If Ability 1 is on cooldown or currently active
        if(Player.AbilityOnCooldown[0] == true || Player.AbilityActive[0] == true)
        {
            // Fill Ablity 1 cooldown meter accordingly
            PlayerAbility1Background.fillAmount = 0;
            if(Player.AbilityActive[0] == true)
            {
                PlayerAbility1Cooldown.fillAmount = 1;
            }
            else
            {
                PlayerAbility1Cooldown.fillAmount = 1 - ((Time.time - Player.LastAbilityCooldownStartedTime[0]) / Player.Stats.AbilityCooldownTime[0]);
                float Ability1CooldownLeftTime = Player.Stats.AbilityCooldownTime[0] - (Time.time - Player.LastAbilityCooldownStartedTime[0]);
                PlayerAbility1CDText.text = Ability1CooldownLeftTime > 10f ? Ability1CooldownLeftTime.ToString("0") : Ability1CooldownLeftTime.ToString("0.0");
            }
        }
        // If Ability 1 is not on cooldown
        else
        {
            // Fill Ability 1 cooldown meter accordingly
            PlayerAbility1Background.fillAmount = 1;
            PlayerAbility1Cooldown.fillAmount = 0;
            PlayerAbility1CDText.text = "";
        }
        // If Ability 2 is on cooldown or currently active
        if(Player.AbilityOnCooldown[1] == true || Player.AbilityActive[1] == true)
        {
            // Fill Ability 2 cooldown meter accordingly
            PlayerAbility2Background.fillAmount = 0;
            if(Player.AbilityActive[1] == true)
            {
                PlayerAbility2Cooldown.fillAmount = 1;
            }
            else
            {
                PlayerAbility2Cooldown.fillAmount = 1 - ((Time.time - Player.LastAbilityCooldownStartedTime[1]) / Player.Stats.AbilityCooldownTime[1]);
                float Ability2CooldownLeftTime = Player.Stats.AbilityCooldownTime[1] - (Time.time - Player.LastAbilityCooldownStartedTime[1]);
                PlayerAbility2CDText.text = Ability2CooldownLeftTime > 10f ? Ability2CooldownLeftTime.ToString("0") : Ability2CooldownLeftTime.ToString("0.0");
            }
        }
        // If Ability 2 is not on cooldown
        else
        {
            // Fill Ability 2 cooldown meter accordingly
            PlayerAbility2Background.fillAmount = 1;
            PlayerAbility2Cooldown.fillAmount = 0;
            PlayerAbility2CDText.text = "";
        }
        // If Ability 3 is on cooldown or currently active
        if(Player.AbilityOnCooldown[2] == true || Player.AbilityActive[2] == true)
        {
            // Fill Ability 3 cooldown meter accordingly
            PlayerAbility3Background.fillAmount = 0;
            if(Player.AbilityActive[2] == true)
            {
                PlayerAbility3Cooldown.fillAmount = 1;
            }
            else
            {
                PlayerAbility3Cooldown.fillAmount = 1 - ((Time.time - Player.LastAbilityCooldownStartedTime[2]) / Player.Stats.AbilityCooldownTime[2]);
                float Ability3CooldownLeftTime = Player.Stats.AbilityCooldownTime[2] - (Time.time - Player.LastAbilityCooldownStartedTime[2]);
                PlayerAbility3CDText.text = Ability3CooldownLeftTime > 10f ? Ability3CooldownLeftTime.ToString("0") : Ability3CooldownLeftTime.ToString("0.0");
            }
        }
        // If Ability 3 is not on cooldown
        else
        {
            // Fill Ability 3 cooldown meter accordingly
            PlayerAbility3Background.fillAmount = 1;
            PlayerAbility3Cooldown.fillAmount = 0;
            PlayerAbility3CDText.text = "";
        }
        // If shield damage vignette effect is currently active and either it has been longer than the effect duration specified or the health damage vignette effect is on
        if(ShieldDamageEffect.activeSelf == true && (Time.time - ShieldDamageEffectStartTime > ShowDamageEffectDuration || HealthDamageEffect.activeSelf == true))
        {
            // Turn off shield damage vignette
            ShieldDamageEffect.SetActive(false);
        }
        // If health damage vignette effect is currently active and it has been longer than the effect duration specified
        if(HealthDamageEffect.activeSelf == true && Time.time - HealthDamageEffectStartTime > ShowDamageEffectDuration)
        {
            // Turn off health damage vignette
            HealthDamageEffect.SetActive(false);
        }
    }

    // Clear all healthbars
    private static void ClearHealthbars()
    {
        //Loop through healthbar uis
        foreach(KeyValuePair<uint, GameObject> healthbarui in Healthbars)
        {
            // Destroy game object for healthbar
            GameObject.Destroy(healthbarui.Value);
        }
        Healthbars.Clear();
    }

    // Update cursor state
    private static void UpdateCursorState(bool _forceCursorVisible)
    {
        // If input mode is controller
        if(PlayerInput.InputMode == PlayerInput.InputModeEnum.Controller)
        {
            // Use default cursor and set invisible
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            // Set invisible
            Cursor.visible = false;
        }
        // If input mode is KB&M
        else
        {
            // If game state is menus or paused
            if(GameController.CurrentGameState == GameController.GameState.Menus || GameController.CurrentGameState == GameController.GameState.Paused)
            {
                // Use default cursor and set visible
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                Cursor.visible = true;
            }
            // If game state is playing
            else
            {
                // If force cursor visible is true
                if(_forceCursorVisible == true)
                {
                    // Use default cursor and set visible
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    Cursor.visible = true;
                }
                // If force cursor visible is not true
                else
                {
                    // Use aim cursor and set visible
                    Cursor.SetCursor(AimCursorTexture, AimCursorHotspot, CursorMode.Auto);
                    Cursor.visible = true;
                }
            }
        }
    }

    // Check select default button
    private static void CheckSelectDefaultButton(GameObject _defaultButton)
    {
        // If input mode is controller
        if(PlayerInput.InputMode == PlayerInput.InputModeEnum.Controller)
        {
            // Select default button for screen
            EventSys.SetSelectedGameObject(_defaultButton);
        }
    }

    // Make unique changes depending on new screen entered
    private static void NewScreenUniqueChanges()
    {
        // If new screen is main menu
        if(CurrentScreen.gameObject == MainMenuScreen)
        {
            // Show menu background
            MenuBackground.SetActive(true);
        }
        // If new screen is ship select menu
        else if(CurrentScreen.gameObject == ShipSelectMenuScreen)
        {
            // Show menu background
            MenuBackground.SetActive(true);
            // Get current ship select toggle
            GetShipSelectToggle();
        }
        // If new screen is settings menu
        else if(CurrentScreen.gameObject == SettingsMenuScreen)
        {
            // Show menu background
            MenuBackground.SetActive(true);
        }
        // If new screen is player UI
        else if(CurrentScreen.gameObject == PlayerUIScreen)
        {
            // Hide menu background
            MenuBackground.SetActive(false);
            // Initialize ability icons
            InitializeAbilityIcons();
        }
    }

    // Clear all
    public static void ClearAll()
    {
        // Clear all healthbars
        ClearHealthbars();
        // Close all popups
        CloseAllPopUps();
        // Hide all ship models
        HideAllShipModels();
    }

    // Remove Healthbar
    public static void RemoveHealthbar(uint _id)
    {
        GameObject.Destroy(Healthbars[_id]);
        Healthbars.Remove(_id);
    }

    // Show shield damage effect vignette
    public static void ShowShieldDamageEffect()
    {
        ShieldDamageEffect.SetActive(true);
        ShieldDamageEffectStartTime = Time.time;
    }

    // Show health damage effect vignette
    public static void ShowHealthDamageEffect()
    {
        HealthDamageEffect.SetActive(true);
        HealthDamageEffectStartTime = Time.time;
    }

    // Change game state
    public static void ChangeGameState(GameController.GameState _newGameState)
    {
        // Update cursor
        UpdateCursorState(false);
    }

    // Change screen
    public static void ChangeScreen(UIScreen _newScreen)
    {
        // Slide out current screen
        CurrentScreen.SetupSlideScreen(UIAnimations.InOutEnum.Out);
        CurrentScreen.Active = false;
        // Set current screen to new screen
        CurrentScreen = _newScreen;
        // Slide in new screen
        CurrentScreen.SetupSlideScreen(UIAnimations.InOutEnum.In);
        CurrentScreen.Active = true;
        // Check select default button
        CheckSelectDefaultButton(CurrentScreen.DefaultButton);
        // Make unique changes depending on new screen
        NewScreenUniqueChanges();
    }

    // Back
    public static void Back()
    {
        // Close all popups
        CloseAllPopUps();
        // Hide all ship models
        HideAllShipModels();
        // Change screen to the back screen of the current screen
        ChangeScreen(CurrentScreen.BackScreen);
    }

    // Open PopUp
    public static void OpenPopUp(UIPopUp _newPopUp)
    {
        // Update cursor
        UpdateCursorState(true);
        // Slide in popup
        _newPopUp.SetupSlideScreen(UIAnimations.InOutEnum.In);
        _newPopUp.Active = true;
        ActivePopUps.Add(_newPopUp);
        // If popup is supposed to be only popup on screen
        if(_newPopUp.OnlyPopUp == true)
        {
            // Loop through all pop ups
            foreach(KeyValuePair<string, UIPopUp> popUp in UIPopUps)
            {
                // If popup is not the new popup
                if(popUp.Value != _newPopUp)
                {
                    // Deactivate popup
                    ClosePopUp(popUp.Value);
                }
            }
        }
        // If select button on open is enabled for this popup
        if(_newPopUp.SelectButtonOnOpen == true)
        {
            // Check if default button should be selected and select if so
            CheckSelectDefaultButton(_newPopUp.DefaultButton);
        }
    }

    // Close PopUp
    public static void ClosePopUp(UIPopUp _popUp)
    {
        // Update cursor
        UpdateCursorState(false);
        // Slide out popup
        _popUp.SetupSlideScreen(UIAnimations.InOutEnum.Out);
        _popUp.Active = false;
        ActivePopUps.Remove(_popUp);
        // If select button on open is enabled for this popup
        if(_popUp.SelectButtonOnOpen == true)
        {
            // If this popup was the only popup active
            if(ActivePopUps.Count == 0)
            {
                // Check if default button should be selected for screen and select if so
                CheckSelectDefaultButton(CurrentScreen.DefaultButton);
            }
            // If there are other active popups
            else
            {
                // Check if default button should be selected for last popup and select if so
                CheckSelectDefaultButton(ActivePopUps.Last().DefaultButton);
            }
        }
    }

    // Close all popups
    public static void CloseAllPopUps()
    {
        // Loop through all popups
        foreach(KeyValuePair<string, UIPopUp> popUp in UIPopUps)
        {
            // Deactivate popup
            ClosePopUp(popUp.Value);
        }
        // Check select default button
        CheckSelectDefaultButton(CurrentScreen.DefaultButton);
    }

    // Show ship model
    public static void ShowShipModel(GameObject _model)
    {
        // Set model game object active
        _model.SetActive(true);
        // Loop through all models
        foreach(KeyValuePair<string, GameObject> model in ShipModels)
        {
            // If current model is not model to be shown
            if(model.Value != _model)
            {
                // Deactivate model
                model.Value.SetActive(false);
            }
        }
    }

    // Hide all ship models
    public static void HideAllShipModels()
    {
        // Loop through all models
        foreach(KeyValuePair<string, GameObject> model in ShipModels)
        {
            // Deactivate model
            model.Value.SetActive(false);
        }
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

    // Get or set selected button
    public static void GetOrSetSelectedButton(bool _get)
    {
        // If get is true
        if(_get == true)
        {
            // Grab currently selected button
            LastSelectedButton = EventSys.currentSelectedGameObject;
            // Select nothing
            EventSys.SetSelectedGameObject(null);
        }
        // If get is false, set
        else
        {
            // Select last selected button
            EventSys.SetSelectedGameObject(LastSelectedButton);
        }
    }

    // Get ship select toggle
    public static void GetShipSelectToggle()
    {
        // Get ship select toggle
        ShipSelectElements.GetShipSelectToggle();
        // If toggle is bomber
        if(ShipSelectElements.CurrentSelection.gameObject.name == UIElementsShipSelect.BomberToggleName)
        {
            // Update player ship type
            GameController.ChangePlayerShipType(PlayerShip.PlayerShipType.Bomber);
            // Show bomber model
            ShowShipModel(BomberModel);
            // If bomber popup is not open
            if(BomberInfoPopUp.Active == false)
            {
                // Open bomber info panel
                OpenPopUp(BomberInfoPopUp);
            }
        }
        // If toggle is engineer
        else if(ShipSelectElements.CurrentSelection.gameObject.name == UIElementsShipSelect.EngineerToggleName)
        {
            // Update player ship type
            GameController.ChangePlayerShipType(PlayerShip.PlayerShipType.Engineer);
            // Show engineer model
            ShowShipModel(EngineerModel);
            // If engineer popup is not open
            if(EngineerInfoPopUp.Active == false)
            {
                // Open engineer info panel
                OpenPopUp(EngineerInfoPopUp);
            }
        }
        // If input is controller
        if(PlayerInput.InputMode == PlayerInput.InputModeEnum.Controller)
        {
            // Select currently selected toggle
            EventSys.SetSelectedGameObject(ShipSelectElements.CurrentSelection.gameObject);
        }
    }

    // Get movement style toggle
    public static void GetMovementStyleToggle()
    {
        // Get movement style
        SettingsElements.GetMovementStyleToggle();
        // If movement style is 0
        if(SettingsElements.MovementStyle == 0)
        {
            // Change movement style to screenspace
            PlayerInput.ChangeMovementStyle(PlayerInput.MovementStyleEnum.ScreenSpace);
        }
        // If movement style is 1
        else if(SettingsElements.MovementStyle == 1)
        {
            // Change movement style to tank
            PlayerInput.ChangeMovementStyle(PlayerInput.MovementStyleEnum.Tank);
        }
    }

    // Get input type toggle
    public static void GetInputTypeToggle()
    {
        // Get input type
        SettingsElements.GetInputTypeToggle();
        // If input type is 0
        if(SettingsElements.InputType == 0)
        {
            // Change input type to KB&M
            PlayerInput.ChangeInputType(PlayerInput.InputModeEnum.KeyboardAndMouse);
            // Select default button as nothing
            CheckSelectDefaultButton(null);
            // Update cursor
            UpdateCursorState(false);
        }
        // If input type is 1
        else if(SettingsElements.InputType == 1)
        {
            // Change input type to controller
            PlayerInput.ChangeInputType(PlayerInput.InputModeEnum.Controller);
            // Select default button for controller
            CheckSelectDefaultButton(CurrentScreen.DefaultButton);
            // Update cursor
            UpdateCursorState(false);
        }
    }

    // Start new game
    public static void StartNewGame()
    {
        // Change screen to player UI
        ChangeScreen(UIScreens[PlayerUIName]);
    }

    // Open pause popup
    public static void OpenPausePopUp(bool _open)
    {
        // If open is true
        if(_open == true)
        {
            // Open pause popup
            OpenPopUp(UIPopUps[PausePopUpName]);
        }
        // If open is false
        else
        {
            // Close pause popup
            ClosePopUp(UIPopUps[PausePopUpName]);
        }
    }

    // Open game over popup
    public static void OpenGameOverPopUp(bool _open)
    {
        // If open is true
        if(_open == true)
        {
            // Open game over popup
            OpenPopUp(UIPopUps[GameOverPopUpName]);
        }
        // If open is false
        else
        {
            // Close game over popup
            ClosePopUp(UIPopUps[GameOverPopUpName]);
        }
    }
}
