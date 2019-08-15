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
    public int MovementStyle;
    public int InputType;


    // Start is called before the first frame update
    private void Start()
    {
        this.UpdateSettingsToggles();
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

    // Update settings toggles
    public void UpdateSettingsToggles()
    {
        this.MovementStyle = (int)PlayerInput.InputSettings.MovementStyle;
        this.MovementStyleDropdown.value = this.MovementStyle;
        this.InputType = (int)PlayerInput.InputSettings.InputMode;
        this.InputTypeDropdown.value = this.InputType;
    }
}
