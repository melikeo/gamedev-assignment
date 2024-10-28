using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;

    //private Vector3 spawnPosition; //where cherry will spawn (starting point)
    private Vector3 targetPosition; //where cherry will move to

    public float spawnDurationTime = 10.0f; //first cherry appears after 10 second
    public float repeatRate = 10.0f;

    public float moveSpeed = 7f; // default speed, can be changed in inspector

    Vector3 cameraCenter = new Vector3(-6, -6, 0); //taking into account that camera center is not at 0,0,0


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnCherry", spawnDurationTime, repeatRate);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 GetRandomSpawnPosition()
    {
        //get camera sizes
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        int side = Random.Range(0, 4); //choose value between 0 and 3 for random side to spawn from

        switch (side)
        {
            case 0: return new Vector3(Random.Range(-cameraWidth, cameraWidth), cameraHeight + 1, -1);  // above screen //top
            case 1: return new Vector3(Random.Range(-cameraWidth, cameraWidth), -cameraHeight - 1, -1); // below screen //bottom
            case 2: return new Vector3(-cameraWidth - 1, Random.Range(-cameraHeight, cameraHeight), -1); // left from screen //left
            case 3: return new Vector3(cameraWidth + 1, Random.Range(-cameraHeight, cameraHeight), -1); // right from screen //right
            default: return Vector3.zero;
        }
    }


    void SpawnCherry()
    {
        //create random location for cherry outside of camera
        Vector3 spawnPosition = GetRandomSpawnPosition();

        //instantiate cherry at a random location
        GameObject cherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);

        // set target point for cherry movement (through camera center)
        //targetPosition = GetTargetPosition(spawnPosition);   // on the other side
        StartCoroutine(MoveCherry(cherry, spawnPosition)); // start movement with movement coroutine
        //Debug.Log("Cherry spawned at: " + Time.time); // time of spawn
    }

    IEnumerator MoveCherry(GameObject cherry, Vector3 spawnPosition) //to move cherry across the map (in 2 parts: first to center than to other side)
    {
        // move cherry to center
        while (Vector3.Distance(cherry.transform.position, cameraCenter) > 0.1f)
        {
            cherry.transform.position = Vector3.MoveTowards(cherry.transform.position, cameraCenter, moveSpeed * Time.deltaTime);
            yield return null;
        }
        Vector3 targetPosition = GetTargetPosition(spawnPosition);
        // Move cherry to targetPosition
        while (Vector3.Distance(cherry.transform.position, targetPosition) > 0.1f)
        {
            cherry.transform.position = Vector3.MoveTowards(cherry.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // destroy cherry gameObject after target is reached
        Destroy(cherry);
    }

    // so that cherry goes through center
    Vector3 GetTargetPosition (Vector3 startPosition)
    {
        //Vector3 cameraCenter = new Vector3(-6, -6, 0); // adjust according to camera position

        // Calculate the opposite side of the start position, through the center (-6,-6,0)
        float targetX = 2 * cameraCenter.x - startPosition.x; //gets x position for target pos
        float targetY = 2 * cameraCenter.y - startPosition.y;  //gets y position for target pos
        return new Vector3(targetX, targetY, 0);        
    }
}
