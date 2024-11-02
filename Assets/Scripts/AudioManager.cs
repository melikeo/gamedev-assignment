using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //variables for AudioSource Input Fields and AudioClip Input fields
    public AudioSource introMusicSource;
    public AudioSource backgroundMusicSource;

    public AudioClip introMusicClip;
    public AudioClip backgroundMusicClip;
    public AudioClip scaredGhostsBackgroundMusic;
    public AudioClip deadGhostsBackgroundMusicClip;

    public Animator[] ghostAnimators;

    private bool ScaredMusicIsPlaying = false;
    private bool DeadMusicIsPlaying = false;

    private void Start()
    {
        //introMusicSource.clip = introMusicClip;


        backgroundMusicSource.clip = backgroundMusicClip;
        //backgroundMusicSource.Play();

        //introMusicSource.Play(); //start playing intro music

        //Invoke("PlayBackgroundMusic", introMusicSource.clip.length); //switch to background music after intro



    }

    private void Update()
    {
        if (CheckIfGhostsState("Dead"))
            {
                PlayDeadGhostsMusic();
            }


        else if (CheckIfGhostsState("Scared") || CheckIfGhostsState("Recovering"))
        {
            PlayScaredGhostsMusic();
        }

        //else if (CheckIfGhostsState("Dead") && (CheckIfGhostsState("Scared") || CheckIfGhostsState("Recovering")))
        //{
        //    PlayDeadGhostsMusic();
        //}


        else
        {
            PlayBackgroundMusic();
        }
    }


    private void PlayBackgroundMusic()
    {
        //introMusicSource.Stop();
        //backgroundMusicSource.Play();

        if (ScaredMusicIsPlaying || DeadMusicIsPlaying)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.Play();
            ScaredMusicIsPlaying = false;
            DeadMusicIsPlaying = false;
        }
    }


    private void PlayScaredGhostsMusic()
    {
        if (!ScaredMusicIsPlaying)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.clip = scaredGhostsBackgroundMusic;
            backgroundMusicSource.Play();
            ScaredMusicIsPlaying = true;
            //DeadMusicIsPlaying = false;
        }
    }

    private void PlayDeadGhostsMusic()
    {
        if (!DeadMusicIsPlaying)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.clip = deadGhostsBackgroundMusicClip;
            backgroundMusicSource.Play();
            DeadMusicIsPlaying = true;
            //ScaredMusicIsPlaying = false;
        }
    }


    private bool CheckIfGhostsState(string stateName)
    {
        foreach (Animator animator in ghostAnimators)
        {
            if (animator.GetBool(stateName))
            {
                Debug.Log("is in state: " + stateName);
                return true; // true if any of the ghosts is in mentioned state
            }
        }
        return false;
    }

    //bool checkForScared() 
    //{
    //    foreach (Animator animator in ghostAnimators)
    //    {
    //        if (animator.GetBool("Scared"))
    //        {
    //            Debug.Log("is in state: scared");
    //            return true; // true if any of the ghosts is in mentioned state
    //        }
    //    }
    //    return false;
    //}



}
