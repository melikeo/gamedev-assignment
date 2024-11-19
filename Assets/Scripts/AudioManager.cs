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

    }

    private void Update()
    {
        // change music according to other (not dead) ghosts
        bool isAnyGhostDead = CheckIfGhostsState("Dead");
        bool isAnyGhostScared = CheckIfGhostsState("Scared") || CheckIfGhostsState("Recovering");

        if (isAnyGhostDead)
            {
                PlayDeadGhostsMusic();
            }


        else if (isAnyGhostScared)
        {
            PlayScaredGhostsMusic();
        }

        else
        {
            PlayBackgroundMusic();
        }
    }

    private void PlayBackgroundMusic()
    {
        //introMusicSource.Stop();
        //backgroundMusicSource.Play();

        if (ScaredMusicIsPlaying || DeadMusicIsPlaying || backgroundMusicSource.clip != backgroundMusicClip)
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
        if (!ScaredMusicIsPlaying || backgroundMusicSource.clip != scaredGhostsBackgroundMusic)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.clip = scaredGhostsBackgroundMusic;
            backgroundMusicSource.Play();
            ScaredMusicIsPlaying = true;
            DeadMusicIsPlaying = false;
        }
    }

    private void PlayDeadGhostsMusic()
    {
        if (!DeadMusicIsPlaying || backgroundMusicSource.clip != deadGhostsBackgroundMusicClip)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.clip = deadGhostsBackgroundMusicClip;
            backgroundMusicSource.Play();
            DeadMusicIsPlaying = true;
            ScaredMusicIsPlaying = false;
        }
    }

    private bool CheckIfGhostsState(string stateName)
    {
        foreach (Animator animator in ghostAnimators)
        {
            if (animator.GetBool(stateName))
            {
                //Debug.Log("is in state: " + stateName);
                return true; // true if any of the ghosts is in mentioned state
            }
        }
        return false;
    }
}
