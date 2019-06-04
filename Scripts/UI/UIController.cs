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
    private static GameObject UI;
    private static GameObject canvas;
    private static RectTransform rectTransform;
    private static GameObject PlayerUI;
    private static GameObject NPCUIPrefab;
    private static GameObject HealthbarUI;
    private static GameObject MinimapCoords;
    private static TextMeshProUGUI InfoLabel;
    private static GameObject ShieldDamageEffect;
    private static GameObject HealthDamageEffect;
    private static GameObject PauseMenuScreen;
    private static GameObject GameOverScreen;
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


    // Initialize
    public static void Initialize()
    {
        HealthbarUIs.Clear();
        MainMenu = GameObject.Find(GameController.MainMenuName);
        UI = GameObject.Find(GameController.UIName);
        canvas = GameObject.Find(GameController.CanvasName);
        rectTransform = canvas.GetComponent<RectTransform>();
        MinimapCoords = GameObject.Find(GameController.MinimapCoordsName);
        InfoLabel = GameObject.Find(GameController.InfoLabelName).GetComponent<TextMeshProUGUI>();
        ShieldDamageEffect = GameObject.Find(GameController.ShieldDamageEffectName);
        ShieldDamageEffect.SetActive(false);
        HealthDamageEffect = GameObject.Find(GameController.HealthDamageEffectName);
        HealthDamageEffect.SetActive(false);
        PauseMenuScreen = GameObject.Find(GameController.PauseMenuScreenName);
        GameOverScreen = GameObject.Find(GameController.GameOverScreenName);
        GameOverScreen.SetActive(false);
        GameOverText = GameObject.Find(GameController.GameOverTextName);
        PlayerUI = GameObject.Find(GameController.PlayerUIName);
        NPCUIPrefab = Resources.Load<GameObject>(GameController.NPCUIPrefabName);
    }

    // Update is called once per frame
    public static void Update()
    {
        if(GameController.CurrentGameState == GameController.GameState.MainMenu)
        {
            ShowMainMenu();
            HideUI();
            HidePauseMenu();
        }
        // If game state is playing
        else if(GameController.CurrentGameState == GameController.GameState.Playing)
        {
            HideMainMenu();
            ShowUI();
            HidePauseMenu();
            if(HealthbarUIs.Count < 1)
            {
                HealthbarUIs.Add(1, PlayerUI); // TODO: replace 1 with actual player ID
                HealthbarUIs[1].SetActive(true);
            }
            // Minimap coords
            Vector3 PlayerPosition = GameController.Player.ShipObject.transform.position;
            MinimapCoords.GetComponent<TextMeshProUGUI>().text = $@"X: {Mathf.RoundToInt(PlayerPosition.x)}{Environment.NewLine}Z: {Mathf.RoundToInt(PlayerPosition.z)}";
            // Add info to info label
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            FPS = Mathf.FloorToInt(1.0f / deltaTime);
            Seconds = Mathf.FloorToInt((Time.time - GameController.TimeStarted) % 60);
            Minutes = Mathf.FloorToInt((Time.time - GameController.TimeStarted) / 60 % 60);
            Hours = Mathf.FloorToInt((Time.time - GameController.TimeStarted) / 3600);
            TimeString = $@"{Hours}:{(Minutes < 10 ? "0" + Minutes.ToString() : Minutes.ToString())}:{(Seconds < 10 ? "0" + Seconds.ToString() : Seconds.ToString())}";
            InfoLabel.text = $@"FPS: {FPS}{Environment.NewLine}Timer: {TimeString}{Environment.NewLine}Score: {GameController.Score}";
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
                        HealthbarUI.transform.SetParent(canvas.transform, false);
                        HealthbarUI.name = $@"Healthbar: {ship.Key}";
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
                        // If ship is NPC
                        if(ship.Value.IsPlayer == false)
                        {
                            // Move healthbar to appear where ship appears on the screen
                            HealthbarUI.GetComponent<RectTransform>().localPosition = ProportionalPosition - UIOffset;
                        }
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
                        // If ship is player
                        if(ship.Value.IsPlayer)
                        {
                            // Fill energy bar accordingly
                            HealthbarUI.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = ship.Value.Energy / ship.Value.MaxEnergy;
                            // If Ability 1 is on cooldown or currently active
                            if(ship.Value.Ability1OnCooldown == true || ship.Value.Ability1Active == true)
                            {
                                // Fill Ablity 1 cooldown meter accordingly
                                HealthbarUI.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                                if(ship.Value.Ability1Active == true)
                                {
                                    HealthbarUI.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1;
                                }
                                else
                                {
                                    HealthbarUI.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - ((Time.time - ship.Value.LastAbility1CooldownStartedTime) / ship.Value.Ability1CooldownTime);
                                }
                            }
                            // If Ability 1 is not on cooldown
                            else
                            {
                                // Fill Ability 1 cooldown meter accordingly
                                HealthbarUI.transform.GetChild(2).GetComponent<Image>().fillAmount = 1;
                                HealthbarUI.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 0;
                            }
                            // If Ability 2 is on cooldown or currently active
                            if(ship.Value.Ability2OnCooldown == true || ship.Value.Ability2Active == true)
                            {
                                // Fill Ability 2 cooldown meter accordingly
                                HealthbarUI.transform.GetChild(3).GetComponent<Image>().fillAmount = 0;
                                if(ship.Value.Ability2Active == true)
                                {
                                    HealthbarUI.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1;
                                }
                                else
                                {
                                    HealthbarUI.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - ((Time.time - ship.Value.LastAbility2CooldownStartedTime) / ship.Value.Ability2CooldownTime);
                                }
                            }
                            // If Ability 2 is not on cooldown
                            else
                            {
                                // Fill Ability 2 cooldown meter accordingly
                                HealthbarUI.transform.GetChild(3).GetComponent<Image>().fillAmount = 1;
                                HealthbarUI.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 0;
                            }
                            // If Ability 3 is on cooldown or currently active
                            if(ship.Value.Ability3OnCooldown == true || ship.Value.Ability3Active == true)
                            {
                                // Fill Ability 3 cooldown meter accordingly
                                HealthbarUI.transform.GetChild(4).GetComponent<Image>().fillAmount = 0;
                                if(ship.Value.Ability3Active == true)
                                {
                                    HealthbarUI.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1;
                                }
                                else
                                {
                                    HealthbarUI.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - ((Time.time - ship.Value.LastAbility3CooldownStartedTime) / ship.Value.Ability3CooldownTime);
                                }
                            }
                            // If Ability 3 is not on cooldown
                            else
                            {
                                // Fill bomb cooldown meter accordingly
                                HealthbarUI.transform.GetChild(4).GetComponent<Image>().fillAmount = 1;
                                HealthbarUI.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 0;
                            }
                        }
                    }
                }
                // Remove healthbar if healthbar currently exists on a ship that is dead or has no GameObject
                else if(HealthbarUIs.ContainsKey(ship.Key) == true && (ship.Value.Alive == false || ship.Value.ShipObject == null))
                {
                    RemoveHealthbar(ship.Key);
                }
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
        // If game state is paused
        else if(GameController.CurrentGameState == GameController.GameState.Paused)
        {
            ShowPauseMenu();
        }
    }

    // Remove Healthbar
    public static void RemoveHealthbar(uint _id)
    {
        if(_id != 1)
        {
            GameObject.Destroy(HealthbarUIs[_id]);
        }
        else
        {
            HealthbarUIs[_id].SetActive(false);
        }
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
        GameOverScreen.SetActive(true);
        GameOverText = GameObject.Find(GameController.GameOverTextName);
        GameOverText.GetComponent<TextMeshProUGUI>().text = $@"GAME OVER{Environment.NewLine}{Environment.NewLine}TIME: {TimeString}{Environment.NewLine}SCORE: {GameController.Score}";
    }

    // Show main menu screen
    public static void ShowMainMenu()
    {
        MainMenu.SetActive(true);
    }

    // Hide main menu screen
    public static void HideMainMenu()
    {
        MainMenu.SetActive(false);
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
        GameOverScreen.SetActive(false);
        // Loop through healthbar uis
        foreach(KeyValuePair<uint, GameObject> healthbarui in HealthbarUIs)
        {
            // If healthbar is for player
            if(healthbarui.Key == 1)
            {
                // Hide healthbar
                HealthbarUI.SetActive(false);
            }
            // If healthbar is for anyone else
            else
            {
                // Destroy game object for healthbar
                GameObject.Destroy(healthbarui.Value);
            }
        }
        HealthbarUIs.Clear();
    }
}
