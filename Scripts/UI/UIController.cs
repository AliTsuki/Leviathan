using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using UnityEngine.UI;

// Controls the game UI
public static class UIController
{
    // GameObjects
    private static GameObject MainMenu;
    private static GameObject MainMenuContainer;
    private static GameObject NewGameContainer;
    private static GameObject UI;
    private static GameObject UICanvas;
    private static RectTransform rectTransform;
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
    private static GameObject NPCUIPrefab;
    private static GameObject HealthbarUI;
    private static GameObject MinimapCoords;
    private static TextMeshProUGUI InfoLabel;
    private static GameObject ShieldDamageEffect;
    private static GameObject HealthDamageEffect;
    private static GameObject PauseMenuScreen;
    private static GameObject GameOverMenuScreen;
    private static GameObject GameOverText;

    // Dictionary of Healthbar UIs
    private static Dictionary<uint, GameObject> HealthbarUIs = new Dictionary<uint, GameObject>();

    // Fields
    private static Vector2 UIOffset;
    private static float deltaTime = 0.0f;
    private static int FPS;
    private static float ShieldDamageEffectStartTime = 0f;
    private static float HealthDamageEffectStartTime = 0f;
    private static float ShowDamageEffectDuration = 0.25f;
    private static int Seconds = 0;
    private static int Minutes = 0;
    private static int Hours = 0;
    private static string TimeString = "";

    // UI type
    public enum UITypeEnum
    {
        MainMenu,
        NewGameMenu,
        Playing,
        Paused,
        GameOver
    }
    public static UITypeEnum UIType;


