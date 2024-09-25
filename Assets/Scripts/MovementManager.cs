using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private GameObject pacStudent; // pacStudent GameObject
    [SerializeField] private GameObject pacStudent2; //pacStudent2 GameObject to show Deathstate
    [SerializeField] private AudioClip movementAudioClip;
    private AudioSource audioSource;
    private int pacstudentIndex = 0; // target position
    private Vector2[] positions = new Vector2[]
    {
        new Vector2(-18.4f, 7.5f),   // top left
        new Vector2(-13.4f, 7.5f),   // top right
        new Vector2(-13.4f, 3.6f),   // bottom right
        new Vector2(-18.4f, 3.6f)    // bottom left
    };

    private float speed = 1f;
    private float t = 0f;

    private Animator animator; // to get animator for parameters
    private bool isDead = false;

    void Start()
    {
        pacStudent.transform.position = positions[0]; // starting point
        t = 0f; // init t

        audioSource = pacStudent.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = pacStudent.AddComponent<AudioSource>();
        }

        audioSource.clip = movementAudioClip; //to put movement sound audio clip


        animator = pacStudent.GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
        {
            animator.SetBool("isDead", true);
            pacStudent2.SetActive(true);
        }
        if (t < 1f)
        {
            t += Time.deltaTime * speed; //framerate independent

            //LERP
            pacStudent.transform.position = Vector2.Lerp(positions[pacstudentIndex], positions[(pacstudentIndex + 1) % positions.Length], t);
        }
        else
        {            
            pacstudentIndex = (pacstudentIndex + 1) % positions.Length; // if tween done, set next position 
            t = 0f; // reset t
            UpdateAnimation();
        }

        PlayMovementAudio();
    }

    private void PlayMovementAudio()
    {
        AudioSource audioSource = pacStudent.GetComponent<AudioSource>();
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play(); // play audio
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // set all animator bools to false
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);

            // movement depends on index ( 0 -> 1 is walkingRight)
            if (pacstudentIndex == 0)
            {
                animator.SetBool("walkingRight", true);
            }
            else if (pacstudentIndex == 1)
            {
                animator.SetBool("walkingDown", true);
            }
            else if (pacstudentIndex == 2)
            {
                animator.SetBool("walkingLeft", true);
            }
            else if (pacstudentIndex == 3)
            {
                animator.SetBool("walkingUp", true);
            }
        }

    }
}
