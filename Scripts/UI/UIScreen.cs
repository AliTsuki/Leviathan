using UnityEngine;

public class UIScreen : MonoBehaviour
{
    // Fields
    [SerializeField]
    public GameObject PreviousScreen;
    [SerializeField]
    public SlideDirectionEnum SlideDirection = SlideDirectionEnum.Right;
    [SerializeField, Range(0.001f, 0.999f)]
    public float SlideRate = 0.2f;

    // Private fields
    private RectTransform Rect;
    private bool IsCurrentlySliding = false;
    private InOutEnum CurrentSlidingStatus = InOutEnum.In;
    private Vector2 OutwardRestingPosition = new Vector2();

    // Animation direction
    public enum SlideDirectionEnum
    {
        Up,
        Down,
        Left,
        Right
    }
    // Animation in or out
    public enum InOutEnum
    {
        In,
        Out
    }


    // Start is called before the first frame update
    private void Start()
    {
        // Get the rect transform component
        this.Rect = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void Update()
    {
        // If sliding in
        if(this.CurrentSlidingStatus == InOutEnum.In)
        {
            // If currently sliding and not yet to zero position
            if(this.IsCurrentlySliding == true && this.Rect.anchoredPosition != Vector2.zero)
            {
                // Lerp toward zero position by specified rate
                this.Rect.anchoredPosition = Vector2.Lerp(this.Rect.anchoredPosition, Vector2.zero, this.SlideRate);
            }
            // If currently sliding and reached zero position
            else
            {
                // Stop sliding
                this.IsCurrentlySliding = false;
            }
        }
        else if(this.CurrentSlidingStatus == InOutEnum.Out)
        {
            // If currently sliding and not yet to zero position
            if(this.IsCurrentlySliding == true && this.Rect.anchoredPosition != this.OutwardRestingPosition)
            {
                // Lerp toward zero position by specified rate
                this.Rect.anchoredPosition = Vector2.Lerp(this.Rect.anchoredPosition, this.OutwardRestingPosition, this.SlideRate);
            }
            // If currently sliding and reached zero position
            else
            {
                // Stop sliding
                this.IsCurrentlySliding = false;
                // Deactivate game object
                this.gameObject.SetActive(false);
            }
        }
    }

    // Slide screen
    public void SlideScreen(InOutEnum _inOrOut)
    {
        // Set to is currently sliding
        this.IsCurrentlySliding = true;
        // Get sliding status
        this.CurrentSlidingStatus = _inOrOut;
        // If sliding in
        if(this.CurrentSlidingStatus == InOutEnum.In)
        {
            // Activate this screen's gameobject
            this.gameObject.SetActive(true);
            // Get screen height and width
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            // If direction is up
            if(this.SlideDirection == SlideDirectionEnum.Up)
            {
                // Set position to be one screen height above
                this.Rect.anchoredPosition = new Vector2(0f, screenHeight);
            }
            // If direction is down
            else if(this.SlideDirection == SlideDirectionEnum.Down)
            {
                // Set position to be one screen height below
                this.Rect.anchoredPosition = new Vector2(0f, -screenHeight);
            }
            // If direction is left
            else if(this.SlideDirection == SlideDirectionEnum.Left)
            {
                // Set position to be one screen width left
                this.Rect.anchoredPosition = new Vector2(-screenWidth, 0f);
            }
            // If direction is right
            else if(this.SlideDirection == SlideDirectionEnum.Right)
            {
                // Set position to be one screen width right
                this.Rect.anchoredPosition = new Vector2(screenWidth, 0f);
            }
        }
        // If sliding out
        else if(this.CurrentSlidingStatus == InOutEnum.Out)
        {
            // Get screen height and width
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            // If direction is up
            if(this.SlideDirection == SlideDirectionEnum.Up)
            {
                // Set resting position to be one screen height above
                this.OutwardRestingPosition = new Vector2(0f, screenHeight);
            }
            // If direction is down
            else if(this.SlideDirection == SlideDirectionEnum.Down)
            {
                // Set resting position to be one screen height below
                this.OutwardRestingPosition = new Vector2(0f, -screenHeight);
            }
            // If direction is left
            else if(this.SlideDirection == SlideDirectionEnum.Left)
            {
                // Set resting position to be one screen width left
                this.OutwardRestingPosition = new Vector2(-screenWidth, 0f);
            }
            // If direction is right
            else if(this.SlideDirection == SlideDirectionEnum.Right)
            {
                // Set resting position to be one screen width right
                this.OutwardRestingPosition = new Vector2(screenWidth, 0f);
            }
        }
    }
}
