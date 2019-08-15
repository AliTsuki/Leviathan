using UnityEngine;

// Animations for UI elements
public class UIAnimations
{
    // Private fields
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


    // Update screen slide
    public void UpdateScreenSlide(GameObject go, RectTransform rect, float slideRate)
    {
        // If sliding in
        if(this.IsCurrentlySliding == true && this.CurrentSlidingStatus == InOutEnum.In)
        {
            // If not yet to zero position
            if(rect.anchoredPosition != Vector2.zero)
            {
                // Lerp toward zero position by specified rate
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, Vector2.zero, slideRate);
            }
            // If reached zero position
            else
            {
                // Stop sliding
                this.IsCurrentlySliding = false;
            }
        }
        // If sliding out
        else if(this.IsCurrentlySliding == true && this.CurrentSlidingStatus == InOutEnum.Out)
        {
            // If not yet to zero position
            if(Vector2.Distance(rect.anchoredPosition, this.OutwardRestingPosition) > 0.1f)
            {
                // Lerp toward zero position by specified rate
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, this.OutwardRestingPosition, slideRate);
            }
            // If reached zero position
            else
            {
                // Stop sliding
                this.IsCurrentlySliding = false;
            }
        }
    }

    // Slide screen
    public void SetupSlideScreen(GameObject go, RectTransform rect, InOutEnum inOrOut, SlideDirectionEnum slideDirection)
    {
        // Set to is currently sliding
        this.IsCurrentlySliding = true;
        // Get sliding status
        this.CurrentSlidingStatus = inOrOut;
        // If sliding in
        if(this.CurrentSlidingStatus == InOutEnum.In)
        {
            // Activate this screen's gameobject
            go.SetActive(true);
            // Get screen height and width
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            // If direction is up
            if(slideDirection == SlideDirectionEnum.Up)
            {
                // Set position to be one screen height above
                rect.anchoredPosition = new Vector2(0f, screenHeight);
            }
            // If direction is down
            else if(slideDirection == SlideDirectionEnum.Down)
            {
                // Set position to be one screen height below
                rect.anchoredPosition = new Vector2(0f, -screenHeight);
            }
            // If direction is left
            else if(slideDirection == SlideDirectionEnum.Left)
            {
                // Set position to be one screen width left
                rect.anchoredPosition = new Vector2(-screenWidth, 0f);
            }
            // If direction is right
            else if(slideDirection == SlideDirectionEnum.Right)
            {
                // Set position to be one screen width right
                rect.anchoredPosition = new Vector2(screenWidth, 0f);
            }
        }
        // If sliding out
        else if(this.CurrentSlidingStatus == InOutEnum.Out)
        {
            // Get screen height and width
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            // If direction is up
            if(slideDirection == SlideDirectionEnum.Up)
            {
                // Set resting position to be one screen height above
                this.OutwardRestingPosition = new Vector2(0f, screenHeight);
            }
            // If direction is down
            else if(slideDirection == SlideDirectionEnum.Down)
            {
                // Set resting position to be one screen height below
                this.OutwardRestingPosition = new Vector2(0f, -screenHeight);
            }
            // If direction is left
            else if(slideDirection == SlideDirectionEnum.Left)
            {
                // Set resting position to be one screen width left
                this.OutwardRestingPosition = new Vector2(-screenWidth, 0f);
            }
            // If direction is right
            else if(slideDirection == SlideDirectionEnum.Right)
            {
                // Set resting position to be one screen width right
                this.OutwardRestingPosition = new Vector2(screenWidth, 0f);
            }
        }
    }
}
