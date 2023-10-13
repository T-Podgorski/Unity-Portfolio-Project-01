using UnityEngine;

public class CharacterAudioManager : MonoBehaviour
{
    private AudioSource audioSource;


    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRollSFX()
    {
        audioSource.PlayOneShot( GlobalAudioManager.instance.rollSFX );
    }
}
