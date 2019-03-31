using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour, IFish
{

    // Events to inform the game manager of your state
    // I have died show the button to restart todo

    // Fish Controls
    [Header("Fish Game Settings")]
    [SerializeField] float speed = 5.0f;
    [SerializeField] float drag = 0.9f;
    [SerializeField] float gravity = 1.0f;
    [SerializeField] float foodSizeIncrease = 0.1f;
    [SerializeField] float fishSizeGainProportion = 0.3f;
    [SerializeField] Sprite deadFish;
    [SerializeField] float angleOfTip = 15.0f;
    [SerializeField] SpriteRenderer render = null;

    [Header("Fish Animation Settings")]
    [SerializeField] float growthRate = 0.1f;
    [SerializeField] int splits = 20;

    // Handle the movement 
    Vector3 m_MovementVector = new Vector3(0.0f, 0.0f, 0.0f);
    Quaternion angle = new Quaternion();

    // Camera reference 
    Camera camera;

    // life or death
    bool alive = true;

    private void Start()
    {
        camera = Camera.main;
    }

    // Get your fishSize
    public float GetSize()
    {
        return transform.localScale.x;
    }

    public Fish.TYPE GetType()
    {
        return Fish.TYPE.PLAYER;
    }

    public void Eaten()
    {
        // You are dead 
        alive = false;
        render.sprite = deadFish;
    }

    // Apply some basic physics and movement 
    private void FixedUpdate()
    {

        // Variables 
        Vector3 m_CurrentTarget = new Vector3(0.0f, 0.0f, 0.0f);

        // Check if the screen is being touched 
        if ((Input.touchCount != 0 || Input.GetMouseButton(0)) && alive)
        {

            // Get the position to move towards 
            // Vector2 screenPosition = Input.touches[0].position;
            Vector2 screenPosition = Input.mousePosition;

            // I need to check this position and use it to create a movement vector 

            // Horizontal 
            if (screenPosition.x > (Screen.width / 2))
            {
                m_CurrentTarget.x = (screenPosition.x / (Screen.width)) - 0.5f + transform.position.x;
                render.flipX = false;
            }
            else
            {
                m_CurrentTarget.x = -(screenPosition.x / (Screen.width)) + transform.position.x;
                render.flipX = true;
            }

            // Vertical 
            if (screenPosition.y > (Screen.height / 2))
            {
                m_CurrentTarget.y = (screenPosition.y / (Screen.height)) - 0.5f + transform.position.y;
                // set the angle to +
                if(render.flipX)
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, -angleOfTip);
                else
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, angleOfTip);
            }
            else
            {
                m_CurrentTarget.y = -(screenPosition.y / (Screen.width)) + transform.position.y;
                // set the angle to +
                if (render.flipX)
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, angleOfTip);
                else
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, -angleOfTip);
            }

            // Apply your new direction 
            if(m_MovementVector.sqrMagnitude < speed * speed)
                m_MovementVector += (m_CurrentTarget - transform.position).normalized * speed;

            // apply the rotation 
            render.gameObject.transform.rotation = angle;

            // Apply that speed
            transform.position += m_MovementVector * Time.deltaTime;

        }
        else
        {
            // Apply the drag
            m_MovementVector *= drag;
            // Apply that position and if not sink
            m_MovementVector += new Vector3(0.0f, -gravity * Time.deltaTime, 0.0f);
            // Apply that speed
            transform.position += m_MovementVector * Time.deltaTime;
            // set the angle to 0
            angle.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            render.gameObject.transform.rotation = angle;

        }

    }

    // Colliision 
    private void OnTriggerEnter2D(Collider2D collision)
    {


        // Check what it is 
        switch(collision.gameObject.tag)
        {


            case "LevelLimits":

                // Invert your speed 
                m_MovementVector = -m_MovementVector * 2.0f;

                break;

            case "Food":

                // Eat the food and grow
                StartCoroutine(GrowUP(foodSizeIncrease));

                break;

            case "Fish":

                IFish otherFish = collision.gameObject.GetComponent<IFish>();

                // Check which is bigger
                if (otherFish.GetSize() < transform.localScale.x)
                {
                    // Eat them 
                    StartCoroutine(GrowUP(otherFish.GetSize() * fishSizeGainProportion));
                    // Kill the other fish 
                    collision.gameObject.GetComponent<IFish>().Eaten();
                }
    
                break;

            case "Dead":

                if (!alive)
                    Destroy(this);

                break;

            case "VisionCone":

                break;

        }

    }

    // Coroutine to increase the size of the fish 
    IEnumerator GrowUP(float amount)
    {

        // split the growth to steps 
        float SizeIncrease = amount / (float)splits;

        // Grown the fish
        for(int i = 0; i < splits; i++)
        {
            Vector3 scale = transform.localScale;
            scale.x += SizeIncrease;
            scale.y += SizeIncrease;
            transform.localScale = scale;
            yield return new WaitForSeconds(growthRate);
        }

        // Update the camera
        for (int i = 0; i < splits; i++)
        {
            camera.orthographicSize += SizeIncrease;
            yield return new WaitForSeconds(growthRate);
        }

    }

}
