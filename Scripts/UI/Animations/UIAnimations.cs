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
    public void UpdateScreenSlide(GameObject _go, RectTransform _rect, float _slideRate)
    {
        // If sliding in
        if(this.IsCurrentlySliding == true && this.CurrentSlidingStatus == InOutEnum.In)
        {
            // If not yet to zero position
            if(_rect.anchoredPosition != Vector2.zero)
            {
                // Lerp toward zero position by specified rate
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, Vector2.zero, _slideRate);
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
            if(Vector2.Distance(_rect.anchoredPosition, this.OutwardRestingPosition) > 0.1f)
            {
                // Lerp toward zero position by specified rate
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, this.OutwardRestingPosition, _slideRate);
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
    public void SetupSlideScreen(GameObject _go, RectTransform _rect, InOutEnum _inOrOut, SlideDirectionEnum _slideDirection)
    {
        // Set to is currently sliding
        this.IsCurrentlySliding = true;
        // Get sliding status
        this.CurrentSlidingStatus = _inOrOut;
        // If sliding in
        if(this.CurrentSlidingStatus == InOutEnum.In)
        {
            // Activate this screen's gameobject
            _go.SetActive(true);
            // Get screen height and width
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            // If direction is up
            if(_slideDirection == SlideDirectionEnum.Up)
            {
                // Set position to be one screen height above
                _rect.anchoredPosition = new Vector2(0f, screenHeight);
            }
            // If direction is down
            else if(_slideDirection == SlideDirectionEnum.Down)
            {
                // Set position to be one screen height below
                _rect.anchoredPosition = new Vector2(0f, -screenHeight);
            }
            // If direction is left
            else if(_slideDirection == SlideDirectionEnum.Left)
            {
                // Set position to be one screen width left
                _rect.anchoredPosition = new Vector2(-screenWidth, 0f);
            }
            // If direction is right
            else if(_slideDirection == SlideDirectionEnum.Right)
            {
                // Set position to be one screen width right
                _rect.anchoredPosition = new Vector2(screenWidth, 0f);
            }
        }
        // If sliding out
        else if(this.CurrentSlidingStatus == InOutEnum.Out)
        {
            // Get screen height and width
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            // If direction is up
            if(_slideDirection == SlideDirectionEnum.Up)
            {
                // Set resting position to be one screen height above
                this.OutwardRestingPosition = new Vector2(0f, screenHeight);
            }
            // If direction is down
            else if(_slideDirection == SlideDirectionEnum.Down)
            {
                // Set resting position to be one screen height below
                this.OutwardRestingPosition = new Vector2(0f, -screenHeight);
            }
            // If direction is left
            else if(_slideDirection == SlideDirectionEnum.Left)
            {
                // Set resting position to be one screen width left
                this.OutwardRestingPosition = new Vector2(-screenWidth, 0f);
            }
            // If direction is right
            else if(_slideDirection == SlideDirectionEnum.Right)
            {
                // Set resting position to be one screen width right
                this.OutwardRestingPosition = new Vector2(screenWidth, 0f);
            }
        }
    }
}
