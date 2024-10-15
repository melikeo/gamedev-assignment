using UnityEngine;
using UnityEngine.UI;

public class BorderAnimator : MonoBehaviour
{
    public Sprite borderSprite1; //animated border input1
    public Sprite borderSprite2; //input 2
    public float framesPerSecond = 5f; //change speed between border sprites

    private Image imageComponent;
    private float timeBetweenFrames;
    private float timer;
    private bool usingFirstSprite = true;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.sprite = borderSprite1;  //start with first sprite

        //calc time between sprites
        timeBetweenFrames = 1f / framesPerSecond;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenFrames)
        {
            //changing sprites
            if (usingFirstSprite)
            {
                imageComponent.sprite = borderSprite2;
            }
            else
            {
                imageComponent.sprite = borderSprite1;
            }

            usingFirstSprite = !usingFirstSprite;

            //reset timer
            timer = 0f;
        }
    }
}
