using UnityEngine;

// UI PopUp script is added to a pop up object in Editor
public class UIPopUp : MonoBehaviour
{
    // Editor fields
    [SerializeField]
    public bool DefaultActive = false;
    [SerializeField]
    public bool OnlyPopUp = false;
    [SerializeField]
    public GameObject[] Buttons;
    [SerializeField]
    public GameObject[] ModifiableTexts;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
