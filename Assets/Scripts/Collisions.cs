using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Collisions : MonoBehaviour
{
    private int score = 0; //init score
    public TMP_Text scoreText; // UI text field for score

    private bool ghostIsScared;
    public TMP_Text ghostTimerText;
    private float scaredTimer; //timer for 10 seconds
    private bool ghostIsRecovering;

    private float ghostDiesTimer; // Timer for 5 seconds

    public Animator[] ghostAnimators; // to include every ghost animator
    public Animator pacStudentAnimator; // PacStudent animator

    private int currentLives = 3; //inital number of lives 3
    public GameObject Life1, Life2, Life3; //input 3 life gameobjects (hearts)

    //PacStudent Controller (for movement)
    public PacStudentController pacStudent;

    private CountdownManager countdownManager;
    public TMP_Text gameOverText;

    // Pacstudent Death Particle Effect
    public ParticleSystem pacstudentDeathEffect;
    private ParticleSystem pacstudentDeathEffectInstance;

    // Pacstudent Death Sound Effect
    public AudioSource deathSoundEffect;
    [SerializeField] AudioClip pacstudentDeathSoundEffect;

    // Pacstudent avoid multiple deaths
    private bool pacstudentDyingOrRespawning = false; // so pacstudent does not lose multiple lives at once when immediately multiple ghost collisions happen

    private void Awake()
    {
        pacstudentDeathEffectInstance = Instantiate(pacstudentDeathEffect, transform.position, Quaternion.identity); //instantiate wall collision effect
        pacstudentDeathEffectInstance.transform.SetParent(transform); //set wall collision effect as child of pacstudent to place it at pacstudent
        pacstudentDeathEffectInstance.Stop(); //stop so it does not play automatically
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText();
        ghostTimerText.gameObject.SetActive(false);

        foreach (var animator in ghostAnimators)
        {
            animator.SetBool("walkingUp", true); // setting initial state to true
        }

        countdownManager = Object.FindFirstObjectByType<CountdownManager>(); // find CountdownManager.cs file
        gameOverText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (ghostIsRecovering)
        {
            //Debug.Log("Ghost is in recovering state");
        }


        if (ghostIsScared)
        {
            StartGhostScaredTimer();
        }

        if (currentLives<=0 || NoRemainingPellets()) // Game Over if no lives or pellets left
        {
            GameOver();
        }

        //if (ghostIsRecovering || ghostIsDying) // to keep the state
        //{            
        //    return;
        //}

    }

    bool NoRemainingPellets()
    {
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet"); // array with all pellets
        GameObject[] powerPellets = GameObject.FindGameObjectsWithTag("PowerPellet");
                
        //if ( pellets.Length == 1) { Debug.Log($"Last Pellet found: {pellets[0].name} at: {pellets[0].transform.position}"); }

        // true if no pellets (including powerpellets) are left
        return pellets.Length == 0 && powerPellets.Length == 0;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pacstudentDyingOrRespawning)
        {
            return; // pacstudent should not have other collisions while dying/respawing
        }

        if (collision.gameObject.CompareTag("Pellet"))
        {
            //Debug.Log("Pellet passed!");
            score += 10;
            Destroy(collision.gameObject);
            UpdateScoreText();
        }

        if (collision.gameObject.CompareTag("BonusCherry"))
        {
            //Debug.Log("BonusCherry passed!");
            score += 100;
            Destroy(collision.gameObject);
            UpdateScoreText();
        }

        if (collision.gameObject.CompareTag("PowerPellet"))
        {
            //Debug.Log("PowerPellet passed!");
            //score += 50; //assumption: points should be added, points based on original pacman game
            Destroy(collision.gameObject);
            UpdateScoreText();

            //if (ghostIsRecovering) // if ghost is in recovering state because a powerpellet was already eaten before
            //{
            //    foreach (var animator in ghostAnimators)
            //    {
            //        animator.SetBool("Recovering", false);
            //    }
            //    ghostIsRecovering = false;
            //}

            StartScaredState(); // A - Pacstudent eats powerpellet -> Ghost scared & recovering state
        }

        // PACSTUDENT & GHOSTS COLLISIONS

        if(collision.gameObject.CompareTag("Ghost")) {
            Animator ghostAnimator = collision.GetComponent<Animator>();
            if(ghostAnimator != null)
            {
                // ------------ C - if ghost is scared/recovering & collision with pacstudent -> ghost death -----------
                if ( ghostIsScared || ghostIsRecovering ) 
                {
                    StartCoroutine(GhostDeathReaction(ghostAnimator));
                }

                // ------------ B - if ghost is in walking state & collision with pacstudent -> pacstudent death -----------
                else if (IsGhostWalking(ghostAnimator) && (!ghostIsScared) && (!ghostIsRecovering))
                {
                    StartCoroutine(PacStudentDeathReaction());
                }
            }
        }

        else 
        { 
            //Debug.Log("no collision");
        }
    }


    // Ghost is walking -> Collision -> Pacstudent dies
    IEnumerator PacStudentDeathReaction()
    {
        //Debug.Log("pacstudent died");

        pacstudentDyingOrRespawning = true; // pacstudent cannot die during death state when already died

        pacStudentAnimator.SetBool("isDead", true);

        deathSoundEffect.clip = pacstudentDeathSoundEffect;
        deathSoundEffect.Play();

        pacstudentDeathEffectInstance.Play();

        yield return new WaitForSeconds(1.0f); //wait until animation is played / can change number for longer animation

        Vector3 restartPos = new Vector3(-18.4f, 7.4f, 0);
        transform.position = restartPos; // respawn at start position

        pacStudentAnimator.SetBool("isDead", false);
        pacstudentDeathEffectInstance.Stop();


        currentLives -= 1;
        UpdateHeartsUI(); //reduce number of hearts on Game Screen


        //respawn should only happen after animation has been played //wait!

        //Vector3 restartPos = new Vector3(-18.4f, 7.4f, 0);
        //transform.position = restartPos; // respawn at start position

        //Debug.Log("did dead animation play?");

        pacstudentDyingOrRespawning = false;


        if (currentLives > 0)
        {
            //Debug.Log("weiter gehts");            
        }

        else if (currentLives <= 0)
        {
            //Debug.Log("game over");
        }


    }

    void UpdateHeartsUI() //reduce number of hearts at collisions
    {
        if (currentLives == 2)
        {
            Life3.SetActive(false);
        }
        else if (currentLives == 1)
        {
            Life2.SetActive(false);
        }
            
        else if (currentLives == 0)
        {
            Life1.SetActive(false);
            //GameOver();
        }            
    }

    IEnumerator GhostDeathReaction(Animator ghostAnimator)
    {
        //ghostAnimator.SetTrigger("TriggerDead"); //transition to dead state
        //Debug.Log("Ghosts is in dead state.");

        ghostAnimator.SetBool("Scared", false);
        ghostAnimator.SetBool("Recovering", false);
        ghostAnimator.SetBool("Dead", true);

        score += 300; //add 300 points to score
        UpdateScoreText(); //update highscore

        yield return new WaitForSeconds(5.0f); // wait for 5 seconds

        //transition back to walking state (reset state)
        ghostAnimator.SetBool("Dead", false);
        //Debug.Log("Ghosts is in walking state.");
    }

    //void StartGhostDiesTimer()
    //{
    //    ghostDiesTimer = 5.0f;
    //    //ghostDiesTimer -= Time.deltaTime;

    //    //after 5 seconds passed
    //    if (scaredTimer <= 0)
    //    {
    //        foreach (var animator in ghostAnimators) //transition back to walking state (reset state)
    //        {
    //            animator.ResetTrigger("TriggerDead");
    //            Debug.Log("Ghosts going into walking state.");
    //        }

    //    }
    //}

    private bool IsGhostWalking(Animator ghostAnimator)
    {
        return ghostAnimator.GetBool("walkingLeft") ||
               ghostAnimator.GetBool("walkingRight") ||
               ghostAnimator.GetBool("walkingUp") ||
               ghostAnimator.GetBool("walkingDown");
    }

    void UpdateScoreText() //update high score
    {
        scoreText.text = score.ToString();
    }


    void StartScaredState()
    {
        //if (ghostIsScared)
        //{
        //    //restart timer if ghost is already scared and new collision
        //    scaredTimer = 10.0f;
        //    ghostTimerText.text = Mathf.Ceil(scaredTimer).ToString();            
        //    return; //exit because ghost is already scared
        //}


        //ghostIsScared = true;
        scaredTimer = 10.0f;
        ghostTimerText.gameObject.SetActive(true);
        ghostTimerText.text = Mathf.Ceil(scaredTimer).ToString();

        //set ghost animator state to "scared" (all ghosts)
        foreach (var animator in ghostAnimators)
        {            
            animator.SetBool("Scared", true);
            animator.SetBool("Recovering", false);
            animator.SetBool("Dead", false);
        }
        ghostIsScared = true;
        ghostIsRecovering = false;
        //Debug.Log("Ghosts are scared right now.");
    }

    void StartGhostScaredTimer()
    {
        if (!ghostIsScared) return; //only change state if ghost is in scared

        scaredTimer -= Time.deltaTime;
        ghostTimerText.text = Mathf.Ceil(scaredTimer).ToString();

        //3 seconds left on timer -> ghosts go in recovering state
        if (scaredTimer <= 3.0f && scaredTimer > 0)
        {
            if (!ghostIsRecovering)
            {
                foreach (var animator in ghostAnimators)
                {
                    animator.SetBool("Scared", false);
                    animator.SetBool("Recovering", true);
                    //Debug.Log("Ghosts going into recovering state.");
                }
                ghostIsRecovering = true;
                //Debug.Log("3 seconds if is left");

            }
        }
        //after 10 seconds passed
        else if (scaredTimer <= 0)
        {
            //Debug.Log("10 seconds passed is entered");
            EndGhostScaredState();
        }
    }

    void EndGhostScaredState()
    {
        ghostIsScared = false; //reset state
        ghostIsRecovering = false;
        ghostTimerText.gameObject.SetActive(false); //hide timer
        foreach (var animator in ghostAnimators)
        {
            //animator.SetBool("walkingUp", true);
            animator.SetBool("Recovering", false);
            animator.SetBool("Scared", false);
        }
        //Debug.Log("Ghosts are in walking state now.");
    }

    //saving Highscore after GameOver (all pellets eaten or no lives left)
    void GameOver()
    {
        //Debug.Log("Game is over.");
        // Stop Game
        pacStudent.GetComponent<PacStudentController>().enabled = false; //stop player movement
        foreach (var animator in ghostAnimators) // stop ghosts
        {
            animator.enabled = false; //TBA
        }

        //pause Game Timer
        countdownManager.StopTimer();
        
        // Save Highscore
        SaveHighscore();

        // Show Game Over Text
        gameOverText.gameObject.SetActive(true);

        // Return to Startscene (with updated Highscore)
        StartCoroutine(ReturnToStartScene());

    }

    void SaveHighscore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // load saved highscore (if nothing saved use 0)
        float bestTime = PlayerPrefs.GetFloat("HighScoreTime", float.MaxValue); // load highscore time (if nothing saved load highscore max)

        if (score > highScore || (score == highScore && countdownManager.elapsedTime < bestTime))
        {
            PlayerPrefs.SetInt("HighScore", score); // save new highscore
            PlayerPrefs.SetFloat("HighScoreTime", countdownManager.elapsedTime); // save new best time
            //Debug.Log("New highscore: " + score + " time: " + countdownManager.elapsedTime);
        }
    }
    IEnumerator ReturnToStartScene()
    {
        yield return new WaitForSeconds(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
        gameOverText.gameObject.SetActive(false);
    }
}