    // Initialize
    public static void Initialize()
    {
        HealthbarUIs.Clear();
        MainMenu = GameObject.Find(GameController.MainMenuName);
        MainMenuContainer = GameObject.Find(GameController.MainMenuContainerName);
        MainMenuContainer.SetActive(true);
        NewGameContainer = GameObject.Find(GameController.NewGameContainerName);
        NewGameContainer.SetActive(false);
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
        if(GameController.CurrentGameState == GameController.GameState.MainMenu)
        {
            MainMenuUIUpdate();
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
    }

    // Called during update when gamestate is main menu
    private static void MainMenuUIUpdate()
    {
        SetupUIType(UITypeEnum.MainMenu);
    }

    // Called during update if gamestate is playing
    private static void PlayingUIUpdate()
    {
        SetupUIType(UITypeEnum.Playing);
        UpdateMinimapCoords();
        UpdateInfoLabel();
        UpdateNPCHealthbars();
        UpdatePlayerUI();
    }

    // Called during update if gamestate is paused
    private static void PauseMenuUIUpdate()
    {
        SetupUIType(UITypeEnum.Paused);
    }

    // Setup UI to type
    public static void SetupUIType(UITypeEnum _uiType)
    {
        switch(_uiType)
        {
            case UITypeEnum.MainMenu:
            {
                UIType = UITypeEnum.MainMenu;
                // Show main menu if not currently shown
                if(MainMenu.activeSelf == false)
                {
                    ShowMainMenu();
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
                // Show cursor if not currently shown
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                break;
            }
            case UITypeEnum.NewGameMenu:
            {
                UIType = UITypeEnum.NewGameMenu;
                break;
            }
            case UITypeEnum.Playing:
            {
                UIType = UITypeEnum.Playing;
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
                // Hide cursor if not currently hidden
                if(Cursor.visible == true)
                {
                    Cursor.visible = false;
                }
                break;
            }
            case UITypeEnum.Paused:
            {
                UIType = UITypeEnum.Paused;
                // Show cursor
                if(Cursor.visible == false)
                {
                    Cursor.visible = true;
                }
                // Show pause menu
                if(PauseMenuScreen.activeSelf == false)
                {
                    ShowPauseMenu();
                    // Momentarily disable pause button input so pause menu doesn't disappear immediately after showing
                    PlayerInput.PauseButtonInput = false;
                }
                // If pause is pressed
                if(PlayerInput.PauseButtonInput == true)
                {
                    // Get out of pause menu and back into playing state
                    GameController.CurrentGameState = GameController.GameState.Playing;
                }
                break;
            }
            case UITypeEnum.GameOver:
            {
                UIType = UITypeEnum.GameOver;
                if(GameOverMenuScreen.activeSelf == false)
                {
                    GameOver();
                }
                break;
            }
        }
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
                    ShieldbarFillImageBackground.fillAmount = 1 - (ship.Value.Shields / ship.Value.MaxShields);
                    ShieldbarFillImage.fillAmount = ship.Value.Shields / ship.Value.MaxShields;
                    HealthbarFillImageBackground.fillAmount = 1 - (ship.Value.Health / ship.Value.MaxHealth);
                    HealthbarFillImage.fillAmount = ship.Value.Health / ship.Value.MaxHealth;
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
        PlayerShieldForeground.fillAmount = Player.Shields / Player.MaxShields;
        PlayerShieldText.text = $@"{Player.Shields.ToString("0")} / {Player.MaxShields.ToString("0")}";
        // Fill health and shield bars
        PlayerHealthForeground.fillAmount = Player.Health / Player.MaxHealth;
        PlayerHealthText.text = $@"{Player.Health.ToString("0")} / {Player.MaxHealth.ToString("0")}";
        // Fill energy bar accordingly
        PlayerEnergyForeground.fillAmount = Player.Energy / Player.MaxEnergy;
        PlayerEnergyText.text = $@"{Player.Energy.ToString("0")} / {Player.MaxEnergy.ToString("0")}";
        // If Ability 1 is on cooldown or currently active
        if(Player.Ability1OnCooldown == true || Player.Ability1Active == true)
        {
            // Fill Ablity 1 cooldown meter accordingly
            PlayerAbility1Background.fillAmount = 0;
            if(Player.Ability1Active == true)
            {
                PlayerAbility1Cooldown.fillAmount = 1;
            }
            else
            {
                PlayerAbility1Cooldown.fillAmount = 1 - ((Time.time - Player.LastAbility1CooldownStartedTime) / Player.Ability1CooldownTime);
                PlayerAbility1CDText.text = (Player.Ability1CooldownTime - (Time.time - Player.LastAbility1CooldownStartedTime)).ToString("0.0");
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
        if(Player.Ability2OnCooldown == true || Player.Ability2Active == true)
        {
            // Fill Ability 2 cooldown meter accordingly
            PlayerAbility2Background.fillAmount = 0;
            if(Player.Ability2Active == true)
            {
                PlayerAbility2Cooldown.fillAmount = 1;
            }
            else
            {
                PlayerAbility2Cooldown.fillAmount = 1 - ((Time.time - Player.LastAbility2CooldownStartedTime) / Player.Ability2CooldownTime);
                PlayerAbility2CDText.text = (Player.Ability2CooldownTime - (Time.time - Player.LastAbility2CooldownStartedTime)).ToString("0.0");
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
        if(Player.Ability3OnCooldown == true || Player.Ability3Active == true)
        {
            // Fill Ability 3 cooldown meter accordingly
            PlayerAbility3Background.fillAmount = 0;
            if(Player.Ability3Active == true)
            {
                PlayerAbility3Cooldown.fillAmount = 1;
            }
            else
            {
                PlayerAbility3Cooldown.fillAmount = 1 - ((Time.time - Player.LastAbility3CooldownStartedTime) / Player.Ability3CooldownTime);
                PlayerAbility3CDText.text = (Player.Ability3CooldownTime - (Time.time - Player.LastAbility3CooldownStartedTime)).ToString("0.0");
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
    public static void GameOver()
    {
        GameOverMenuScreen.SetActive(true);
        GameOverText = GameObject.Find(GameController.GameOverTextName);
        GameOverText.GetComponent<TextMeshProUGUI>().text = $@"GAME OVER{Environment.NewLine}{Environment.NewLine}TIME: {TimeString}{Environment.NewLine}SCORE: {GameController.Score}";
    }

    // Show main menu screen
    public static void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        MainMenuContainer.SetActive(true);
        NewGameContainer.SetActive(false);
    }

    // Hide main menu screen
    public static void HideMainMenu()
    {
        MainMenu.SetActive(false);
    }

    // Show new game menu screen
    public static void ShowNewGameMenu()
    {
        UIType = UITypeEnum.NewGameMenu;
        MainMenuContainer.SetActive(false);
        NewGameContainer.SetActive(true);
    }

    // Exit new game menu screen
    public static void ExitNewGameMenu()
    {
        UIType = UITypeEnum.MainMenu;
        MainMenuContainer.SetActive(true);
        NewGameContainer.SetActive(false);
    }

    // Show UI
    public static void ShowUI()
    {
        UI.SetActive(true);
    }

    // Hide UI
    public static void HideUI()
    {
        UI.SetActive(false);
    }

    // Show pause menu
    public static void ShowPauseMenu()
    {
        PauseMenuScreen.SetActive(true);
        // Pause physics simulation time
        Time.timeScale = 0;
    }

    // Hide pause menu
    public static void HidePauseMenu()
    {
        PauseMenuScreen.SetActive(false);
        // Set physics simulation time to default
        Time.timeScale = 1;
    }

    // Restart
    public static void Restart()
    {
        GameOverMenuScreen.SetActive(false);
        // Loop through healthbar uis
        foreach(KeyValuePair<uint, GameObject> healthbarui in HealthbarUIs)
        {
            // Destroy game object for healthbar
            GameObject.Destroy(healthbarui.Value);
        }
        HealthbarUIs.Clear();
    }
}
