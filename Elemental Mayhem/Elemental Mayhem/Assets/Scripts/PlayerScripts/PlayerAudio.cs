using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip[] audioClips;
    public Vector2[] clipPitchRanges; 
    public AudioSource audioSource;

    public void PlayAudioClip(int clipIndex)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = audioClips[clipIndex];
        audioSource.pitch = Random.Range(clipPitchRanges[clipIndex].x, clipPitchRanges[clipIndex].y);
        audioSource.Play();

    }



}
