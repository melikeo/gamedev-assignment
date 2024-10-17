using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private Vector3Int currentGridPosition; //current Pacstudent grid position
    private Vector3Int targetGridPosition; //target Pacstudent position

    private bool isMoving = false;

    //storing last key player pressed
    private KeyCode lastInput;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            lastInput = KeyCode.W;
        }

    }
}
