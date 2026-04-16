using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("AUDIO SOURCE")]
    public AudioSource AudioPlayer;

    public AudioClip TapSound;
    public AudioClip SocketOccupiedSound;
    public AudioClip SpawnerDespawnSound;
    public AudioClip SpawnerDespawnSpinSound;
    public AudioClip ArrowHitSound;

    public void Play(AudioClip Clip)
    {
        AudioPlayer.PlayOneShot(Clip);
    }
}
