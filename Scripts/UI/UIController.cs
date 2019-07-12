using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Controls the game UI
public static class UIController
{
    // GameObjects
    // Event system and buttons
    public static EventSystem CurrentEventSystem { get; private set; }
    private static bool EnteredNewState = false;
    private static GameObject MainMenuButtonDefault;
    private static GameObject NewGameMenuButtonDefault;
    private static GameObject SettingsMenuButtonDefault;
    private static GameObject PauseMenuButtonDefault;
    private static GameObject GameOverMenuButtonDefault;
    // Main menu
    private static GameObject MainMenu;
    private static GameObject MainMenuContainer;
    private static TextMeshProUGUI VersionText;
    // New game menu
    private static GameObject NewGameContainer;
    // Settings menu
    private static GameObject SettingsMenuContainer;
    private static TextMeshProUGUI SettingsErrorText;
    private static TextMeshProUGUI MainGunCurrentInputText;
    private static TextMeshProUGUI Ability1CurrentInputText;
    private static TextMeshProUGUI Ability2CurrentInputText;
    private static TextMeshProUGUI Ability3CurrentInputText;
    // Pause menu
    private static GameObject PauseMenuScreen;
    // Game over menu
    private static GameObject GameOverMenuScreen;
    private static GameObject GameOverText;
    // UI
    private static GameObject UI;
    private static GameObject UICanvas;
    private static RectTransform rectTransform;
    // Player UI
    private static GameObject PlayerUI;
    private static Image PlayerHealthForeground;
    private static TextMeshProUGUI PlayerHealthText;
    private static Image PlayerShieldForeground;
    private static TextMeshProUGUI PlayerShieldText;
    private static Image PlayerEnergyForeground;
    private static TextMeshProUGUI PlayerEnergyText;
    private static Image PlayerAbility1Background;
    private static Image PlayerAbility1Cooldown;
    private static TextMeshProUGUI PlayerAbility1CDText;
    private static Image PlayerAbility2Background;
    private static Image PlayerAbility2Cooldown;
    private static TextMeshProUGUI PlayerAbility2CDText;
    private static Image PlayerAbility3Background;
    private static Image PlayerAbility3Cooldown;
    private static TextMeshProUGUI PlayerAbility3CDText;
    // NPC UI
    private static GameObject NPCUIPrefab;
    private static GameObject HealthbarUI;
    // Minimap
    private static GameObject MinimapCoords;
    // Info label
    private static TextMeshProUGUI InfoLabel;
    // FX
    private static GameObject ShieldDamageEffect;
    private static GameObject HealthDamageEffect;

    // Dictionary of Healthbar UIs
    private static Dictionary<uint, GameObject> HealthbarUIs = new Dictionary<uint, GameObject>();

    // Fields
    private static Vector2 UIOffset;
    private static float deltaTime = 0.0f;
    private static int FPS;
    private static float ShieldDamageEffectStartTime = 0f;
    private static float HealthDamageEffectStartTime = 0f;
    private const float ShowDamageEffectDuration = 0.25f;
    private static int Seconds = 0;
    private static int Minutes = 0;
    private static int Hours = 0;
    private static string TimeString = "";


