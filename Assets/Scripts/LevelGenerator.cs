using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject outsideCorner;  // 1-outside corner
    public GameObject outsideWall;    // 2-outside wall
    public GameObject insideCorner; // 3-inside corner
    public GameObject insideWall; // 4-inside wall
    public GameObject pellet;      // 5-standard pellet
    public GameObject powerPellet; // 6-power pellet
    public GameObject tJunction; // 7-t junction piece

    int[,] levelMap = { //should be replaceable
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    void Start()
    {
        BuildLevel();
    }

    void BuildLevel()
    {
        float spriteSize = 1.0f; // size of sprites (1 is typical)

        int width = levelMap.GetLength(1); //number of columns
        int height = levelMap.GetLength(0); //number of rows

        for (int y = 0; y < levelMap.GetLength(0); y++) //iterate through rows
        {
            for (int x = 0; x < levelMap.GetLength(1); x++) //iterate through columns
            {
                Vector2 position = new Vector2(x * spriteSize, -y * spriteSize);

                int tileType = levelMap[y, x];

                // checking value and placing sprite

                //switch(tileType)
                //{
                //    case 1:
                //        Instantiate(outsideCorner, position, Quaternion.identity);
                //        break;
                //    case 2:
                //        Instantiate(outsideWall, position, Quaternion.identity);
                //        break;
                //    case 3:
                //        Instantiate(insideCorner, position, Quaternion.identity);
                //        break;
                //    case 4:
                //        Instantiate(insideWall, position, Quaternion.identity);
                //        break;
                //    case 5:
                //        Instantiate(pellet, position, Quaternion.identity);
                //        break;
                //    case 6:
                //        Instantiate(powerPellet, position, Quaternion.identity);
                //        break;
                //    case 7:
                //        Instantiate(tJunction, position, Quaternion.identity);
                //        break;
                //    default:
                //        break; //empty fields

                //}

                if (levelMap[y, x] == 1) // 1 - outside corner
                {
                    Instantiate(outsideCorner, position, Quaternion.identity);
                }
                else if (levelMap[y, x] == 2) // 2 - outside wall
                {
                    Instantiate(outsideWall, position, Quaternion.identity);
                }
                else if (levelMap[y, x] == 3) // 3 - inside corner
                {
                    Instantiate(insideCorner, position, Quaternion.identity);
                }
                else if (levelMap[y, x] == 4) // 4- inside wall
                {
                    Instantiate(insideWall, position, Quaternion.identity);
                }
                else if (levelMap[y, x] == 5) // 5- standard pellet
                {
                    Instantiate(pellet, position, Quaternion.identity);
                }
                else if (levelMap[y, x] == 6) // powerpellet
                {
                    Instantiate(powerPellet, position, Quaternion.identity);
                }
                else if (levelMap[y, x] == 7) //t junction piece
                {
                    Instantiate(tJunction, position, Quaternion.identity);
                }
            }
        }
    }
}
