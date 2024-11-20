using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //highscore fields for playerprefs
    public TMP_Text highScoreText;
    public TMP_Text highScoreTimeText;


    // Start is called before the first frame update
    void Start()
    {
        LoadHighScore();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSecondLevel()
    {
        SceneManager.LoadScene("InnovationScene");
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadStartScreen()
    {
        SceneManager.LoadScene("StartScene");
    }

    //void SaveHighscore(int score, float timeElapsed)
    //{
    //    PlayerPrefs.SetInt("HighScore", score);
    //    PlayerPrefs.SetFloat("HighScoreTime", timeElapsed);
    //    PlayerPrefs.Save(); // save PlayerPrefs
    //}
    private void LoadHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // initial 0
        float highScoreTime = PlayerPrefs.GetFloat("HighScoreTime", 0); // initial 0.0f

        // update highscore
        highScoreText.text = highScore.ToString();
        highScoreTimeText.text = FormatTime(highScoreTime);
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); //format time to mm:ss:ms
    }


}