    // Initialize
    public static void Initialize()
    {
        CurrentEventSystem = EventSystem.current;
        MainMenuButtonDefault = GameObject.Find(GameController.MainMenuButtonDefaultName);
        NewGameMenuButtonDefault = GameObject.Find(GameController.NewGameMenuButtonDefaultName);
        SettingsMenuButtonDefault = GameObject.Find(GameController.SettingsMenuButtonDefaultName);
        PauseMenuButtonDefault = GameObject.Find(GameController.PauseMenuButtonDefaultName);
        GameOverMenuButtonDefault = GameObject.Find(GameController.GameOverMenuButtonDefaultName);
        HealthbarUIs.Clear();
        MainMenu = GameObject.Find(GameController.MainMenuName);
        MainMenuContainer = GameObject.Find(GameController.MainMenuContainerName);
        VersionText = GameObject.Find(GameController.VersionTextName).GetComponent<TextMeshProUGUI>();
        VersionText.text = GameController.Version;
        MainMenuContainer.SetActive(true);
        NewGameContainer = GameObject.Find(GameController.NewGameContainerName);
        NewGameContainer.SetActive(false);
        SettingsMenuContainer = GameObject.Find(GameController.SettingsMenuContainerName);
        SettingsErrorText = GameObject.Find(GameController.SettingsErrorTextName).GetComponent<TextMeshProUGUI>();
        MainGunCurrentInputText = GameObject.Find(GameController.MainGunCurrentInputTextName).GetComponent<TextMeshProUGUI>();
        Ability1CurrentInputText = GameObject.Find(GameController.Ability1CurrentInputTextName).GetComponent<TextMeshProUGUI>();
        Ability2CurrentInputText = GameObject.Find(GameController.Ability2CurrentInputTextName).GetComponent<TextMeshProUGUI>();
        Ability3CurrentInputText = GameObject.Find(GameController.Ability3CurrentInputTextName).GetComponent<TextMeshProUGUI>();
        SettingsMenuContainer.SetActive(false);
        UI = GameObject.Find(GameController.UIName);
        UICanvas = GameObject.Find(GameController.UICanvasName);
        rectTransform = UICanvas.GetComponent<RectTransform>();
        HealthDamageEffect = GameObject.Find(GameController.HealthDamageEffectName);
        HealthDamageEffect.SetActive(false);
        ShieldDamageEffect = GameObject.Find(GameController.ShieldDamageEffectName);
        ShieldDamageEffect.SetActive(false);
        MinimapCoords = GameObject.Find(GameController.MinimapCoordsName);
        InfoLabel = GameObject.Find(GameController.InfoLabelName).GetComponent<TextMeshProUGUI>();
        PlayerUI = GameObject.Find(GameController.PlayerUIName);
        PlayerHealthForeground = GameObject.Find(GameController.PlayerHealthForegroundName).GetComponent<Image>();
        PlayerHealthText = GameObject.Find(GameController.PlayerHealthTextName).GetComponent<TextMeshProUGUI>();
        PlayerShieldForeground = GameObject.Find(GameController.PlayerShieldForegroundName).GetComponent<Image>();
        PlayerShieldText = GameObject.Find(GameController.PlayerShieldTextName).GetComponent<TextMeshProUGUI>();
        PlayerEnergyForeground = GameObject.Find(GameController.PlayerEnergyForegroundName).GetComponent<Image>();
        PlayerEnergyText = GameObject.Find(GameController.PlayerEnergyTextName).GetComponent<TextMeshProUGUI>();
        PlayerAbility1Background = GameObject.Find(GameController.PlayerAbility1BackgroundName).GetComponent<Image>();
        PlayerAbility1Cooldown = GameObject.Find(GameController.PlayerAbility1CooldownName).GetComponent<Image>();
        PlayerAbility1CDText = GameObject.Find(GameController.PlayerAbility1CDTextName).GetComponent<TextMeshProUGUI>();
        PlayerAbility2Background = GameObject.Find(GameController.PlayerAbility2BackgroundName).GetComponent<Image>();
        PlayerAbility2Cooldown = GameObject.Find(GameController.PlayerAbility2CooldownName).GetComponent<Image>();
        PlayerAbility2CDText = GameObject.Find(GameController.PlayerAbility2CDTextName).GetComponent<TextMeshProUGUI>();
        PlayerAbility3Background = GameObject.Find(GameController.PlayerAbility3BackgroundName).GetComponent<Image>();
        PlayerAbility3Cooldown = GameObject.Find(GameController.PlayerAbility3CooldownName).GetComponent<Image>();
        PlayerAbility3CDText = GameObject.Find(GameController.PlayerAbility3CDTextName).GetComponent<TextMeshProUGUI>();
        UI.SetActive(false);
        GameOverMenuScreen = GameObject.Find(GameController.GameOverMenuName);
        GameOverText = GameObject.Find(GameController.GameOverTextName);
        GameOverMenuScreen.SetActive(false);
        PauseMenuScreen = GameObject.Find(GameController.PauseMenuName);
        PauseMenuScreen.SetActive(false);
        NPCUIPrefab = Resources.Load<GameObject>(GameController.NPCUIPrefabName);
    }

