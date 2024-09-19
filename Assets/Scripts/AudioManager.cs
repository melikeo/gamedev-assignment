using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip introMusic;
    public AudioClip ghostsNormalMusic;

    private void Start()
    {        
        StartCoroutine(PlayMusic());
    }

    IEnumerator PlayMusic()
    {        
        audioSource.clip = introMusic;
        audioSource.Play(); //play intro music
                
        yield return new WaitForSeconds(audioSource.clip.length); //waits for intro music to finish
                
        audioSource.clip = ghostsNormalMusic;
        audioSource.Play(); //plays normal state ghosts (normal background) music
    }
}
