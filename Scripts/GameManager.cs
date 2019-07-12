using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Sends Start(), Update(), and FixedUpdate() calls from UnityEngine to GameController to propogate to all non-MonoBehaviour scripts
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

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

    // Start a coroutine
    public void StartSelectDefaultButtonCoroutine(GameObject _button)
    {
        this.StartCoroutine(SelectDefaultButton(_button));
    }

    // Select default button after frame update
    private static IEnumerator SelectDefaultButton(GameObject _button)
    {
        UIController.CurrentEventSystem.currentSelectedGameObject.GetComponent<Button>().OnDeselect(null);
        UIController.CurrentEventSystem.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        _button.GetComponent<Button>().Select();
        UIController.CurrentEventSystem.SetSelectedGameObject(_button);
    }
}
