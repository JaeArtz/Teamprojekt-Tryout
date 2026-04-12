using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField, Tooltip("")]
    private AudioSource audioSource;

    [SerializeField, Tooltip("")]
    private AudioClip[] audioClips;

    [SerializeField, Tooltip("")]
    private string audioName;
    public string Name => audioName;

    [SerializeField, Range(0f, 5f)]
    private float volume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayRandomSound()
    {
        if (audioClips == null || audioClips.Length == 0 || !audioSource) return;
        int index = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[index], volume);
    }

    public void PlayRandomSoundAtSpot(Vector2 pos)
    {
        if (audioClips == null || audioClips.Length == 0) return;
        int index = Random.Range(0, audioClips.Length);
        AudioSource.PlayClipAtPoint(audioClips[index], pos, volume);
    }
}
