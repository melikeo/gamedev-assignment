using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //variables for AudioSource Input Fields and AudioClip Input fields
    public AudioSource introMusicSource;
    public AudioSource backgroundMusicSource;
    public AudioClip introMusicClip;
    public AudioClip backgroundMusicClip;

    private void Start()
    {
        introMusicSource.clip = introMusicClip;
        backgroundMusicSource.clip = backgroundMusicClip;
                
        introMusicSource.Play(); //start playing intro music
                
        Invoke("PlayBackgroundMusic", introMusicSource.clip.length); //switch to background music after intro
    }

    private void PlayBackgroundMusic()
    {
        introMusicSource.Stop();
        backgroundMusicSource.Play();
    }
}
