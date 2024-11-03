using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;

    public float spawnDurationTime = 1f; // First cherry appears after 1 second
    public float repeatRate = 10.0f; // Time between spawns: every 10 seconds a cherry spawns
    public float moveSpeed = 7f; // Default speed, can be changed in inspector

    Vector3 cameraCenter = new Vector3(-6, -6, -1); // Taking into account that camera center is not at 0,0,0

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnCherry", spawnDurationTime, repeatRate);
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Get camera sizes
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        int side = Random.Range(0, 4); // choose value between 0 and 3 for random side to spawn from

        switch (side) //additional 10 to be out of camera view
        {
            case 0: return new Vector3(Random.Range(-cameraWidth, cameraWidth), cameraHeight + 10, -1);  // Above screen // Top
            case 1: return new Vector3(Random.Range(-cameraWidth, cameraWidth), -cameraHeight - 10, -1); // Below screen // Bottom
            case 2: return new Vector3(-cameraWidth - 10, Random.Range(-cameraHeight, cameraHeight), -1); // Left from screen // Left
            case 3: return new Vector3(cameraWidth + 10, Random.Range(-cameraHeight, cameraHeight), -1); // Right from screen // Right
            default: return Vector3.zero;
        }
    }

    void SpawnCherry()
    {
        // Create random location for cherry outside of camera
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        GameObject cherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity); // Instantiate cherry at a random location
        StartCoroutine(MoveCherry(cherry, spawnPosition)); // Start movement with coroutine
        //Debug.Log("Cherry spawned at: " + spawnPosition);
    }

    IEnumerator MoveCherry(GameObject cherry, Vector3 spawnPosition) // Move cherry across the map in 2 parts
    {
        if (cherry == null) yield break;
        Vector3 startPos = spawnPosition;
        Vector3 targetPos = cameraCenter; //first part: move to center

        float t = 0; //t value for lerp

        // Move cherry to center
        while( t < 1.0f )
        {
            if (cherry == null) yield break;

            t += Time.deltaTime * (moveSpeed/Vector3.Distance(startPos, targetPos));
            //cherry.transform.position = Vector3.Lerp(cherry.transform.position, cameraCenter, t); //LERP to center
            cherry.transform.position = Vector3.Lerp(startPos, targetPos, t); // LERP to center
            yield return null;
        }

        //second part: move to target position
        startPos= cameraCenter;
        targetPos = GetTargetPosition(spawnPosition);

        t = 0;

        while ( t < 1.0f )
        {
            if (cherry == null) yield break;

            t += Time.deltaTime * (moveSpeed / Vector3.Distance(startPos, targetPos));
            cherry.transform.position = Vector3.Lerp(startPos, targetPos, t); // LERP to target at diagonal
            yield return null;
        }

        Destroy(cherry); // Destroy cherry GameObject after reaching the target position
        //Debug.Log("Cherry reached target and destroyed at position: " + cherry.transform.position);

        //while (Vector3.Distance(cherry.transform.position, cameraCenter) > 0.1f)
        //{

        //    yield return null; // wait for next frame
        //}

        //Vector3 targetPosition = GetTargetPosition(spawnPosition);

        //// Move cherry to target position
        //while (Vector3.Distance(cherry.transform.position, targetPosition) > 0.1f)
        //{
        //    t += moveSpeed * Time.deltaTime;

        //    cherry.transform.position = Vector3.Lerp(cherry.transform.position, targetPosition, t);
        //    yield return null;
        //}

        // Check if cherry has reached the target position
        //if (HasReachedTarget(cherry.transform.position, targetPosition)){}                
    }

    // Calculate the target position for cherry
    Vector3 GetTargetPosition(Vector3 startPosition)
    {
        // Calculate the opposite side of the start position, through the center (-6,-6,0)
        float targetX = 2 * cameraCenter.x - startPosition.x; // Get x position for target position
        float targetY = 2 * cameraCenter.y - startPosition.y; // Get y position for target position
        return new Vector3(targetX, targetY, -1);
    }

    // Check if cherry has reached the target position
    //bool HasReachedTarget(Vector3 currentPosition, Vector3 targetPosition)
    //{
    //    return Vector3.Distance(currentPosition, targetPosition) <= 0.1f;
    //}
}
