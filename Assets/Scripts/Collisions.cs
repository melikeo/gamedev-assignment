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

    ////for pellet collision
    //// Tilemaps for each section of the map
    //[SerializeField] private Tilemap topLeftTilemap; // top-left section of the map
    //[SerializeField] private Tilemap topRightTilemap; // top-right section of the map
    //[SerializeField] private Tilemap bottomLeftTilemap; // bottom-left section of the map
    //[SerializeField] private Tilemap bottomRightTilemap; // bottom-right section of the map

    //public Tile pelletTile;



    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Pellet"))
        {
            Debug.Log("Pellet passed!");
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
            Debug.Log("PowerPellet passed!");
            score += 100;
            Destroy(collision.gameObject);
            UpdateScoreText();
        }


        else { Debug.Log("no collision"); }
    }

    void UpdateScoreText() //update high score
    {
        scoreText.text = "Score: " + score;
    }


}
