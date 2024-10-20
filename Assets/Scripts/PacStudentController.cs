using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private Vector3Int currentGridPosition; //current Pacstudent grid position
    private Vector3Int targetGridPosition;  //target Pacstudent position

    //start and end position for pacstudents movements
    private Vector3 startPos;
    private Vector3 targetPos;

    [SerializeField] private float moveSpeed = 5f; //default move speed

    private bool isMoving = false;

    //storing last key player pressed
    private KeyCode lastInput; //store last input
    private Animator animator;

    private float t = 0; //interpolation value for lerp

    private Vector3Int currentDirection; //stores current moving direction

    //levelMap to check for wall collision
    private int[,] levelMap = {
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

    private void Awake()
    {
        //sets pacstudents position at start of the game
        startPos = transform.position;
        targetPos = transform.position;

        //grid positions of pacstudent
        currentGridPosition = Vector3Int.FloorToInt(transform.position);
        targetGridPosition = currentGridPosition;

        //currentDirection = Vector3Int.right; // Default direction (right) - can be changed
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkLastInput();  //always check for input

        if (!isMoving)
        {
            setTargetPosition(currentDirection); //continue moving in currentDirection
        }
        else
        {
            moveTowardsTarget(); //continue moving towards target
        }
    }

    void checkLastInput()
    {
        //check for input and set the new direction
        if (Input.GetKeyDown(KeyCode.W))
        {
            lastInput = KeyCode.W;
            currentDirection = Vector3Int.up;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastInput = KeyCode.D;
            currentDirection = Vector3Int.right;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            lastInput = KeyCode.S;
            currentDirection = Vector3Int.down;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            lastInput = KeyCode.A;
            currentDirection = Vector3Int.left;
        }
    }

    void setTargetPosition(Vector3Int direction)
    {
        if (!isMoving)
        {
            //set target grid position based on current direction
            targetGridPosition = currentGridPosition + direction;
            startPos = transform.position;
            targetPos = new Vector3(targetGridPosition.x + 0.5f, targetGridPosition.y + 0.5f, transform.position.z); // 0.5f puts pacstudent in center of grid
            t = 0;
            isMoving = true;

            updateAnimatorParam(direction); //update animation based on direction
        }
    }

    void moveTowardsTarget()
    {
        t += Time.deltaTime * moveSpeed;

        // LERP to move pacstudent from startPos to targetPos
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        // check whether target position is reached
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; // set Pacstudent to target position
            currentGridPosition = targetGridPosition; // update currentGridPosition
            isMoving = false; // finish movement (lerping), ready for next move
        }
    }

    void updateAnimatorParam(Vector3Int direction)
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