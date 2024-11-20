using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level2 : MonoBehaviour
{
    public GameObject ghost3Prefab; //reference to prefab
    //public CountdownManager countdownManager; // reference to CountdownManager

    private float spawnInterval = 10; // create a new random walking ghosts every 10 seconds


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateNewGhosts());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CreateNewGhosts()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval); // create new ghosts every x seconds

            GameObject newGhostInstance = Instantiate(ghost3Prefab, new Vector3(-5.5f, -5, 0), Quaternion.identity); // spawn at Ghost 3 position

            //countdownManager.newGhost = newGhostInstance;

            //if (countdownManager != null)
            //{
            //    countdownManager.newGhost = newGhostInstance;
            //}

            GhostController ghostController = newGhostInstance.GetComponent<GhostController>(); // get ghost controller

            if (ghostController != null)
            {
                ghostController.topLeftTilemap = FindTilemap("TopLeft");
                ghostController.topRightTilemap = FindTilemap("TopRight");
                ghostController.bottomLeftTilemap = FindTilemap("BottomLeft");
                ghostController.bottomRightTilemap = FindTilemap("BottomRight");

                Debug.Log("Tilemaps referencing worked!");
            }

            Debug.Log("code running");
        }
    }

    Tilemap FindTilemap(string name)
    {
        GameObject tilemapObject = GameObject.Find(name); // search for tilemap
        if (tilemapObject != null)
        {
            return tilemapObject.GetComponent<Tilemap>(); // returns Tilemap Object
        }
        //Debug.LogWarning($"Tilemap '{name}' not found!!");
        return null; // found nothing
    }


}
