using UnityEngine;

// Makes an instance of the game controller and calls Start, Update, and FixedUpdate
public class GameManager : MonoBehaviour
{
    public static GameController instance = new GameController();

    // Start is called before the first frame update
    public void Start()
    {
        instance.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        instance.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        instance.FixedUpdate();
    }
}
