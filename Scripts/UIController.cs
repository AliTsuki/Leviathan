using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using UnityEngine.UI;

// Controls the game UI
public static class UIController
{
    // GameObjects
    private static GameObject MainMenuPrefab;
    private static GameObject MainMenu;
    private static GameObject UIPrefab;
    private static GameObject UI;
    private static GameObject canvas;
    private static RectTransform rectTransform;
    private static GameObject PlayerUI;
    private static GameObject NPCUIPrefab;
    private static GameObject HealthbarUI;
    private static TextMeshProUGUI InfoLabel;
    private static GameObject ShieldDamageEffect;
    private static GameObject HealthDamageEffect;
    private static GameObject GameOverScreen;

    // Dictionary of Healthbar UIs
    private static Dictionary<uint, GameObject> HealthbarUIs = new Dictionary<uint, GameObject>();

    // Fields
    private static Vector2 UIOffset;
    private static float deltaTime = 0.0f;
    private static int FPS;
    private static float ShieldDamageEffectStartTime = 0f;
    private static float HealthDamageEffectStartTime = 0f;
    private readonly static float ShowDamageEffectDuration = 0.25f;
    private static int Seconds = 0;
    private static int Minutes = 0;
    private static int Hours = 0;
    private static string TimeString = "";


    // Initialize is called before the first frame update
    public static void Initialize()
    {
        // Instantiate main menu
        MainMenuPrefab = Resources.Load<GameObject>(GameController.MainMenuPrefabName);
        MainMenu = GameObject.Instantiate(MainMenuPrefab);
        MainMenu.name = "Main Menu";
        // Instantiate UI
        UIPrefab = Resources.Load<GameObject>(GameController.UIPrefabName);
        UI = GameObject.Instantiate(UIPrefab);
        UI.name = "UI";
        canvas = GameObject.Find(GameController.CanvasName);
        rectTransform = canvas.GetComponent<RectTransform>();
        InfoLabel = GameObject.Find(GameController.InfoLabelName).GetComponent<TextMeshProUGUI>();
        ShieldDamageEffect = GameObject.Find(GameController.ShieldDamageEffectName);
        ShieldDamageEffect.SetActive(false);
        HealthDamageEffect = GameObject.Find(GameController.HealthDamageEffectName);
        HealthDamageEffect.SetActive(false);
        GameOverScreen = GameObject.Find(GameController.GameOverScreenName);
        GameOverScreen.SetActive(false);
        PlayerUI = GameObject.Find(GameController.PlayerUIName);
        NPCUIPrefab = Resources.Load<GameObject>(GameController.NPCUIPrefabName);
        HealthbarUIs.Add(1, PlayerUI); // TODO: replace 1 with actual player ID
    }

    // Update is called once per frame
    public static void Update()
    {
        if(GameController.CurrentGameState == GameController.GameState.MainMenu)
        {

        }
        else if(GameController.CurrentGameState == GameController.GameState.Playing)
        {
            // Add info to info label
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            FPS = Mathf.FloorToInt(1.0f / deltaTime);
            Seconds = Mathf.FloorToInt(Time.time % 60);
            Minutes = Mathf.FloorToInt(Time.time / 60 % 60);
            Hours = Mathf.FloorToInt(Time.time / 3600);
            TimeString = $@"{Hours}:{(Minutes < 10 ? "0" + Minutes.ToString() : Minutes.ToString())}:{(Seconds < 10 ? "0" + Seconds.ToString() : Seconds.ToString())}";
            InfoLabel.text = $@"FPS: {FPS}{Environment.NewLine}Timer: {TimeString}{Environment.NewLine}Score: {GameController.Score}";
            // Get UIOffset again, in case user has changed screen size during play, subtract 60 relative pixels from y to have healthbar appear above ship
            UIOffset = new Vector2(rectTransform.sizeDelta.x / 2f, (rectTransform.sizeDelta.y / 2f) - 60);
            // Loop through all ships to determine if healthbar UIs need to be added or moved
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
                    // Set the position of the healthbar UI
                    Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(ship.Value.ShipObject.transform.position);
                    Vector2 ProportionalPosition = new Vector2(ViewportPosition.x * rectTransform.sizeDelta.x, ViewportPosition.y * rectTransform.sizeDelta.y);
                    // If ship is NPC
                    if(ship.Value.IsPlayer == false)
                    {
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
                        HealthbarUI.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = ship.Value.Energy / ship.Value.MaxEnergy;
                        if(ship.Value.BombOnCooldown == true)
                        {
                            HealthbarUI.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                            HealthbarUI.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - ((Time.time - ship.Value.LastBombActivatedTime) / ship.Value.BombCooldownTime);
                        }
                        else
                        {
                            HealthbarUI.transform.GetChild(2).GetComponent<Image>().fillAmount = 1;
                        }
                        if(ship.Value.BarrierOnCooldown == true || ship.Value.BarrierActive)
                        {
                            HealthbarUI.transform.GetChild(3).GetComponent<Image>().fillAmount = 0;
                            HealthbarUI.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - ((Time.time - ship.Value.LastBarrierCooldownStartedTime) / ship.Value.BarrierCooldownTime);
                        }
                        else
                        {
                            HealthbarUI.transform.GetChild(3).GetComponent<Image>().fillAmount = 1;
                        }
                    }
                }
                // Remove healthbar if healthbar currently exists on a ship that is dead or has no GameObject
                else if(HealthbarUIs.ContainsKey(ship.Key) == true && (ship.Value.Alive == false || ship.Value.ShipObject == null))
                {
                    RemoveHealthbar(ship.Key);
                }
            }
            if(ShieldDamageEffect.activeSelf == true && (Time.time - ShieldDamageEffectStartTime > ShowDamageEffectDuration || HealthDamageEffect.activeSelf == true))
            {
                ShieldDamageEffect.SetActive(false);
            }
            if(HealthDamageEffect.activeSelf == true && Time.time - HealthDamageEffectStartTime > ShowDamageEffectDuration)
            {
                HealthDamageEffect.SetActive(false);
            }
        }
        else if(GameController.CurrentGameState == GameController.GameState.Paused)
        {

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
        GameOverScreen.GetComponent<TextMeshProUGUI>().text = $@"GAME OVER{Environment.NewLine}{Environment.NewLine}TIME: {TimeString}{Environment.NewLine}SCORE: {GameController.Score}";
        GameOverScreen.SetActive(true);
    }

    // Show main menu screen
    public static void ShowMainMenu()
    {
        
        MainMenu.SetActive(true);
        HideUI();
    }

    // Hide main menu screen
    public static void HideMainMenu()
    {
        MainMenu.SetActive(false);
        ShowUI();
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
}
