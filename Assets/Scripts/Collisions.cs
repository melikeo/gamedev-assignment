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


    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText();
        ghostTimerText.gameObject.SetActive(false);
        
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

        //if (collision.gameObject.CompareTag("BonusCherry")) //Need to fix the error when destroyed
        //{
        //    Debug.Log("BonusCherry passed!");
        //    score += 100;
        //    Destroy(collision.gameObject);
        //    UpdateScoreText();
        //}

        if (collision.gameObject.CompareTag("PowerPellet"))
        {
            //Debug.Log("PowerPellet passed!");
            score += 100;
            Destroy(collision.gameObject);
            UpdateScoreText();
            StartScaredState();
        }


        else 
        { 
            //Debug.Log("no collision");
        }
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
