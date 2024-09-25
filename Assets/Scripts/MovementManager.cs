using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private GameObject pacStudent; // pacStudent GameObject
    private int pacstudentIndex = 0; // target position
    private Vector2[] positions = new Vector2[]
    {
        new Vector2(-18.4f, 7.5f),   // top left
        new Vector2(-13.4f, 7.5f),   // top right
        new Vector2(-13.4f, 3.4f),   // bottom right
        new Vector2(-18.4f, 3.4f)    // bottom left
    };

    private float speed = 1f;
    private float t = 0f;

    void Start()
    {
        pacStudent.transform.position = positions[0]; // starting point
        t = 0f; // init t
    }

    void Update()
    {
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
}
