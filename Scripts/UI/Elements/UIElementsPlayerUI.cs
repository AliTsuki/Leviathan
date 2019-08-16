using TMPro;

using UnityEngine;
using UnityEngine.UI;

// UI elements for Player UI
public class UIElementsPlayerUI : MonoBehaviour
{
    // Element references
    [Header("Healthbars")]
    [SerializeField]
    public GameObject HealthbarsParent;
    [Header("Info Label")]
    [SerializeField]
    public TextMeshProUGUI InfoLabel;
    [Header("Minimap")]
    [SerializeField]
    public TextMeshProUGUI MinimapCoords;
    [Header("Health, Shields, Energy")]
    [SerializeField]
    public GameObject ShieldDamageEffect;
    [SerializeField]
    public GameObject HealthDamageEffect;
    [SerializeField]
    public Image HealthForeground;
    [SerializeField]
    public TextMeshProUGUI HealthText;
    [SerializeField]
    public Image ShieldForeground;
    [SerializeField]
    public TextMeshProUGUI ShieldText;
    [SerializeField]
    public Image EnergyForeground;
    [SerializeField]
    public TextMeshProUGUI EnergyText;
    [Header("Abilities")]
    [SerializeField]
    public Image Ability1Background;
    [SerializeField]
    public Image Ability1Icon;
    [SerializeField]
    public Image Ability1CD;
    [SerializeField]
    public TextMeshProUGUI Ability1CDText;
    [SerializeField]
    public Image Ability2Background;
    [SerializeField]
    public Image Ability2Icon;
    [SerializeField]
    public Image Ability2CD;
    [SerializeField]
    public TextMeshProUGUI Ability2CDText;
    [SerializeField]
    public Image Ability3Background;
    [SerializeField]
    public Image Ability3Icon;
    [SerializeField]
    public Image Ability3CD;
    [SerializeField]
    public TextMeshProUGUI Ability3CDText;
}
