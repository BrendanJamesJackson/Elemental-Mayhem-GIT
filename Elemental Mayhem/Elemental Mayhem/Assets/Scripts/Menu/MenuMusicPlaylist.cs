using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicPlaylist : MonoBehaviour
{
    [SerializeField] private AudioClip[] playlist;

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (playlist.Length == 0)
        {
            Debug.LogWarning("No music clips assigned to playlist.");
            return;
        }

        PlayTrack(currentTrackIndex);
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    private void PlayTrack(int index)
    {
        audioSource.clip = playlist[index];
        audioSource.Play();
    }

    private void PlayNextTrack()
    {
        currentTrackIndex++;

        if (currentTrackIndex >= playlist.Length)
        {
            currentTrackIndex = 0;
        }

        PlayTrack(currentTrackIndex);
    }
}