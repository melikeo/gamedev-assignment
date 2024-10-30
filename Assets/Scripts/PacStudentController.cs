using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    private Vector3Int currentGridPosition; //current Pacstudent grid position
    private Vector3Int targetGridPosition;  //target Pacstudent position

    //start and end position for pacstudents movements
    private Vector3 startPos;
    private Vector3 targetPos;

    [SerializeField] private float pacstudentMoveSpeed = 5f; //default move speed

    private bool isMoving = false;

    //storing last key player pressed
    private KeyCode lastInput; //store last input
    private Animator animator;

    private KeyCode currentInput; //for walkable position from lastInput


    private float t = 0; //interpolation value for lerp

    private Vector3Int currentDirection; //stores current moving direction

    // Tilemaps for each section of the map
    [SerializeField] private Tilemap topLeftTilemap; // top-left section of the map
    [SerializeField] private Tilemap topRightTilemap; // top-right section of the map
    [SerializeField] private Tilemap bottomLeftTilemap; // bottom-left section of the map
    [SerializeField] private Tilemap bottomRightTilemap; // bottom-right section of the map

    //list of wall tiles that will be checked
    [SerializeField] private TileBase[] wallTiles; // array of wall tiles

    //add Dust Particle System Effect
    [SerializeField] private ParticleSystem dustParticleEffect;
    private ParticleSystem dustParticleInstance;
    bool inputReceived = false;

    //add audio
    public AudioSource audioSource;

    [SerializeField] AudioClip eatingPelletClip;
    [SerializeField] AudioClip movingNotEatingClip;
    [SerializeField] TileBase pelletTile; //to add pellet Tile to check
    bool fieldHasPellet = false;
    bool fieldHasPowerPellet = false;

    //----80%----
    //wall collision
    public ParticleSystem wallCollisionEffect;
    private ParticleSystem wallCollisionEffectInstance;
    bool hasCollidedWithWall = false;
    [SerializeField] AudioClip wallCollisionSoundEffectClip;

    //teleporter (spawn positions)
    [SerializeField] private Vector3 leftTunnelExitPosition; //take teleport position as input
    [SerializeField] private Vector3 rightTunnelExitPosition;


    private void Awake()
    {
        //sets pacstudents position at start of the game
        startPos = transform.position;
        targetPos = transform.position;

        //grid positions of pacstudent
        currentGridPosition = Vector3Int.FloorToInt(transform.position);
        targetGridPosition = currentGridPosition;

        dustParticleInstance = Instantiate(dustParticleEffect, transform.position, Quaternion.identity); //instantiate dust particle effect prefab as a gameobject
        dustParticleInstance.transform.SetParent(transform); // set particle effect as a child of PacStudent GameObject so it moves with pacstudent
        dustParticleInstance.Stop();

        wallCollisionEffectInstance = Instantiate(wallCollisionEffect, transform.position, Quaternion.identity); //instantiate wall collision effect
        wallCollisionEffectInstance.transform.SetParent(transform); //set wall collision effect as child of pacstudent to place it at pacstudent
        wallCollisionEffectInstance.Stop(); //stop so it does not play automatically


        //get audiosource
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        TeleportPacstudent();

        //wallCollisionEffectInstance.Play();
        checkLastInput();  //always check for input //check for player input for moving with W, A, S, D keys to move pacstudent

        if (!isMoving)
        {
            //setTargetPosition(currentDirection); //continue moving in currentDirection

            // move to direction of lastInput
            Vector3Int directionFromLastInput = getDirectionFromInput(lastInput);

            // check if walkable
            if (IsWalkable(currentGridPosition + directionFromLastInput))
            {
                currentDirection = directionFromLastInput;
                currentInput = lastInput;  // set currentInput to lastInput - if is walkable store lastInput in currentInput
                SetTargetPosition(currentDirection);

                if (inputReceived)
                {
                    PlayDustParticleEffect();
                }
                //dustParticleEffect.Play();

                StopWallCollisionEffect(); // stop effect in case it plays
                hasCollidedWithWall = false;

            }
            else
            {
                Vector3Int directionFromCurrentInput = getDirectionFromInput(currentInput); //if last input is blocked, continue movement

                if (IsWalkable(currentGridPosition + directionFromCurrentInput))
                {
                    currentDirection = directionFromCurrentInput;
                    SetTargetPosition(currentDirection);

                    if (inputReceived)
                    {
                        PlayDustParticleEffect();
                    }

                    StopWallCollisionEffect();
                    hasCollidedWithWall = false;

                    //dustParticleEffect.Play();
                }
                else
                {
                    //Debug.Log("PacStudent is blocked in both directions.");

                    StopDustParticleEffect();

                    //dustParticleEffect.Stop();

                    //wallCollisionEffectInstance.Play();
                    //Debug.Log("Wall collision effect should be playing");

                    if (!hasCollidedWithWall)
                    {
                        PlayWallCollisionEffect();
                        hasCollidedWithWall = true;
                    }
                }
            }
        }
        else
        {
            MoveTowardsTarget(); //continue moving towards target
        }
    }

    void checkLastInput()
    {
        //check for input and set the new direction (use of lastInput variable)
        if (Input.GetKeyDown(KeyCode.W))
        {
            lastInput = KeyCode.W;
            currentDirection = Vector3Int.up;
            inputReceived = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastInput = KeyCode.D;
            currentDirection = Vector3Int.right;
            inputReceived = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            lastInput = KeyCode.S;
            currentDirection = Vector3Int.down;
            inputReceived = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            lastInput = KeyCode.A;
            currentDirection = Vector3Int.left;
            inputReceived = true;
        }
    }

    Vector3Int getDirectionFromInput(KeyCode input)
    {
        if (input == KeyCode.W) return Vector3Int.up;
        if (input == KeyCode.D) return Vector3Int.right;
        if (input == KeyCode.S) return Vector3Int.down;
        if (input == KeyCode.A) return Vector3Int.left;
        return Vector3Int.zero;
    }

    // check if given grid position is walkable (no wall)
    bool IsWalkable(Vector3Int gridPos)
    {
        // getting tile from appropriate tilemap without converting to world position
        TileBase tileAtPosition = GetTileAtPosition(gridPos);

        // if tile is null and is not in wallTiles array, it is walkable
        if (tileAtPosition == null)
        {
            return true;
        }

        for (int i = 0; i < wallTiles.Length; i++)
        {
            if (wallTiles[i] == tileAtPosition)
            {
                return false; // tile is a wall -> not walkable
            }
        }

        return true; //tile is empty or not a wall


        //return tileAtPosition == null || !wallTiles.Contains(tileAtPosition);
    }

    // get tile from the appropriate tilemap
    TileBase GetTileAtPosition(Vector3Int gridPos)
    {
        // converting grid position to world position
        Vector3 worldPosition = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0); //0.5 puts pacstudent in the center of grid

        // get correct tilemap based on world position
        Tilemap targetTilemap = GetTargetTilemap(worldPosition);

        if (targetTilemap != null)
        {
            // convert world position to correct cell position for the specific tilemap
            Vector3Int cellPosition = targetTilemap.WorldToCell(worldPosition);

            //Debug.Log($"Checking tile at world position: {worldPosition}, cell position: {cellPosition}, tilemap: {targetTilemap.name}");

            return targetTilemap.GetTile(cellPosition);
        }
        return null;
    }

    // Determine tilemap based on grid position
    Tilemap GetTargetTilemap(Vector3 worldPosition)
    {
        //Debug.Log($"Current PacStudent position: {worldPosition}");

        float leftBoundary = -6f;
        float rightBoundary = -11f;
        float topBoundary = -6f;
        float bottomBoundary = -6f;

        if (worldPosition.x < leftBoundary && worldPosition.y >= topBoundary)
        {
            //Debug.Log("Using TopLeftTilemap");
            return topLeftTilemap;
        }
        else if (worldPosition.x >= leftBoundary && worldPosition.y >= topBoundary)
        {
            //Debug.Log("Using TopRightTilemap");
            return topRightTilemap;
        }
        else if (worldPosition.x < leftBoundary && worldPosition.y < bottomBoundary)
        {
            //Debug.Log("Using BottomLeftTilemap");
            return bottomLeftTilemap;
        }
        else if (worldPosition.x >= rightBoundary && worldPosition.y < bottomBoundary)
        {
            //Debug.Log("Using BottomRightTilemap");
            return bottomRightTilemap;
        }
        return null;
    }

    void SetTargetPosition(Vector3Int direction)
    {
        Vector3Int newTargetPosition = currentGridPosition + direction;

        if (IsWalkable(newTargetPosition))
        {
            //wallCollisionEffect.Stop();
            // Set the target grid position based on current direction
            targetGridPosition = newTargetPosition;
            startPos = transform.position;
            targetPos = new Vector3(targetGridPosition.x + 0.5f, targetGridPosition.y + 0.5f, transform.position.z); // 0.5f puts pacstudent in center of grid
            t = 0;
            isMoving = true;

            UpdateAnimatorParam(direction); // Update animation based on direction

            fieldHasPellet = CheckForPellet();
            fieldHasPowerPellet = CheckForPowerPellet();
            //wallCollisionEffect.Play();
        }
        else
        {
            // pacstudent stops moving if new direction is blocked
            isMoving = false;
            //Debug.Log($"Blocked at {newTargetPosition}");
            //audioSource.Stop();
            wallCollisionEffectInstance.transform.position = transform.position + (Vector3)currentDirection * 0.5f;
            if (!wallCollisionEffectInstance.isPlaying)
            {
                //wallCollisionEffectInstance.Play();
                //Debug.Log("Wall collision detected, effect played");                
                PlayWallCollisionEffect();  // play effect on collision
                
                hasCollidedWithWall = true;
            }
            else if (wallCollisionEffectInstance.isPlaying)
            {
                StopWallCollisionEffect();            
                PlayWallCollisionEffect();  // play effect on collision

                hasCollidedWithWall = true;
            }
        }

    }

    void MoveTowardsTarget()
    {
        t += Time.deltaTime * pacstudentMoveSpeed;

        // LERP to move pacstudent from startPos to targetPos
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        // Check whether target position is reached
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; // set Pacstudent to target position
            currentGridPosition = targetGridPosition; // update currentGridPosition
            isMoving = false; // finish movement (lerping), ready for next move

            //add audio for moving or next field with pellet
            if (fieldHasPellet || fieldHasPowerPellet)
            {
                audioSource.clip = eatingPelletClip;
                audioSource.Play();
                //Debug.Log("eating sound playing");
            }
            else
            {
                audioSource.clip = movingNotEatingClip;
                audioSource.Play();
                //Debug.Log("moving sound - not eating");
            }
        }
    }

    void PlayDustParticleEffect()
    {
        dustParticleInstance.Play();
        //Debug.Log("particle effect started");
    }

    void StopDustParticleEffect()
    {
        dustParticleInstance.Stop();
        //Debug.Log("particle effect Stopped");
    }

    void PlayWallCollisionEffect()
    {
        //Debug.Log("Play wall collision");
        wallCollisionEffectInstance.transform.position = transform.position + (Vector3)currentDirection * 0.5f;
        
        //rotation of effect to start from wall collision point
        if (currentDirection == Vector3Int.up)
        {
            wallCollisionEffectInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);  // pacstudent walking UP
        }
        else if (currentDirection == Vector3Int.down)
        {
            wallCollisionEffectInstance.transform.rotation = Quaternion.Euler(90, -90, 0);   // DOWN
        }
        else if (currentDirection == Vector3Int.left)
        {
            wallCollisionEffectInstance.transform.rotation = Quaternion.Euler(0, 0, 90);   // LEFT
        }
        else if (currentDirection == Vector3Int.right)
        {
            wallCollisionEffectInstance.transform.rotation = Quaternion.Euler(0, 90, 90);  // RIGHT
        }

        if (!wallCollisionEffectInstance.isPlaying)
        {
            wallCollisionEffectInstance.Stop();
            wallCollisionEffectInstance.Emit(15);
            audioSource.clip = wallCollisionSoundEffectClip;
            audioSource.Play();
        }
    }

    void StopWallCollisionEffect()
    {
        //Debug.Log("stop wall collision");
        if (wallCollisionEffectInstance.isPlaying)
        {
            wallCollisionEffectInstance.Stop();
        }
    }



    bool CheckForPellet()
    {
        //change to gameobjects
        //// get tile at position
        //TileBase tileAtPosition = GetTileAtPosition(position);

        //// check if pellett tile
        //return tileAtPosition == pelletTile;

        GameObject[] normalPellets = GameObject.FindGameObjectsWithTag("Pellet"); //check for pellet tag to find pellet gameobjects

        for (int i = 0; i < normalPellets.Length; i++)
        {
            Vector3 normalPelletPosition = normalPellets[i].transform.position;

            if (Vector3Int.FloorToInt(normalPelletPosition) == targetGridPosition)
            {
                //fieldHasPowerPellet = true;
                //break; // break for if power pellet is found
                return true;
            }
        }
        return false;

    }

    bool CheckForPowerPellet()
    {
        GameObject[] powerPellets = GameObject.FindGameObjectsWithTag("PowerPellet"); //check for powerpellet tag to find powerpellet gameobjects

        for (int i = 0; i < powerPellets.Length; i++)
        {
            Vector3 pelletPosition = powerPellets[i].transform.position;

            if (Vector3Int.FloorToInt(pelletPosition) == targetGridPosition)
            {
                //fieldHasPowerPellet = true;
                //break; // break for if power pellet is found
                return true;
            }
        }
        return false;
    }

    // 80%
    //teleport pacstudent when at the end of the tunnel
    void TeleportPacstudent()
    {
        //if pacstudent is on the position on the left it gets teleported to the right

        //entering LEFT tunnel
        if (currentGridPosition == Vector3Int.FloorToInt(leftTunnelExitPosition))
        {
            //Vector3 newpos = new Vector3(6.0f, -5.5f, 0);
            //transform.position = newpos;
            Vector3 rightTunnelEntryPosition = new Vector3(rightTunnelExitPosition.x - 1, rightTunnelExitPosition.y, rightTunnelExitPosition.z);
            transform.position = rightTunnelEntryPosition;
            //Debug.Log("Teleport to right");
            currentGridPosition = Vector3Int.FloorToInt(rightTunnelEntryPosition); //update currentGridPos
            SetTargetPosition(currentDirection); //pacstudent continues movement to that direction
        }

        //entering RIGHT tunnel
        else if (currentGridPosition == Vector3Int.FloorToInt(rightTunnelExitPosition))
        {
            //Vector3 newpos = new Vector3(6.0f, -5.5f, 0);
            //transform.position = newpos;
            Vector3 leftTunnelEntryPosition = new Vector3(leftTunnelExitPosition.x + 1, leftTunnelExitPosition.y, leftTunnelExitPosition.z);
            transform.position = leftTunnelEntryPosition;
            //Debug.Log("Teleport to left");
            currentGridPosition = Vector3Int.FloorToInt(leftTunnelEntryPosition); //update currentGridPos
            SetTargetPosition(currentDirection);
        }
    }

    void UpdateAnimatorParam(Vector3Int direction)
    {
        // reset all animator params
        animator.SetBool("walkingUp", false);
        animator.SetBool("walkingDown", false);
        animator.SetBool("walkingLeft", false);
        animator.SetBool("walkingRight", false);

        // change animator params
        if (direction == Vector3Int.up)
        {
            animator.SetBool("walkingUp", true);
        }
        else if (direction == Vector3Int.down)
        {
            animator.SetBool("walkingDown", true);
        }
        else if (direction == Vector3Int.right)
        {
            animator.SetBool("walkingRight", true);
        }
        else if (direction == Vector3Int.left)
        {
            animator.SetBool("walkingLeft", true);
        }
    }
}