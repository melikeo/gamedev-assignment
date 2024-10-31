using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public TMP_Text countdownText; // text field for countdown text (3,2,1,go)
    public GameObject pacStudent; // Pacstudent
    public GameObject[] ghosts; // array for ghost objects
    public AudioManager audioManager; // referring to AudioManager object
    public TMP_Text gameTimer; //general game timer

    public float elapsedTime = 0f; // overall passed time in seconds
    private bool isGameActive = false;

    private void Start()
    {
        countdownText.gameObject.SetActive(false); // start with deactivating countdown
        StartCoroutine(ShowCountdown());
    }

    private void Update()
    {
        if (isGameActive)
        {
            elapsedTime += Time.deltaTime;
            UpdateGameTimer(); // update game timer
        }
    }

    private IEnumerator ShowCountdown() //Show 3,2,1,GO! countdown
    {        
        DisableMovement(); //disable pacstudent and ghost movement while countdown is shown

        //reset timer
        elapsedTime = 0;
        UpdateGameTimer();

        countdownText.gameObject.SetActive(true); // show countdown text box

        // countdown starting from 3 to 1
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1); // show countdown for 1 second
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1); // show text for 1 second

        // stop countdown and start game
        countdownText.gameObject.SetActive(false); // hide countdown
        EnableMovement(); // activate Pacstudent (and Ghosts) movement
        PlayBackgroundMusic(); // start background music
    }

    private void DisableMovement()
    {
        pacStudent.GetComponent<PacStudentController>().enabled = false; // deactivate pacstudent movement
        foreach (var ghost in ghosts)
        {
            //ghost.GetComponent<GhostMovement>().enabled = false; // TBA for ghost movement
        }
    }

    private void EnableMovement()
    {
        pacStudent.GetComponent<PacStudentController>().enabled = true; // activate pacstudent movement
        foreach (var ghost in ghosts)
        {
            //ghost.GetComponent<GhostMovement>().enabled = true; // activate ghost movement
        }
        isGameActive = true; //set game to active to start the Game Timer
    }

    private void PlayBackgroundMusic()
    {
        if (audioManager != null && audioManager.backgroundMusicSource != null)
        {
            audioManager.backgroundMusicSource.Play();
        }
    }

    private void UpdateGameTimer()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60); //converting to minutes
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime - Mathf.Floor(elapsedTime)) * 100);

        gameTimer.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); //display time
    }

    public void StopTimer()
    {
        isGameActive = false; //to reference to from collisions.cs to stop the timer
    }
}
