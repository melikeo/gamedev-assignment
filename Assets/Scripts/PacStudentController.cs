using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    //using LERP to move Pacstudent from startposition to targetposition

    private Vector3Int currentGridPosition; //current Pacstudent grid position
    private Vector3Int targetGridPosition; //target Pacstudent position

    //start and end position for pacstudents movements
    private Vector3 startPos;
    private Vector3 targetPos;

    [SerializeField] private float moveSpeed = 5f; // Default move speed

    private bool isMoving = false;

    //storing last key player pressed
    private KeyCode lastInput; // Store last input
    private Animator animator;

    private float t = 0; //interpolation value for lerp


    private void Awake()
    {
        //sets pacstudents position at start of the game
        startPos = transform.position;
        targetPos = transform.position;

        //grid positions of pacstudent
        currentGridPosition = Vector3Int.FloorToInt(transform.position);
        targetGridPosition = currentGridPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            checkLastInput();
        }
        else
        {
            moveTowardsTarget(); //move pacstudent to target grid position after checkLastInput was done and set isMoving to true
        }
    }

    void checkLastInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isMoving)
        {
            lastInput = KeyCode.W;
            setTargetPosition(Vector3Int.up);
        }

        if (Input.GetKeyDown(KeyCode.D) && !isMoving)
        {
            lastInput = KeyCode.D;
            setTargetPosition(Vector3Int.right);
        }
        if (Input.GetKeyDown(KeyCode.S) && !isMoving)
        {
            lastInput = KeyCode.S;
            setTargetPosition(Vector3Int.down);
        }
        if (Input.GetKeyDown(KeyCode.A) && !isMoving)
        {
            lastInput = KeyCode.A;
            setTargetPosition(Vector3Int.left);
        }
    }

    void setTargetPosition(Vector3Int direction)
    {
        if (!isMoving)
        {
            //set target grid position to current grid position + lastInput direction
            targetGridPosition = currentGridPosition + direction;
            startPos = transform.position;
            targetPos = new Vector3(targetGridPosition.x + 0.5f, targetGridPosition.y + 0.5f, transform.position.z); // 0.5f puts pacstudent in center of grid
            t = 0;
            isMoving = true;

            updateAnimatorParam(direction);
        }
    }

    void moveTowardsTarget()
    {
        t += Time.deltaTime * moveSpeed;

        // LERP to move pacstudent from startPos to targetPos
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        //check whether targetpos is reached
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; //place pacstudent to targetpos            
            currentGridPosition = targetGridPosition; //update currentGridPosition
            isMoving = false; //finish movement (lerping)
        }
    }

    void updateAnimatorParam(Vector3Int direction)
    {
        animator.SetBool("walkingUp", false);
        animator.SetBool("walkingDown", false);
        animator.SetBool("walkingLeft", false);
        animator.SetBool("walkingRight", false);

        //change animator params
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
