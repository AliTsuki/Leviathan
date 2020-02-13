using UnityEngine;

// Sends Start(), Update(), and FixedUpdate() calls from UnityEngine to GameController to propogate to all non-MonoBehaviour scripts
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // References
    [Header("Game Object References")]
    [SerializeField]
    public GameObject Cameras;

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        GameController.Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
        GameController.Update();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    private void FixedUpdate()
    {
        GameController.FixedUpdate();
    }

    // On application quit
    private void OnApplicationQuit()
    {
        GameController.OnApplicationQuit();
    }
}
