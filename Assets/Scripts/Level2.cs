using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level2 : MonoBehaviour
{

    public GameObject[] ghosts; // Array of the new ghosts that will be activated every interval seconds

    private float timeInterval = 20f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ActivateNewGhost());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ActivateNewGhost()
    {
        foreach (GameObject ghost in ghosts)
        {
            yield return new WaitForSeconds(timeInterval); // activate every interval seconds (20 default)
            ghost.SetActive(true); // activate ghost gameobjects of array
        }
    }
}
