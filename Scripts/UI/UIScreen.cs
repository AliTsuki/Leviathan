using System.Collections.Generic;

using UnityEngine;

// UIScreen script is added to a screen gameobject in Editor
[RequireComponent(typeof(RectTransform))]
public class UIScreen : MonoBehaviour
{
    // Editor fields
    [SerializeField]
    public UIScreen BackScreen;
    [SerializeField]
    public UIAnimations.SlideDirectionEnum SlideDirection = UIAnimations.SlideDirectionEnum.Right;
    [SerializeField, Range(0.001f, 0.999f)]
    public float SlideRate = 0.2f;
    [SerializeField]
    public GameObject[] Buttons;
    [SerializeField]
    public GameObject[] ModifiableTexts;
    [SerializeField]
    public GameObject[] ModifiableUIElements;

    // Dictionaries
    public Dictionary<string, GameObject> ButtonDict { get; private set; } = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> ModifiableTextDict { get; private set; } = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> ModifiableUIElementDict { get; private set; } = new Dictionary<string, GameObject>();

    // Private fields
    private RectTransform Rect;
    private readonly UIAnimations UIAnims = new UIAnimations();


    // Start is called before the first frame update
    private void Start()
    {
        // Get the rect transform component
        this.Rect = this.gameObject.GetComponent<RectTransform>();
        // Assign values to dictionaries
        foreach(GameObject button in this.Buttons)
        {
            this.ButtonDict.Add(button.name, button);
        }
        foreach(GameObject text in this.ModifiableTexts)
        {
            this.ModifiableTextDict.Add(text.name, text);
        }
        foreach(GameObject element in this.ModifiableUIElements)
        {
            this.ModifiableUIElementDict.Add(element.name, element);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        this.UIAnims.UpdateScreenSlide(this.gameObject, this.Rect, this.SlideRate);
    }

    // Set up slide animation
    public void SetupSlideScreen(UIAnimations.InOutEnum _inOrOut)
    {
        this.UIAnims.SetupSlideScreen(this.gameObject, this.Rect, _inOrOut, this.SlideDirection);
    }
}
