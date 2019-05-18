using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using UnityEngine.UI;

// Controls the game UI
public static class UIController
{
    // GameObjects
    private static GameObject UIPrefab;
    private static GameObject UI;
    private static GameObject canvas;
    private static RectTransform rectTransform;
    private static GameObject PlayerUIPrefab;
    private static GameObject NPCUIPrefab;
    private static GameObject HealthbarUI;
    private static TextMeshProUGUI InfoLabel;
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
    private readonly static float ShowDamageEffectDuration = 0.25f;


    // Start is called before the first frame update
    public static void Start()
    {
        // Instantiate UI
        UIPrefab = Resources.Load(GameController.UIPrefabName, typeof(GameObject)) as GameObject;
        UI = GameObject.Instantiate(UIPrefab);
        UI.name = "UI";
        canvas = GameObject.Find(GameController.CanvasName);
        rectTransform = canvas.GetComponent<RectTransform>();
        InfoLabel = GameObject.Find(GameController.InfoLabelName).GetComponent<TextMeshProUGUI>();
        ShieldDamageEffect = GameObject.Find(GameController.ShieldDamageEffectName);
        HealthDamageEffect = GameObject.Find(GameController.HealthDamageEffectName);
        // Get the Player UI prefab
        PlayerUIPrefab = Resources.Load(GameController.PlayerUIPrefabName, typeof(GameObject)) as GameObject;
        // Get the NPC UI prefab
        NPCUIPrefab = Resources.Load(GameController.NPCUIPrefabName, typeof(GameObject)) as GameObject;
        // Get the UI Offset, , subtract 60 relative pixels from y to have healthbar appear above ship
        UIOffset = new Vector2(rectTransform.sizeDelta.x / 2f, (rectTransform.sizeDelta.y / 2f) - 60);
    }

    // Update is called once per frame
    public static void Update()
    {
        // Add info to info label
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        FPS = Mathf.RoundToInt(1.0f / deltaTime);
        int seconds = Mathf.RoundToInt(Time.time % 60);
        int minutes = Mathf.RoundToInt(Time.time / 60 % 60);
        int hours = Mathf.RoundToInt(Time.time / 3600);
        string TimeString = $@"{hours}:{(minutes < 10 ? "0" + minutes.ToString() : minutes.ToString())}:{(seconds < 10 ? "0" + seconds.ToString() : seconds.ToString())}";
        InfoLabel.text = $@"FPS: {FPS}{Environment.NewLine}Timer: {TimeString}{Environment.NewLine}Score: {GameController.Score}";
        // Get UIOffset again, in case user has changed screen size during play, subtract 60 relative pixels from y to have healthbar appear above ship
        UIOffset = new Vector2(rectTransform.sizeDelta.x / 2f, (rectTransform.sizeDelta.y / 2f) - 60);
        // Loop through all ships to determine if healthbar UIs need to be added or moved
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship doesn't currently have a healthbar
            if(HealthbarUIs.ContainsKey(ship.Key) == false)
            {
                // Player healthbar has different stats to show, so it is a different prefab to be instantiated
                if(ship.Value.IsPlayer == true)
                {
                    // Add a new healthbar UI for player
                    HealthbarUIs.Add(ship.Key, GameObject.Instantiate(PlayerUIPrefab));
                }
                else
                {
                    // Add a new healthbar UI for npc ship
                    HealthbarUIs.Add(ship.Key, GameObject.Instantiate(NPCUIPrefab));
                }
                // Set the parent and name of healthbar object
                HealthbarUI = HealthbarUIs[ship.Key];
                HealthbarUI.transform.SetParent(canvas.transform, false);
                HealthbarUI.name = $@"Healthbar: {ship.Key}";
            }
            // If ship has a healthbar
            if(HealthbarUIs.ContainsKey(ship.Key) == true && ship.Value.Alive == true)
            {
                HealthbarUI = HealthbarUIs[ship.Key];
                // Set the position of the healthbar UI
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(ship.Value.ShipObject.transform.position);
                Vector2 ProportionalPosition = new Vector2(ViewportPosition.x * rectTransform.sizeDelta.x, ViewportPosition.y * rectTransform.sizeDelta.y);
                // Set the position and remove the screen offset
                if(ship.Value.IsPlayer)
                {
                    HealthbarUI.GetComponent<RectTransform>().localPosition = ProportionalPosition - new Vector2(UIOffset.x, UIOffset.y - 25);
                }
                else
                {
                    HealthbarUI.GetComponent<RectTransform>().localPosition = ProportionalPosition - UIOffset;
                }
                // Fill the healthbar relative to ship's health value
                Image ShieldbarFillImageBackground = HealthbarUI.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                Image ShieldbarFillImage = HealthbarUI.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                Image HealthbarFillImageBackground = HealthbarUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>();
                Image HealthbarFillImage = HealthbarUI.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
                ShieldbarFillImageBackground.fillAmount = 1 - (ship.Value.Shields / ship.Value.MaxShields);
                ShieldbarFillImage.fillAmount = ship.Value.Shields / ship.Value.MaxShields;
                HealthbarFillImageBackground.fillAmount = 1 - (ship.Value.Health / ship.Value.MaxHealth);
                HealthbarFillImage.fillAmount = ship.Value.Health / ship.Value.MaxHealth;
                if(HealthbarFillImage.fillAmount < 1 || ShieldbarFillImage.fillAmount < 1)
                {
                    HealthbarUI.SetActive(true);
                }
                // If ship is player, also fill energy bar relative to player energy
                if(ship.Value.IsPlayer)
                {
                    HealthbarUI.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount = ship.Value.Energy / ship.Value.MaxEnergy;
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
}
