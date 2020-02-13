using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Makes sounds on click or select
[RequireComponent(typeof(Toggle), typeof(AudioSource))]
public class ToggleClickAudio : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    // Editor references
    public AudioClip ClickSound;
    public AudioClip SelectSound;

    // GameObject references
    private Toggle toggle;
    private AudioSource audioSource;


    // Start is called before the first frame update
    private void Start()
    {
        this.toggle = this.gameObject.GetComponent<Toggle>();
        this.audioSource = this.gameObject.GetComponent<AudioSource>();
        this.audioSource.clip = this.SelectSound;
        this.audioSource.playOnAwake = false;
        this.toggle.onValueChanged.AddListener(this.PlayClickSound);
    }

    // On pointer enter
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.PlaySelectSound();
    }

    // On select
    public void OnSelect(BaseEventData eventData)
    {
        this.PlaySelectSound();
    }

    // Play click sound
    public void PlayClickSound(bool isClick)
    {
        this.audioSource.PlayOneShot(this.ClickSound);
    }

    // Play select sound
    public void PlaySelectSound()
    {
        this.audioSource.PlayOneShot(this.SelectSound);
    }
}
