using TMPro;

using UnityEngine;

// Contains references to player ability icons
public class UIPlayerAbilityIcons : MonoBehaviour
{
    // Ability icon references
    [Header("Icons")]
    [SerializeField]
    public Sprite[] BomberAbilityIcons = new Sprite[3];
    [SerializeField]
    public Sprite[] EngineerAbilityIcons = new Sprite[3];

    // Keybind text references
    [Header("Keybind Text")]
    [SerializeField]
    public TextMeshProUGUI Ability1Keybind;
    [SerializeField]
    public TextMeshProUGUI Ability2Keybind;
    [SerializeField]
    public TextMeshProUGUI Ability3Keybind;
    [SerializeField]
    public TextMeshProUGUI MainGunKeybind;
    [SerializeField]
    public TextMeshProUGUI WarpKeybind;
    [SerializeField]
    public TextMeshProUGUI MoveKeybind;
    [SerializeField]
    public TextMeshProUGUI AimKeybind;
    [SerializeField]
    public TextMeshProUGUI PauseKeybind;
}
