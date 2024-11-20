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

    [SerializeField] public Tilemap topLeftTilemap;
    [SerializeField] public Tilemap topRightTilemap;
    [SerializeField] public Tilemap bottomLeftTilemap;
    [SerializeField] public Tilemap bottomRightTilemap;

    //list of wall tiles that will be checked
    [SerializeField] private TileBase[] wallTiles;

    private Vector3Int lastDirection = Vector3Int.zero;

    [SerializeField] private int ghostID;

    private Animator animator;

    //default no ghost is dead
    private bool isDead = false;


    Vector3Int newDirection;

    // spawn exit routes
    private List<Vector3Int> spawnRightExitRoute;
    private List<Vector3Int> spawnLeftExitRoute;
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
    Vector3 targetRespawnPosition = Vector3.zero; //initialising
    private List<GhostController> otherGhosts; // to check for states of other ghosts

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
        spawnRightExitRoute = new List<Vector3Int>
    {
        new Vector3Int(-6, -5, 0),
        new Vector3Int(-6, -4, 0),
        new Vector3Int(-6, -3, 0)
    };
        spawnLeftExitRoute = new List<Vector3Int>
    {
        new Vector3Int(-7, -5, 0),
        new Vector3Int(-7, -4, 0),
        new Vector3Int(-7, -3, 0)
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

        // get ghosts states
        otherGhosts = new List<GhostController>(FindObjectsByType<GhostController>(FindObjectsSortMode.None));
        otherGhosts.Remove(this); // remove this ghost from list to check states of the others

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

        //check death state for respawn
        bool isDeadInAnimator = animator.GetBool("Dead");

        if (isDeadInAnimator)
        {
            isMoving = true; //stop moving
            //isDead = true;
            StartCoroutine(RespawnDeadGhost());
            return;
        }


        if (!isMoving && !isDeadInAnimator)
        {

            bool isScaredInAnimator = animator.GetBool("Scared");
            bool isRecoveringInAnimator = animator.GetBool("Recovering");

            if (!didExitSpawn)
            {
                //Debug.Log("if(!ismoving) didexitspawn:" + didExitSpawn);
                TakeSpawnExitRoute();
            }

            else
            {
                // ---- Ghost Scared/Recovering -> Ghost 1 behaviour ----
                if (isScaredInAnimator || isRecoveringInAnimator)
                {
                    // Ghost 1 behaviour
                    newDirection = Ghost1MovementFurtherDistance();

                    if (IsWalkable(currentGridPosition + newDirection))
                    {
                        currentDirection = newDirection;
                        SetTargetPosition(currentDirection);
                        SetWalkingDirection(currentDirection);
                    }
                }

                else
                {
                    switch (ghostID)
                    {
                        // ---- Ghost 1: Further/equal distance ----
                        case 1:
                            newDirection = Ghost1MovementFurtherDistance();

                            if (IsWalkable(currentGridPosition + newDirection))
                            {
                                currentDirection = newDirection;
                                SetTargetPosition(currentDirection);
                                SetWalkingDirection(currentDirection);
                            }
                            break;

                        // ---- Ghost 2: Closer/equal distance ----
                        case 2:
                            newDirection = Ghost2MovementCloserDistance();

                            if (IsWalkable(currentGridPosition + newDirection))
                            {
                                currentDirection = newDirection;
                                SetTargetPosition(currentDirection);
                                SetWalkingDirection(currentDirection);
                            }
                            break;

                        // ---- Ghost 3: Random directions ----
                        case 3:                               
                                // set new direction
                            newDirection = ChooseRandomDirection();

                            if (IsWalkable(currentGridPosition + newDirection))
                            {
                                currentDirection = newDirection;
                                SetTargetPosition(currentDirection);
                                SetWalkingDirection(currentDirection);
                            }
                            break;

                        // ---- Ghost 4: Clockwise around the map  ----
                        case 4:
                            if (currentClockTarget == Vector3Int.zero) // set first target
                            {
                                currentClockTarget = clockRotationPoints[clockRotationIndex];
                            }
                            MoveTowardsClockwiseTarget();
                            break;
                    }
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
        //isDead = true;

        SetDeadState();

        Vector3 startPos = transform.position;
        float duration = 1.0f; // spawn duration
        float elapsedTime = 0; // time passed

        // Ghosts respawn positions
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

        //reset positions after respawn
        startPos = transform.position;
        targetPos = transform.position;

        if(!animator.GetBool("Dead"))
        {
            AfterRespawnState();
            TakeSpawnExitRoute();
        }

        //reset values after respawn
        //animator.SetBool("Dead", false); // --- duplicate - is set in Collisions already
        //animator.SetBool("Dead", false);
        //isDead = false;

    }

    void AfterRespawnState()
    {
        // Set ghosts to state that other ghosts are in (scared/recovering etc.)
        // Check states of other ghosts
        // Set state to recovering or scared when other ghosts are in that

        bool anyScared = otherGhosts.Exists(ghost => ghost.animator.GetBool("Scared"));
        bool anyRecovering = otherGhosts.Exists(ghost => ghost.animator.GetBool("Recovering"));

        //Debug.Log($"Respawned Ghost {ghostID}. Scared: {anyScared}, Recovering: {anyRecovering}");

        if (anyScared)
        {
            SetScaredState();
            //Debug.Log($"Ghost {ghostID}: Animator Scared State Set to: {animator.GetBool("Scared")}");
        }
        else if (anyRecovering)
        {
            SetRecoveringState();
            //Debug.Log($"Ghost {ghostID}: Animator Recovering State Set to: {animator.GetBool("Recovering")}");
        }

        else
        {
            animator.SetBool("Recovering", false);
            animator.SetBool("Scared", false);
        }

    }


    // --- Ghost 1 - Further/equal distance ---
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
            //Debug.Log("Ghost 1: No valid direction found.");

            // Fallback: random direction (if ghost cannot move anymore)
            newDirection = ChooseRandomDirection();
            SetTargetPosition(newDirection);

            return lastDirection;
        }

        return validDirections[Random.Range(0, validDirections.Count)];
    }

    // --- Ghost 2 - Closer/equal distance ---
    Vector3Int Ghost2MovementCloserDistance()
    {
        List<Vector3Int> validDirections = new List<Vector3Int>();

        //Vector3Int bestDirection = Vector3Int.zero; // to choose best path, not randomly
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
            //Debug.Log("No walkable positions found Ghost 2!");

            // Fallback: random direction (if ghost cannot move)
            newDirection = ChooseRandomDirection();
            SetTargetPosition(newDirection);

            return lastDirection;
        }

        // Choose a random valid direction
        return validDirections[Random.Range(0, validDirections.Count)];
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
            //Debug.Log("Ghost 3 no walkable position!");
        }

        // choose random direction
        return walkableDirections[Random.Range(0, walkableDirections.Count)];
    }

    // --- Ghost 4 - Clockwise rotation ---
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
            //Debug.Log("Ghost 4: Pathfinding is not working. Fallback else.");
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
        // Ghost 1 and 2 take left route to exit, Ghost 3 and 4 exit from right way
        List<Vector3Int> spawnExitRoute = (ghostID == 1 || ghostID == 2) ? spawnLeftExitRoute : spawnRightExitRoute;

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
        //if (IsWalkable(newTargetPosition))
        if (t >= 1f || Vector3.Distance(transform.position, targetPos) < 0.05f)
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
        if (currentGridPosition == leftTunnelExitPosition)
        {
            // Ghost is on left teleport position
            // if ghost is on teleport position change direction
            currentDirection = Vector3Int.right; // ghost is in left position -> change to right direction
            SetTargetPosition(currentDirection);
            MoveTowardsTarget();
        }
        else if (currentGridPosition == rightTunnelExitPosition)
        {
            // Ghost is on right teleport position
            currentDirection = Vector3Int.left;
            SetTargetPosition(currentDirection);
            MoveTowardsTarget();
        }
    }
}