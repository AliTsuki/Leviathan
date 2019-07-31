using UnityEngine;
using UnityEngine.UI;

// UI elements for Ship Select Menu
public class UIElementsShipSelect : MonoBehaviour
{
    // Element references
    public Toggle[] ShipToggles;
    public Toggle CurrentSelection;

    // Const names
    public const string BomberToggleName = "Bomber Toggle (Ship Select)";
    public const string EngineerToggleName = "Engineer Toggle (Ship Select)";


    // Start is called before the first frame update
    private void Start()
    {
        this.GetShipSelectToggle();
    }

    // Get currently selected toggle
    public void GetShipSelectToggle()
    {
        // Loop through each toggle
        foreach(Toggle type in this.ShipToggles)
        {
            // If toggle is on
            if(type.isOn == true)
            {
                // Get currently selected toggle
                this.CurrentSelection = type;
            }
        }
    }
}