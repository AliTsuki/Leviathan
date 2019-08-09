using TMPro;

using UnityEngine;

// UI elements for settings menu
public class UIElementsSettings : MonoBehaviour
{
    // Element references
    [SerializeField]
    public TMP_Dropdown MovementStyleDropdown;
    [SerializeField]
    public TMP_Dropdown InputTypeDropdown;

    // Current selections
    public int MovementStyle = 0;
    public int InputType = 0;


    // Start is called before the first frame update
    private void Start()
    {
        this.GetMovementStyleToggle();
        this.GetInputTypeToggle();
    }

    // Get movement style toggle
    public void GetMovementStyleToggle()
    {
        this.MovementStyle = this.MovementStyleDropdown.value;
    }

    // Get input type toggle
    public void GetInputTypeToggle()
    {
        this.InputType = this.InputTypeDropdown.value;
    }
}
