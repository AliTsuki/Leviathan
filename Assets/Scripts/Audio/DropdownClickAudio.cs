using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

// Makes sounds on click or select
[RequireComponent(typeof(TMP_Dropdown), typeof(AudioSource))]
public class DropdownClickAudio : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    // Editor references
    public AudioClip ClickSound;
    public AudioClip SelectSound;

    // GameObject references
    private TMP_Dropdown dropdown;
    private AudioSource audioSource;


    // Start is called before the first frame update
    private void Start()
    {
        this.dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
        this.audioSource = this.gameObject.GetComponent<AudioSource>();
        this.audioSource.clip = this.SelectSound;
        this.audioSource.playOnAwake = false;
        this.dropdown.onValueChanged.AddListener(this.PlayClickSound);
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
    public void PlayClickSound(int _value)
    {
        this.audioSource.PlayOneShot(this.ClickSound);
    }

    // Play select sound
    public void PlaySelectSound()
    {
        this.audioSource.PlayOneShot(this.SelectSound);
    }
}
