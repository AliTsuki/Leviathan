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

    // Dictionary of Healthbar UIs
    private static Dictionary<uint, GameObject> HealthbarUIs = new Dictionary<uint, GameObject>();

    // Fields
    private static Vector2 UIOffset;


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
        // TODO: Add a vignette effect around the edges of the screen when player takes damage
        // TODO: \r\n doesn't seem to work with TMPro, switch to a label or something else maybe
        InfoLabel.text = $@"Timer: {Time.time} \r\nScore: {GameController.Score}";
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
                HealthbarUI.GetComponent<RectTransform>().localPosition = ProportionalPosition - UIOffset;
                // Fill the healthbar relative to ship's health value
                Image ShieldbarFillImageBackground =    HealthbarUI.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                Image ShieldbarFillImage =              HealthbarUI.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                Image HealthbarFillImageBackground =    HealthbarUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>();
                Image HealthbarFillImage =              HealthbarUI.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
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
    }

    // Remove Healthbar
    public static void RemoveHealthbar(uint _id)
    {
        GameObject.Destroy(HealthbarUIs[_id]);
        HealthbarUIs.Remove(_id);
    }
}
