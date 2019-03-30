using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour, IFish
{

    // Fish Controls
    [SerializeField] float speed = 5.0f;
    [SerializeField] float gravity = 1.0f;

    // Controls the size of the fish as it grows
    float deltaSize = 0.0f;

    // Here we want setup the fish and start 
    private void Start()
    {
        



    }

    // Get your fishSize
    public float GetSize()
    {
        return transform.lossyScale.x;
    }

    // Apply some basic physics and movement 
    private void Update()
    {

        // Variables 
        Vector3 m_CurrentTarget = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 m_MovementVector = new Vector3(0.0f, 0.0f, 0.0f);

        // Check if the screen is being touched 
        if (Input.touchCount != 0)
        {

            // Get the position to move towards 
            Vector2 screenPosition = Input.touches[0].position;

            // I need to check this position and use it to create a movement vector 

            // Horizontal 
            if (screenPosition.x > (Screen.width / 2))
            {
                m_CurrentTarget.x = (screenPosition.x / (Screen.width * 2.0f)) - 0.5f + transform.position.x;
            }
            else
            {
                m_CurrentTarget.x = -(screenPosition.x / (Screen.width * 2.0f)) + transform.position.x;
            }

            // Vertical 
            if (screenPosition.y > (Screen.width / 2))
            {
                m_CurrentTarget.y = (screenPosition.y / (Screen.height * 2.0f)) - 0.5f + transform.position.y;
            }
            else
            {
                m_CurrentTarget.x = -(screenPosition.y / (Screen.width * 2.0f)) + transform.position.y;
            }

            // Apply your new direction 
            m_MovementVector = (m_CurrentTarget - transform.position).normalized * speed;

            // Apply that speed
            transform.position += m_MovementVector * Time.deltaTime;

        }
        else
        {
            // Apply that position and if not sink
            transform.position += new Vector3(0.0f, -gravity * Time.deltaTime, 0.0f);

        }

    }





}
