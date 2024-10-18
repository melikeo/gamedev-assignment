using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private Vector3Int currentGridPosition; // Current Pacstudent grid position
    private Vector3Int targetGridPosition; // Target Pacstudent position

    // Start and end position for Pacstudents movements
    private Vector3 startPos;
    private Vector3 targetPos;

    [SerializeField] private float moveSpeed = 5f; // Default move speed

    private bool isMoving = false;

    private Animator animator;

    private float t = 0; // Interpolation value for Lerp

    private void Awake()
    {
        // Set Pacstudent's initial position
        startPos = transform.position;
        targetPos = transform.position;

        // Grid positions of Pacstudent
        currentGridPosition = Vector3Int.FloorToInt(transform.position);
        targetGridPosition = currentGridPosition;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving)
        {
            checkLastInput();
        }
        else
        {
            moveTowardsTarget(); // Move Pacstudent to target grid position
        }
    }

    void checkLastInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isMoving) // Only register input when not moving
        {
            setTargetPosition(Vector3Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.D) && !isMoving)
        {
            setTargetPosition(Vector3Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.S) && !isMoving)
        {
            setTargetPosition(Vector3Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) && !isMoving)
        {
            setTargetPosition(Vector3Int.left);
        }
    }

    void setTargetPosition(Vector3Int direction)
    {
        // Set target grid position based on direction
        targetGridPosition = currentGridPosition + direction;
        startPos = transform.position;
        targetPos = new Vector3(targetGridPosition.x + 0.5f, targetGridPosition.y + 0.5f , transform.position.z);
        t = 0; // Reset Lerp progress
        isMoving = true; // Now moving

        updateAnimatorParam(direction);
    }

    void moveTowardsTarget()
    {
        // Increment the Lerp progress based on moveSpeed
        t += Time.deltaTime * moveSpeed;

        // LERP to move Pacstudent from startPos to targetPos
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        // Check if the target position is reached
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; // Snap to grid
            currentGridPosition = targetGridPosition; // Update current grid position
            isMoving = false; // Stop moving
        }

        // Update the animator parameters based on movement direction
        //Vector3 direction = (targetPos - startPos).normalized;
        //updateAnimatorParam(direction);
    }

    void updateAnimatorParam(Vector3 direction)
    {
        // Reset all animator parameters
        animator.SetBool("walkingUp", false);
        animator.SetBool("walkingDown", false);
        animator.SetBool("walkingLeft", false);
        animator.SetBool("walkingRight", false);

        // Set the correct animation based on the movement direction
        //if (direction.y > 0)
        //{
        //    animator.SetBool("walkingUp", true);
        //}
        //else if (direction.y < 0)
        //{
        //    animator.SetBool("walkingDown", true);
        //}
        //else if (direction.x > 0)
        //{
        //    animator.SetBool("walkingRight", true);
        //}
        //else if (direction.x < 0)
        //{
        //    animator.SetBool("walkingLeft", true);
        //}
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
