using TMPro;

using UnityEngine;

// UI elements for rebind menu
public class UIElementsRebind : MonoBehaviour
{
    // Element references
    [Header("Controller Rebind Buttons")]
    [SerializeField]
    public TextMeshProUGUI MainGunController;
    [SerializeField]
    public TextMeshProUGUI Ability1Controller;
    [SerializeField]
    public TextMeshProUGUI Ability2Controller;
    [SerializeField]
    public TextMeshProUGUI Ability3Controller;
    [SerializeField]
    public TextMeshProUGUI WarpController;
    [Header("KBM Rebind Buttons")]
    [SerializeField]
    public TextMeshProUGUI MainGunKBM;
    [SerializeField]
    public TextMeshProUGUI Ability1KBM;
    [SerializeField]
    public TextMeshProUGUI Ability2KBM;
    [SerializeField]
    public TextMeshProUGUI Ability3KBM;
    [SerializeField]
    public TextMeshProUGUI WarpKBM;
    [SerializeField]
    public TextMeshProUGUI MoveUpKBM;
    [SerializeField]
    public TextMeshProUGUI MoveDownKBM;
    [SerializeField]
    public TextMeshProUGUI MoveLeftKBM;
    [SerializeField]
    public TextMeshProUGUI MoveRightKBM;


    // Update rebind button text
    public void UpdateRebindButtonText()
    {
        // Update text for controller rebind buttons
        this.MainGunController.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsController[InputBinding.GameInputsEnum.MainGun]);
        this.Ability1Controller.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsController[InputBinding.GameInputsEnum.Ability1]);
        this.Ability2Controller.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsController[InputBinding.GameInputsEnum.Ability2]);
        this.Ability3Controller.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsController[InputBinding.GameInputsEnum.Ability3]);
        this.WarpController.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsController[InputBinding.GameInputsEnum.Warp]);
        // Update text for KBM rebind buttons
        this.MainGunKBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.MainGun]);
        this.Ability1KBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.Ability1]);
        this.Ability2KBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.Ability2]);
        this.Ability3KBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.Ability3]);
        this.WarpKBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.Warp]);
        this.MoveUpKBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.MoveForward]);
        this.MoveDownKBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.MoveBack]);
        this.MoveLeftKBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.MoveLeft]);
        this.MoveRightKBM.text = PlayerInput.GetStringForKeybind(PlayerInput.InputBindingsKBM[InputBinding.GameInputsEnum.MoveRight]);
    }
}
