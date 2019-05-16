using System.Collections.Generic;

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
    private static GameObject HealthbarPrefab;
    private static GameObject Healthbar;

    // Dictionary of Healthbar UIs
    private static Dictionary<uint, GameObject> Healthbars = new Dictionary<uint, GameObject>();

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
        // Get the Healthbar prefab
        HealthbarPrefab = Resources.Load(GameController.HealthbarPrefabName, typeof(GameObject)) as GameObject;
        // Get the UI Offset
        UIOffset = new Vector2((float)rectTransform.sizeDelta.x / 2f, (float)rectTransform.sizeDelta.y / 2f - 60);
    }

    // Update is called once per frame
    public static void Update()
    {
        // Get UIOffset again, in case user has changed screen size during play
        UIOffset = new Vector2((float)rectTransform.sizeDelta.x / 2f, (float)rectTransform.sizeDelta.y / 2f - 60); //-30 puts it above
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            // If ship doesn't currently have a healthbar
            if(Healthbars.ContainsKey(ship.Key) == false)
            {
                // Add a new healthbar for that ship
                Healthbars.Add(ship.Key, GameObject.Instantiate(HealthbarPrefab));
                // Set the parent and name of healthbar object
                Healthbar = Healthbars[ship.Key];
                Healthbar.transform.SetParent(canvas.transform, false);
                Healthbar.name = $@"Healthbar: {ship.Key}";
            }
            // If ship has a healthbar
            if(Healthbars.ContainsKey(ship.Key) == true && ship.Value.Alive == true)
            {
                Healthbar = Healthbars[ship.Key];
                // Set the position of the healthbar UI
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(ship.Value.ShipObject.transform.position);
                Vector2 ProportionalPosition = new Vector2(ViewportPosition.x * rectTransform.sizeDelta.x, ViewportPosition.y * rectTransform.sizeDelta.y);
                // Set the position and remove the screen offset
                Healthbar.GetComponent<RectTransform>().localPosition = ProportionalPosition - UIOffset;
                Healthbar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount = ship.Value.Health / ship.Value.MaxHealth;
            }
            else if(Healthbars.ContainsKey(ship.Key) == true && ship.Value.Alive == false || ship.Value.ShipObject == null)
            {
                RemoveHealthbar(ship.Key);
            }
        }
    }

    // Remove Healthbar
    public static void RemoveHealthbar(uint _id)
    {
        GameObject.Destroy(Healthbars[_id]);
        Healthbars.Remove(_id);
    }
}
