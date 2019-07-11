using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Sends Start(), Update(), and FixedUpdate() calls from UnityEngine to GameController to propogate to all non-MonoBehaviour scripts
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Start is called before the first frame update
    public void Start()
    {
        instance = this;
        GameController.Initialize();
    }

    // Update is called once per frame
    public void Update()
    {
        GameController.Update();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public void FixedUpdate()
    {
        GameController.FixedUpdate();
    }

    // On application quit
    public void OnApplicationQuit()
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
        UIController.eventSystem.currentSelectedGameObject.GetComponent<Button>().OnDeselect(null);
        UIController.eventSystem.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        _button.GetComponent<Button>().Select();
        UIController.eventSystem.SetSelectedGameObject(_button);
    }
}
