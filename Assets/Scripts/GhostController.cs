using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostController : MonoBehaviour
{
    private Vector3Int currentGridPosition; //current Ghost position
    private Vector3Int targetGridPosition; //target Ghost position

    private Vector3 startPos;
    private Vector3 targetPos;

    [SerializeField] private float ghostMoveSpeed = 5f;

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

    Vector3Int newDirection;

    // spawn exit route
    private List<Vector3Int> spawnExitRoute;
    private int spawnExitIndex = 0;
    private bool didExitSpawn = false;


    // block teleporting of ghosts
    [SerializeField] private Vector3 leftTunnelExitPosition;
    [SerializeField] private Vector3 rightTunnelExitPosition;


    private void Awake()
    {
        //start setup
        startPos = transform.position;
        targetPos = transform.position;
        currentGridPosition = Vector3Int.FloorToInt(transform.position);
        targetGridPosition = currentGridPosition;

        animator = GetComponent<Animator>();


        spawnExitRoute = new List<Vector3Int>
    {
        new Vector3Int(-6, -5, 0),
        new Vector3Int(-6, -4, 0),
        new Vector3Int(-6, -3, 0),
        new Vector3Int(-6, -2, 0)
    };


    }

    private void Update()
    {
        BlockTeleporting();

        if (!isMoving)
        {
            if (!didExitSpawn)
            {
                TakeSpawnExitRoute(); // if ghost is in spawn direction, move it out
                SetWalkingDirection(currentDirection);
            }

            else
            {
                if (ghostID == 4)
                {
                    // set new direction
                    newDirection = ChooseRandomDirection();

                    if (IsWalkable(currentGridPosition + newDirection))
                    {
                        currentDirection = newDirection;
                        SetTargetPosition(currentDirection);
                    }
                }
                else
                {
                    //code for OTHER STATES will be added here
                    //if(currentState == dead)
                    //{
                    //returntospawn();
                    //set state = dead;
                    //}
                }
            }

        //else
        //{
            // other ghosts to be added!
            //newDirection = ChooseSpecificDirection();
        //}

            //if (IsWalkable(currentGridPosition + newDirection))
            //{
            //    currentDirection = newDirection;
            //    SetTargetPosition(currentDirection);
            //}
        }
        else
        {
            MoveTowardsTarget();
        }
    }

        Vector3Int ChooseSpecificDirection()
        {
            //other ghosts behaviour
            return Vector3Int.right; // change
        }




        Vector3Int ChooseRandomDirection()
    {
        // random direction
        Vector3Int[] possibleDirections = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        //filter last direction for no backstepping
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

    void TakeSpawnExitRoute()
    {
        if (spawnExitIndex < spawnExitRoute.Count)
        {
            Vector3Int nextPosition = spawnExitRoute[spawnExitIndex];

            if (IsWalkable(nextPosition))
            {
                SetTargetPosition(nextPosition - currentGridPosition);
                spawnExitIndex++; // go to next point on route
            }
        }
        else
        {
            didExitSpawn = true; // true if all points on route list are done
        }

        didExitSpawn = true;
    }

    bool IsWalkable(Vector3Int gridPos)
    {
        TileBase tileAtPosition = GetTileAtPosition(gridPos);
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