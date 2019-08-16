using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Makes sounds on click or select
[RequireComponent(typeof(Button), typeof(AudioSource))]
public class ButtonClickAudio : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    // Editor references
    public AudioClip ClickSound;
    public AudioClip SelectSound;

    // GameObject references
    private Button button;
    private AudioSource audioSource;


    // Start is called before the first frame update
    private void Start()
    {
        this.button = this.gameObject.GetComponent<Button>();
        this.audioSource = this.gameObject.GetComponent<AudioSource>();
        this.audioSource.clip = this.SelectSound;
        this.audioSource.playOnAwake = false;
        this.button.onClick.AddListener(() => this.PlaySound(this.ClickSound));
    }

    // On pointer enter
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.PlaySound(this.SelectSound);
    }

    // On select
    public void OnSelect(BaseEventData eventData)
    {
        this.PlaySound(this.SelectSound);
    }

    // Play sound
    public void PlaySound(AudioClip sound)
    {
        this.audioSource.PlayOneShot(sound);
    }
}
