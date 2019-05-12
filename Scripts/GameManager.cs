using UnityEngine;

// Makes an instance of the game controller and calls Start, Update, and FixedUpdate
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        GameController.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        GameController.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        GameController.FixedUpdate();
    }
}
