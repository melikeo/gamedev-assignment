using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostController : MonoBehaviour
{
    private Vector3Int currentGridPosition; //current Ghost position
    private Vector3Int targetGridPosition; //target Ghost position

    private Vector3 startPos;
    private Vector3 targetPos;

    [SerializeField] private float ghostMoveSpeed = 2f;

    private bool isMoving = false;

    private Vector3Int currentDirection;
    private float t = 0;

    [SerializeField] private Tilemap topLeftTilemap;
    [SerializeField] private Tilemap topRightTilemap;
    [SerializeField] private Tilemap bottomLeftTilemap;
    [SerializeField] private Tilemap bottomRightTilemap;

    //list of wall tiles that will be checked
    [SerializeField] private TileBase[] wallTiles;

    private Vector3Int lastDirection = Vector3Int.zero;

    [SerializeField] private int ghostID;

    private Animator animator;

    //default no ghost is dead
    private bool isDead = false;


    Vector3Int newDirection;

    // spawn exit route
    private List<Vector3Int> spawnExitRoute;
    private int spawnExitIndex = 0;
    private bool didExitSpawn = false;


    // block teleporting of ghosts
    private Vector3 leftTunnelExitPosition;
    private Vector3 rightTunnelExitPosition;

    // block re-entering spawn area
    private List<Vector3Int> spawnAreaEntryFields;

    // ghost 4 clockwise rotation points
    private List<Vector3Int> clockRotationPoints;
    private int clockRotationIndex = 0;
    private Vector3Int currentClockTarget;

    // respawn dead ghost
    Vector3 targetRespawnPosition = Vector3.zero;

    // pacstudent reference
    [SerializeField] private Transform pacStudent;

    // Ghost 1 and 2 direction options
    Vector3Int[] possibleDirections = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };


    private void Awake()
    {
        //start setup
        startPos = transform.position;
        targetPos = transform.position;
        currentGridPosition = Vector3Int.FloorToInt(transform.position);
        targetGridPosition = currentGridPosition;

        animator = GetComponent<Animator>();

        // waypoints to exit spawn area
        spawnExitRoute = new List<Vector3Int>
    {
        new Vector3Int(-6, -5, 0),
        new Vector3Int(-6, -4, 0),
        new Vector3Int(-6, -3, 0)
    };

        //teleporting positions
        leftTunnelExitPosition = new Vector3Int(-20, -6, 0);
        rightTunnelExitPosition = new Vector3Int(7, -6, 0);

        
        // spawn area entry fields
        spawnAreaEntryFields = new List<Vector3Int>
        {
            new Vector3Int(-7, -4, 0),
            new Vector3Int(-6, -4, 0),
            new Vector3Int(-7, -8, 0),
            new Vector3Int(-6, -8, 0)
        };

        // ghost 4 clockwise rotation waypoints
        clockRotationPoints = new List<Vector3Int>
        {
            new Vector3Int(-5, 7, 0),
            new Vector3Int(6, 7, 0),
            new Vector3Int(6, 0, 0),
            new Vector3Int(6, -12, 0),
            new Vector3Int(6, -19, 0),
            new Vector3Int(-5, -19, 0),
            new Vector3Int(-8, -19, 0),
            new Vector3Int(-19, -19, 0),
            new Vector3Int(-19, -12, 0),
            new Vector3Int(-19, 0, 0),
            new Vector3Int(-19, 7, 0),
            new Vector3Int(-8, 7, 0),
        };


    }

    private void Update()
    {
        BlockTeleporting();

        //check for death state for respawn
        bool isDeadInAnimator = animator.GetBool("Dead");

        if (isDeadInAnimator)
        {
            isMoving = true; //stop moving
            isDead = true;
            Debug.Log("jetzt sollte er eigentlich in spawn area gespawnt werden");
            //respawnDeadGhost();
            StartCoroutine(RespawnDeadGhost());
        }


        if (!isMoving && !isDeadInAnimator)
        {

            if (!didExitSpawn)
            {
                //Debug.Log("if(!ismoving) didexitspawn:" + didExitSpawn);
                TakeSpawnExitRoute();

            }

            else
            {
                //Debug.Log("if(!ismoving) didexitspawn:" + didExitSpawn);
                //Debug.Log("es sollte eigentlich jetzt laufen, wenn das vorherige true geworden ist.");             

                //if ghostID == 1 || ghost state = scared or recovering {}

                if (ghostID == 1)
                {
                    newDirection = Ghost1MovementFurtherDistance();

                    if (IsWalkable(currentGridPosition + newDirection))
                    {
                        currentDirection = newDirection;
                        SetTargetPosition(currentDirection);
                        SetWalkingDirection(currentDirection);
                    }
                }


                if (ghostID == 2)
                {
                    newDirection = Ghost2MovementCloserDistance();

                    if (IsWalkable(currentGridPosition + newDirection))
                    {
                        currentDirection = newDirection;
                        SetTargetPosition(currentDirection);
                        SetWalkingDirection(currentDirection);
                    }
                }
                
                
                if (ghostID == 3)
                {
                    // ------ random directions ------
                    // set new direction
                    newDirection = ChooseRandomDirection();

                    if (IsWalkable(currentGridPosition + newDirection))
                    {
                        currentDirection = newDirection;
                        SetTargetPosition(currentDirection);
                        SetWalkingDirection(currentDirection);
                    }
                }

                if (ghostID == 4)
                {
                    // ----- clockwise around the map -----

                    if (currentClockTarget == Vector3Int.zero) // set first target
                    {
                        currentClockTarget = clockRotationPoints[clockRotationIndex];
                    }
                    MoveTowardsClockwiseTarget();
                }
            }
        }

        else
        {
            MoveTowardsTarget();
        }
    }

    IEnumerator RespawnDeadGhost()
    {
        Vector3 startPos = transform.position;
        float duration = 1.0f; // spawn duration
        float elapsedTime = 0; // time passed

        //Debug.Log("this method should be called!");

        if (ghostID == 1)
        {
            targetRespawnPosition = new Vector3(-7.5f, -5, 0);
        }

        else if (ghostID == 2)
        {
            targetRespawnPosition = new Vector3(-6.5f, -5, 0);
        }

        else if (ghostID == 3)
        {
            targetRespawnPosition = new Vector3(-5.5f, -5, 0);
            //Debug.Log("it's ghost 3");
        }

        else if (ghostID == 4)
        {
            targetRespawnPosition = new Vector3(-4.5f, -5, 0);
        }

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetRespawnPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // wait for next frame
        }

        transform.position = targetRespawnPosition;
        currentGridPosition = Vector3Int.FloorToInt(targetRespawnPosition);
        targetGridPosition = currentGridPosition;

        didExitSpawn = false;
        spawnExitIndex = 0;

        //reset values after respawn
        isDead = false;
        animator.SetBool("Dead", false);

        //if (transform.position != targetRespawnPosition)
        //{
        //    //Debug.Log("current != target");

        //    //Debug.Log("animator state" + animator.GetBool("Dead"));

        //    //Debug.Log(isDead);


        //    // framerate independent
        //    t += Time.deltaTime * ghostMoveSpeed;

        //    // LERP to move ghost to target respawn position
        //    transform.position = Vector3.Lerp(transform.position, targetRespawnPosition, t);

        //    // put ghost on target position when close enough
        //    if (Vector3.Distance(transform.position, targetRespawnPosition) < 0.01f)
        //    {
        //        transform.position = targetRespawnPosition;                
        //    }
        //}

        //animator.SetBool("Dead", false);
        //isDead = false;

    }


    // --- Ghost 1 - further/equal distance ---
    Vector3Int Ghost1MovementFurtherDistance()
    {
        //Vector3Int bestDirection = Vector3Int.zero;
        //float bestDistance = float.MinValue; // init with low value

        float currentDistance = Vector3.Distance(currentGridPosition, pacStudent.position); // calculate distance between ghost and pacstudent

        List<Vector3Int> validDirections = new List<Vector3Int>(); // list with valid direction options

        foreach (var direction in possibleDirections)
        {
            Vector3Int nextPosition = currentGridPosition + direction;

            // Exclude last direction to avoid going back
            if (nextPosition == currentGridPosition - lastDirection)
            {
                continue;
            }

            if (IsWalkable(nextPosition))
            {
                float newDistance = Vector3.Distance(nextPosition, pacStudent.position);

                if (newDistance >= currentDistance)
                {
                    validDirections.Add(direction);
                }
            }
        }

        // if no valid direction?
        if (validDirections.Count == 0)
        {
            //Debug.Log("No valid direction found.");
            return lastDirection;
        }

        return validDirections[Random.Range(0, validDirections.Count)];
    }

    // --- Ghost 2 - closer/equal distance ---
    Vector3Int Ghost2MovementCloserDistance()
    {
        List<Vector3Int> validDirections = new List<Vector3Int>();

        //Vector3Int bestDirection = Vector3Int.zero;
        //float bestDistance = float.MaxValue; // init with high value

        float currentDistance = Vector3.Distance(currentGridPosition, pacStudent.position); // calculate distance between ghost and pacstudent

        foreach (var direction in possibleDirections)
        {
            Vector3Int nextPosition = currentGridPosition + direction;

            // Exclude last direction to avoid going back
            if (nextPosition == currentGridPosition - lastDirection)
            {
                continue;
            }

            if (IsWalkable(nextPosition))
            {                
                float newDistance = Vector3.Distance(nextPosition, pacStudent.position);

                //add direction if it keeps PacStudent closer or at equal distance
                if (newDistance <= currentDistance)
                {
                    validDirections.Add(direction);
                }
            }
        }

        // if no valid direction?
        //if (bestDirection == Vector3Int.zero)
        //{
        //    //Debug.Log("No valid direction found.");
        //    bestDirection = lastDirection;
        //}

        //return bestDirection;
                 
        
        // If no valid direction found, keep last direction
        if (validDirections.Count == 0)
        {
            Debug.Log("No walkable positions found!");

            // Fallback: random direction (if ghost cannot move)
            //newDirection = ChooseRandomDirection();
            //SetTargetPosition(newDirection);

            return lastDirection;
        }

        // Choose a random valid direction
        return validDirections[Random.Range(0, validDirections.Count)];


        //if ghost cannot move to any direction it should move to a random direction TBA!! for all ghosts

    }

    // --- Ghost 3 - Random Direction ---
    Vector3Int ChooseRandomDirection()
    {
        // random direction
        Vector3Int[] possibleDirections = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        // Exclude last direction to avoid going back
        Vector3Int oppositeDirection = -lastDirection;

        List<Vector3Int> filteredDirections = new List<Vector3Int>();
        foreach (var dir in possibleDirections)
        {
            if (dir != oppositeDirection)
            {
                filteredDirections.Add(dir);
            }
        }

        List<Vector3Int> walkableDirections = new List<Vector3Int>();
        foreach (var dir in filteredDirections)
        {
            if (IsWalkable(currentGridPosition + dir))
            {
                walkableDirections.Add(dir);
            }
        }

        // if all directions are blocked, add last direction
        if (walkableDirections.Count == 0)
        {
            walkableDirections.Add(oppositeDirection);
        }

        // choose random direction
        return walkableDirections[Random.Range(0, walkableDirections.Count)];

    }

    // --- Ghost 4 - clockwise rotation ---
    void MoveTowardsClockwiseTarget()
    {
        if (currentGridPosition == currentClockTarget)
        {
            // increase index when target reached
            clockRotationIndex = (clockRotationIndex + 1) % clockRotationPoints.Count;
            currentClockTarget = clockRotationPoints[clockRotationIndex];
        }

        // find next direction towards target
        Vector3Int directionToTarget = GetDirectionTowards(currentClockTarget);

        if (IsWalkable(currentGridPosition + directionToTarget))
        {
            SetTargetPosition(directionToTarget);
        }
        else
        {
            //Debug.Log("Pathfinding is not working. Fallback else.");
            // Fallback: random direction (if ghost cannot move)
            newDirection = ChooseRandomDirection();
            SetTargetPosition(newDirection);
        }
    }

    Vector3Int GetDirectionTowards(Vector3Int target)
    {
        Vector3Int[] possibleDirections = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        Vector3Int bestDirection = Vector3Int.zero;
        float shortestDistance = float.MaxValue;

        foreach (var direction in possibleDirections)
        {
            Vector3Int nextPosition = currentGridPosition + direction;

            // Exclude last direction to avoid going back
            if (nextPosition == currentGridPosition - lastDirection)
            {
                continue;
            }

            if (IsWalkable(nextPosition))
            {
                float distance = Vector3Int.Distance(nextPosition, target);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestDirection = direction;
                }
            }
        }

        return bestDirection == Vector3Int.zero ? lastDirection : bestDirection;
    }




    // All Ghosts: Exit Spawn Area
    void TakeSpawnExitRoute()
    {
        if (spawnExitIndex < spawnExitRoute.Count)
        {
            Vector3Int nextPosition = spawnExitRoute[spawnExitIndex];

            if (IsWalkable(nextPosition))
            {
                SetTargetPosition(nextPosition - currentGridPosition);
                spawnExitIndex++; // go to next point on route
                //Debug.Log("Moving to spawn exit point: " + nextPosition + " (Index: " + spawnExitIndex + ")");
            }
        }
        else
        {
            didExitSpawn = true; // true if all points on route list are done
            //Debug.Log("Exited spawn route. didExitSpawn set to true.");
        }
    }

    bool IsWalkable(Vector3Int gridPos)
    {
        TileBase tileAtPosition = GetTileAtPosition(gridPos);

        if (didExitSpawn && !isDead && spawnAreaEntryFields.Contains(gridPos))
        {
            return false; // spawn area fields cannot be accessed
        }


        if (tileAtPosition == null) return true;

        foreach (var wallTile in wallTiles)
        {
            if (wallTile == tileAtPosition)
                return false; // tile is a wall -> not walkable
        }
        return true;
    }

    TileBase GetTileAtPosition(Vector3Int gridPos)
    {
        Vector3 worldPosition = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
        Tilemap targetTilemap = GetTargetTilemap(worldPosition);

        if (targetTilemap != null)
        {
            Vector3Int cellPosition = targetTilemap.WorldToCell(worldPosition);
            return targetTilemap.GetTile(cellPosition);
        }
        return null;
    }

    Tilemap GetTargetTilemap(Vector3 worldPosition)
    {
        float leftBoundary = -6f;
        float rightBoundary = -11f;
        float topBoundary = -6f;
        float bottomBoundary = -6f;

        if (worldPosition.x < leftBoundary && worldPosition.y >= topBoundary)
        {
            return topLeftTilemap;
        }
        else if (worldPosition.x >= leftBoundary && worldPosition.y >= topBoundary)
        {
            return topRightTilemap;
        }
        else if (worldPosition.x < leftBoundary && worldPosition.y < bottomBoundary)
        {
            return bottomLeftTilemap;
        }
        else if (worldPosition.x >= rightBoundary && worldPosition.y < bottomBoundary)
        {
            return bottomRightTilemap;
        }
        return null;
    }

    void SetTargetPosition(Vector3Int direction)
    {
        Vector3Int newTargetPosition = currentGridPosition + direction;
        if (IsWalkable(newTargetPosition))
        {
            lastDirection = direction;
            targetGridPosition = newTargetPosition;
            startPos = transform.position;
            targetPos = new Vector3(targetGridPosition.x + 0.5f, targetGridPosition.y + 0.5f, transform.position.z);
            t = 0;
            isMoving = true;

            SetWalkingDirection(direction);
        }
    }

    void MoveTowardsTarget()
    {
        t += Time.deltaTime * ghostMoveSpeed;

        //LERP to move ghost
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            currentGridPosition = targetGridPosition;
            isMoving = false;
        }
    }



    // Animator management
    void SetWalkingDirection(Vector3Int direction)
    {
        // set initial value to false
        animator.SetBool("walkingRight", false);
        animator.SetBool("walkingLeft", false);
        animator.SetBool("walkingUp", false);
        animator.SetBool("walkingDown", false);

        // change param based on direction
        if (direction == Vector3Int.right)
        {
            animator.SetBool("walkingRight", true);
        }
        else if (direction == Vector3Int.left)
        {
            animator.SetBool("walkingLeft", true);
        }
        else if (direction == Vector3Int.up)
        {
            animator.SetBool("walkingUp", true);
        }
        else if (direction == Vector3Int.down)
        {
            animator.SetBool("walkingDown", true);
        }
    }

    // set scared state
    void SetScaredState()
    {
        animator.SetBool("Scared", true);
        animator.SetBool("Recovering", false);
        animator.SetBool("Dead", false);
    }

    // set dead state
    void SetDeadState()
    {
        animator.SetBool("Dead", true);
        animator.SetBool("Scared", false);
        animator.SetBool("Recovering", false);
    }

    // recovering state
    void SetRecoveringState()
    {
        animator.SetBool("Recovering", true);
        animator.SetBool("Scared", false);
        animator.SetBool("Dead", false);
    }

    //block teleporting of ghosts
    void BlockTeleporting()
    {
        //Debug.Log("Current Ghost Position: " + currentGridPosition);
        //Debug.Log("Left Tunnel Exit Position: " + leftTunnelExitPosition);
        //Debug.Log("Right Tunnel Exit Position: " + rightTunnelExitPosition);

        if (currentGridPosition == leftTunnelExitPosition)
        {
            //Debug.Log("ghost is on left teleport position");
            // if ghost is on teleport position change direction
            currentDirection = Vector3Int.right; // ghost is in left position -> change to right direction
            SetTargetPosition(currentDirection);
        }
        else if (currentGridPosition == rightTunnelExitPosition)
        {
            //Debug.Log("ghost is on right teleport position");
            currentDirection = Vector3Int.left;
            SetTargetPosition(currentDirection);
        }
    }


}