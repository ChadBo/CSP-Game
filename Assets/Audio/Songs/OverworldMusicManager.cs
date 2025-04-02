using UnityEngine;

public class OverworldMusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstSong;
    public AudioClip loopSong;

    private bool hasSwitched = false;

    void Start()
    {
        audioSource.clip = firstSong;
        audioSource.Play();
    }

    void Update()
    {
        if (!audioSource.isPlaying && !hasSwitched)
        {
            audioSource.clip = loopSong;
            audioSource.loop = true;
            audioSource.Play();
            hasSwitched = true;
        }
    }
}