    // Update is called once per frame
    public static void Update()
    {
        // If game state is main menu
        if(GameController.CurrentGameState == GameController.GameState.MainMenu)
        {
            MainMenuUIUpdate();
        }
        // If game state is new game menu
        else if(GameController.CurrentGameState == GameController.GameState.NewGameMenu)
        {
            NewGameMenuUpdate();
        }
        // If game state is settings menu
        else if(GameController.CurrentGameState == GameController.GameState.SettingsMenu)
        {
            SettingsMenuUpdate();
        }
        // If game state is playing
        else if(GameController.CurrentGameState == GameController.GameState.Playing)
        {
            PlayingUIUpdate();
        }
        // If game state is paused
        else if(GameController.CurrentGameState == GameController.GameState.Paused)
        {
            PauseMenuUIUpdate();
        }
        // If game state is game over
        else if(GameController.CurrentGameState == GameController.GameState.GameOver)
        {
            GameOverUIUpdate();
        }
    }

    // Restart
    public static void Restart()
    {
        // Loop through healthbar uis
        foreach(KeyValuePair<uint, GameObject> healthbarui in HealthbarUIs)
        {
            // Destroy game object for healthbar
            GameObject.Destroy(healthbarui.Value);
        }
        HealthbarUIs.Clear();
    }

    // Called during update when game state is main menu
    private static void MainMenuUIUpdate()
    {
        SetupUIType(GameController.GameState.MainMenu);
    }

    // Called during update when game state is new game menu
    private static void NewGameMenuUpdate()
    {
        SetupUIType(GameController.GameState.NewGameMenu);
    }

    // Called during update when game state is settings menu
    private static void SettingsMenuUpdate()
    {
        SetupUIType(GameController.GameState.SettingsMenu);
        UpdateRebindInputsText();
    }

    // Called during update if game state is playing
    private static void PlayingUIUpdate()
    {
        SetupUIType(GameController.GameState.Playing);
        UpdateMinimapCoords();
        UpdateInfoLabel();
        UpdateNPCHealthbars();
        UpdatePlayerUI();
    }

    // Called during update if game state is paused
    private static void PauseMenuUIUpdate()
    {
        SetupUIType(GameController.GameState.Paused);
    }

    // Called during update if game state is gameover
    private static void GameOverUIUpdate()
    {
        SetupUIType(GameController.GameState.GameOver);
        UpdateMinimapCoords();
        UpdateInfoLabel();
        UpdateNPCHealthbars();
        UpdatePlayerUI();
    }

    // Set entered new state
    public static void SetEnteredNewState(bool _enteredNewState)
    {
        EnteredNewState = _enteredNewState;
    }

