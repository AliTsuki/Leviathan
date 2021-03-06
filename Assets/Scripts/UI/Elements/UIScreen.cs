﻿using UnityEngine;

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
    public GameObject DefaultButton;

    // State
    public bool Active = false;

    // Private fields
    private RectTransform Rect;
    private readonly UIAnimations UIAnims = new UIAnimations();


    // Start is called before the first frame update
    private void Start()
    {
        // Get the rect transform component
        this.Rect = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void Update()
    {
        this.UIAnims.UpdateScreenSlide(this.gameObject, this.Rect, this.SlideRate);
    }

    // Set up slide animation
    public void SetupSlideScreen(UIAnimations.InOutEnum inOrOut)
    {
        this.UIAnims.SetupSlideScreen(this.gameObject, this.Rect, inOrOut, this.SlideDirection);
    }
}
