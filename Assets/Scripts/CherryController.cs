using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;

    private Vector3 spawnPosition; //where cherry will spawn (starting point)
    private Vector3 targetPosition; //where cherry will move to

    public float spawnDurationTime = 5.0f;
    public float repeatRate = 10.0f;

    public float moveSpeed = 3f;

    Vector3 cameraCenter = new Vector3(-6, -6, 0); //take into account that camera center is not at 0,0,0


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

        int side = Random.Range(0, 4); //choose value between 0 and 3 for side to spawn from

        switch (side)
        {
            case 0: return new Vector3(Random.Range(-cameraWidth, cameraWidth), cameraHeight + 1, 0);  // above screen //top
            case 1: return new Vector3(Random.Range(-cameraWidth, cameraWidth), -cameraHeight - 1, 0); // below screen //bottom
            case 2: return new Vector3(-cameraWidth - 1, Random.Range(-cameraHeight, cameraHeight), 0); // left from screen //left
            case 3: return new Vector3(cameraWidth + 1, Random.Range(-cameraHeight, cameraHeight), 0); // right from screen //right
            default: return Vector3.zero;
        }
    }


    void SpawnCherry()
    {
        //create random location for cherry outside of camera
        spawnPosition = GetRandomSpawnPosition();

        //instantiate cherry at a random location
        GameObject cherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);

        // set target point for cherry movement (through camera center)
        targetPosition = -spawnPosition;  // on the other side
        StartCoroutine(MoveCherry(cherry)); // start movement with movement coroutine
    }

    IEnumerator MoveCherry(GameObject cherry) //to move cherry across the map
    {
        // move cherry to targetPosition
        while (Vector3.Distance(cherry.transform.position, targetPosition) > 0.1f)
        {
            cherry.transform.position = Vector3.MoveTowards(cherry.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // destroy cherry gameObject after target is reached
        Destroy(cherry);
    }

}