    // Setup UI to type
    private static void SetupUIType(GameController.GameState _uiType)
    {
        switch(_uiType)
        {
            case GameController.GameState.MainMenu:
            {
                // Show main menu if not currently shown
                if(MainMenu.activeSelf == false)
                {
                    ShowMainMenu();
                }
                // Default selected button
                if(EnteredNewState == false)
                {
                    EnteredNewState = true;
                    GameManager.Instance.StartSelectDefaultButtonCoroutine(MainMenuButtonDefault);
                }
                // Hide UI if not currently hidden
                if(UI.activeSelf == true)
                {
                    HideUI();
                }
                // Hide pause menu if not currently hidden
                if(PauseMenuScreen.activeSelf == true)
                {
                    HidePauseMenu();
                }
                // Hide pause menu if not currently hidden
                if(GameOverMenuScreen.activeSelf == true)
                {
                    HideGameOver();
                }
                // Show cursor if not currently shown
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                break;
            }
            case GameController.GameState.NewGameMenu:
            {
                // Show cursor if not currently shown
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                // Show new game menu if not currently shown
                if(NewGameContainer.activeSelf == false)
                {
                    ShowNewGameMenu();
                }
                // Default selected button
                if(EnteredNewState == false)
                {
                    EnteredNewState = true;
                    GameManager.Instance.StartSelectDefaultButtonCoroutine(NewGameMenuButtonDefault);
                }
                // Hide main menu container if not currently hidden
                if(MainMenuContainer.activeSelf == true)
                {
                    HideMainMenuContainer();
                }
                break;
            }
            case GameController.GameState.SettingsMenu:
            {
                // Show cursor if not currently shown
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                if(SettingsMenuContainer.activeSelf == false)
                {
                    ShowSettingsMenu();
                }
                // Default selected button
                if(EnteredNewState == false)
                {
                    EnteredNewState = true;
                    GameManager.Instance.StartSelectDefaultButtonCoroutine(SettingsMenuButtonDefault);
                }
                // Hide main menu container if not currently hidden
                if(MainMenuContainer.activeSelf == true)
                {
                    HideMainMenuContainer();
                }
                break;
            }
            case GameController.GameState.Playing:
            {
                // Default selected button
                if(EnteredNewState == false)
                {
                    EnteredNewState = true;
                    CurrentEventSystem.SetSelectedGameObject(null);
                }
                // Hide main menu if not currently hidden
                if(MainMenu.activeSelf == true)
                {
                    HideMainMenu();
                }
                // Show UI if not currently shown
                if(UI.activeSelf == false)
                {
                    ShowUI();
                }
                // Hide pause menu if not currently hidden
                if(PauseMenuScreen.activeSelf == true)
                {
                    HidePauseMenu();
                }
                // Hide pause menu if not currently hidden
                if(GameOverMenuScreen.activeSelf == true)
                {
                    HideGameOver();
                }
                // Hide cursor if not currently hidden
                if(Cursor.visible == true)
                {
                    Cursor.visible = false;
                }
                break;
            }
            case GameController.GameState.Paused:
            {
                // Show pause menu
                if(PauseMenuScreen.activeSelf == false)
                {
                    ShowPauseMenu();
                    // Momentarily disable pause button input so pause menu doesn't disappear immediately after showing
                    PlayerInput.ZeroInputs();
                }
                // Default selected button
                if(EnteredNewState == false)
                {
                    EnteredNewState = true;
                    GameManager.Instance.StartSelectDefaultButtonCoroutine(PauseMenuButtonDefault);
                }
                // Hide pause menu if not currently hidden
                if(GameOverMenuScreen.activeSelf == true)
                {
                    HideGameOver();
                }
                // If pause is pressed
                if(PlayerInput.PauseButtonInput == true)
                {
                    // Get out of pause menu and back into playing state
                    GameController.ChangeGameState(GameController.GameState.Playing);
                }
                // Show cursor
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                break;
            }
            case GameController.GameState.GameOver:
            {
                // Show game over menu if it is currently hidden
                if(GameOverMenuScreen.activeSelf == false)
                {
                    ShowGameOver();
                }
                // Show cursor
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                // Default selected button
                if(EnteredNewState == false)
                {
                    EnteredNewState = true;
                    GameManager.Instance.StartSelectDefaultButtonCoroutine(GameOverMenuButtonDefault);
                }
                break;
            }
        }
    }

    // Update settings screen error text
    public static void UpdateSettingsErrorText(string _text)
    {
        SettingsErrorText.text = _text;
    }

    // Update rebind inputs text
    private static void UpdateRebindInputsText()
    {
        MainGunCurrentInputText.text = PlayerInput.InputBindings["Main Gun Input"].InputButton;
        Ability1CurrentInputText.text = PlayerInput.InputBindings["Ability 1 Input"].InputButton;
        Ability2CurrentInputText.text = PlayerInput.InputBindings["Ability 2 Input"].InputButton;
        Ability3CurrentInputText.text = PlayerInput.InputBindings["Ability 3 Input"].InputButton;
    }

    // Update minimap coords
    private static void UpdateMinimapCoords()
    {
        Vector3 PlayerPosition = GameController.Player.ShipObject.transform.position;
        MinimapCoords.GetComponent<TextMeshProUGUI>().text = $@"X: {Mathf.RoundToInt(PlayerPosition.x)}{Environment.NewLine}Z: {Mathf.RoundToInt(PlayerPosition.z)}";
    }

