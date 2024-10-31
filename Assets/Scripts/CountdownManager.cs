using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public TMP_Text countdownText; // text field for countdown text (3,2,1,go)
    public GameObject pacStudent; // Pacstudent
    public GameObject[] ghosts; // array for ghost objects
    public AudioManager audioManager; // referring to AudioManager object

    private void Start()
    {
        countdownText.gameObject.SetActive(false); // start with deactivating countdown
        StartCoroutine(ShowCountdown());
    }

    private IEnumerator ShowCountdown()
    {        
        DisableMovement(); //disable pacstudent and ghost movement while countdown is shown

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
        pacStudent.GetComponent<PacStudentController>().enabled = false; // deactivate
        foreach (var ghost in ghosts)
        {
            //ghost.GetComponent<GhostMovement>().enabled = false; // TBA for ghost movement
        }
    }

    private void EnableMovement()
    {
        pacStudent.GetComponent<PacStudentController>().enabled = true; // Spielerbewegung aktivieren
        foreach (var ghost in ghosts)
        {
            //ghost.GetComponent<GhostMovement>().enabled = true; // Geisterbewegung aktivieren
        }
    }

    private void PlayBackgroundMusic()
    {
        if (audioManager != null && audioManager.backgroundMusicSource != null)
        {
            audioManager.backgroundMusicSource.Play();
        }
    }
}
