using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Collisions : MonoBehaviour
{
    private int score = 0; //init score
    public TMP_Text scoreText; // UI text field for score

    private bool GhostIsScared;
    public TMP_Text ghostTimerText;
    private float scaredTimer; //timer for 10 seconds

    public Animator[] ghostAnimators; // to include every ghost animator
    public Animator pacStudentAnimator; // PacStudent animator

    private int currentLives = 3; //inital number of lives 3
    public GameObject Life1, Life2, Life3; //input 3 life gameobjects (hearts)

    //PacStudent Controller (for movement)
    public PacStudentController pacStudent;


    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText();
        ghostTimerText.gameObject.SetActive(false);

        foreach (var animator in ghostAnimators)
        {
            animator.SetBool("walkingUp", true); // setting initial state to true
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(GhostIsScared)
        {
            StartGhostScaredTimer();
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

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
            StartScaredState();
        }

        // Ghosts - walking state
        // PacStudent collides with a Ghost in walking state
        if(collision.gameObject.CompareTag("Ghost")) {
            Animator ghostAnimator = collision.GetComponent<Animator>();
            if (ghostAnimator != null && IsGhostWalking(ghostAnimator)) //if ghost is in walking state & collision with pacstudent -> pacstudent death
            {
                PacStudentDeathReaction();
            }
        }


        else 
        { 
            //Debug.Log("no collision");
        }
    }

    private void PacStudentDeathReaction()
    {
        // TODO:
        //lose life
        // particle effect
        // respawn

        Debug.Log("pacstudent died");

        pacStudentAnimator.SetBool("isDead", true);
        //particle effekt
        currentLives -= 1;
        UpdateHeartsUI(); //reduce number of hearts on Game Screen
        
        //animation should play here
        //respawn should only happen after animation has been played //wait!


        Vector3 restartPos = new Vector3(-18.4f, 7.4f, 0);
        transform.position = restartPos; // respawn at start position
        Debug.Log("did dead animation play?");
        pacStudentAnimator.SetBool("isDead", false);


        if (currentLives > 0)
        {
            Debug.Log("weiter gehts");
            
        }

        else
        {
            Debug.Log("game over");
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
        }
            
    }


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
        GhostIsScared = true;
        scaredTimer = 10.0f;
        ghostTimerText.gameObject.SetActive(true);

        //set ghost animator state to "scared" (all ghosts)
        foreach (var animator in ghostAnimators)
        {
            animator.SetTrigger("TriggerScared");
        }
    }

    void StartGhostScaredTimer()
    {
        scaredTimer -= Time.deltaTime;
        ghostTimerText.text = Mathf.Ceil(scaredTimer).ToString();

        //3 seconds left on timer -> ghosts go in recovering state
        if (scaredTimer <= 3.0f && scaredTimer > 0)
        {
            foreach (var animator in ghostAnimators)
            {
                animator.SetTrigger("TriggerRecovering");
            }
        }
        //after 10 seconds passed
        else if (scaredTimer <= 0)
        {
            EndGhostScaredState();
        }
    }

    // ADD AUDIOOOO!!!!!
    void EndGhostScaredState()
    {
        GhostIsScared = false;
        ghostTimerText.gameObject.SetActive(false);
        foreach (var animator in ghostAnimators)
        {
            animator.SetBool("walkingUp", true);
        }
    }




}