    // Update info label
    private static void UpdateInfoLabel()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        FPS = Mathf.FloorToInt(1.0f / deltaTime);
        Seconds = Mathf.FloorToInt((Time.time - GameController.TimeStarted) % 60);
        Minutes = Mathf.FloorToInt((Time.time - GameController.TimeStarted) / 60 % 60);
        Hours = Mathf.FloorToInt((Time.time - GameController.TimeStarted) / 3600);
        TimeString = $@"{Hours}:{(Minutes < 10 ? "0" + Minutes.ToString() : Minutes.ToString())}:{(Seconds < 10 ? "0" + Seconds.ToString() : Seconds.ToString())}";
        InfoLabel.text = $@"FPS: {FPS}{Environment.NewLine}Timer: {TimeString}{Environment.NewLine}Score: {GameController.Score}";
    }

    // Update NPC healthbars
    private static void UpdateNPCHealthbars()
    {
        // Get UIOffset, subtract 60 relative pixels from y to have healthbar appear above ship
        UIOffset = new Vector2(rectTransform.sizeDelta.x / 2f, (rectTransform.sizeDelta.y / 2f) - 60);
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship doesn't currently have a healthbar and is alive
            if(HealthbarUIs.ContainsKey(ship.Key) == false && ship.Value.Alive == true)
            {
                // If ship is NPC
                if(ship.Value.IsPlayer == false)
                {
                    // Add a new healthbar UI for npc ship
                    HealthbarUIs.Add(ship.Key, GameObject.Instantiate(NPCUIPrefab));
                    // Set the parent and name of healthbar object
                    HealthbarUI = HealthbarUIs[ship.Key];
                    HealthbarUI.transform.SetParent(UICanvas.transform, false);
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
            if(HealthbarUIs.ContainsKey(ship.Key) == true && ship.Value.Alive == true)
            {
                HealthbarUI = HealthbarUIs[ship.Key];
                if(HealthbarUI != null)
                {
                    // Set the position of the healthbar UI
                    Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(ship.Value.ShipObject.transform.position);
                    Vector2 ProportionalPosition = new Vector2(ViewportPosition.x * rectTransform.sizeDelta.x, ViewportPosition.y * rectTransform.sizeDelta.y);
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
                    else if(HealthbarFillImage.fillAmount == 1 && ShieldbarFillImage.fillAmount == 1)
                    {
                        // Hide healthbar
                        HealthbarUI.SetActive(false);
                    }
                }
            }
            // Remove healthbar if healthbar currently exists on a ship that is dead or has no GameObject
            else if(HealthbarUIs.ContainsKey(ship.Key) == true && (ship.Value.Alive == false || ship.Value.ShipObject == null))
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
        // Fill health and shield bars
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
            // Fill bomb cooldown meter accordingly
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

    // Remove Healthbar
    public static void RemoveHealthbar(uint _id)
    {
        GameObject.Destroy(HealthbarUIs[_id]);
        HealthbarUIs.Remove(_id);
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

    // Game over screen
    private static void ShowGameOver()
    {
        GameOverMenuScreen.SetActive(true);
        GameOverText = GameObject.Find(GameController.GameOverTextName);
        GameOverText.GetComponent<TextMeshProUGUI>().text = $@"GAME OVER{Environment.NewLine}{Environment.NewLine}TIME: {TimeString}{Environment.NewLine}SCORE: {GameController.Score}";
    }

    // Hide game over screen
    private static void HideGameOver()
    {
        GameOverMenuScreen.SetActive(false);
    }

    // Show main menu screen
    private static void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        MainMenuContainer.SetActive(true);
        NewGameContainer.SetActive(false);
    }

    // Hide main menu screen
    private static void HideMainMenu()
    {
        MainMenu.SetActive(false);
    }

    // Hide main menu container
    private static void HideMainMenuContainer()
    {
        MainMenuContainer.SetActive(false);
    }

    // Show new game menu screen
    private static void ShowNewGameMenu()
    {
        NewGameContainer.SetActive(true);
    }

    // Exit new game menu screen
    public static void ExitNewGameMenu()
    {
        GameController.ChangeGameState(GameController.GameState.MainMenu);
        MainMenuContainer.SetActive(true);
        NewGameContainer.SetActive(false);
    }

    // Show settings menu
    private static void ShowSettingsMenu()
    {
        SettingsErrorText.text = "";
        SettingsMenuContainer.SetActive(true);
    }

    // Exit settings menu screen
    public static void ExitSettingsMenu()
    {
        GameController.ChangeGameState(GameController.GameState.MainMenu);
        MainMenuContainer.SetActive(true);
        SettingsMenuContainer.SetActive(false);
    }

    // Show UI
    private static void ShowUI()
    {
        UI.SetActive(true);
    }

    // Hide UI
    private static void HideUI()
    {
        UI.SetActive(false);
    }

    // Show pause menu
    private static void ShowPauseMenu()
    {
        PauseMenuScreen.SetActive(true);
        // Pause physics simulation time
        Time.timeScale = 0;
    }

    // Hide pause menu
    private static void HidePauseMenu()
    {
        PauseMenuScreen.SetActive(false);
        // Set physics simulation time to default
        Time.timeScale = 1;
    }
}
