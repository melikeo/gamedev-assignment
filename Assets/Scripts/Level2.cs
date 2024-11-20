using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level2 : MonoBehaviour
{
    public GameObject ghost3Prefab; //reference to prefab


    // Start is called before the first frame update
    void Start()
    {
        GameObject newGhost = Instantiate(ghost3Prefab, new Vector3(-5.5f, -5, 0), Quaternion.identity); // spawn at Ghost 3 position

        GhostController ghostController = newGhost.GetComponent<GhostController>(); // get ghost controller

        if (ghostController != null)
        {
            ghostController.topLeftTilemap = FindTilemap("TopLeft");
            ghostController.topRightTilemap = FindTilemap("TopRight");
            ghostController.bottomLeftTilemap = FindTilemap("BottomLeft");
            ghostController.bottomRightTilemap = FindTilemap("BottomRight");

            Debug.Log("Tilemaps referencing worked!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Tilemap FindTilemap(string name)
    {
        GameObject tilemapObject = GameObject.Find(name); // search for tilemap
        if (tilemapObject != null)
        {
            return tilemapObject.GetComponent<Tilemap>(); // returns Tilemap Object
        }
        Debug.LogWarning($"Tilemap '{name}' not found!!");
        return null; // found nothing
    }


}
