using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Fish : MonoBehaviour, IFish
{

    public enum TYPE { Type1, Type2, Type3, Type4, Type5, PLAYER }
    [SerializeField] TYPE type = TYPE.Type1;

    enum STATE { NORMAL, FLEE, ATTACK, BREED }
    [SerializeField] STATE currentState = STATE.NORMAL;

    // What this fish cares about
    Transform target;
    Vector3 staticPosition = new Vector3(0.0f, 0.0f, 0.0f);
    float timer = 0.0f;

    [Header("Fish Game Settings")]
    [SerializeField] float drag = 0.9f;
    [SerializeField] float gravity = 1.0f;
    [SerializeField] float foodSizeIncrease = 0.05f;
    [SerializeField] float fishSizeGainProportion = 0.1f;
    [SerializeField] SpriteRenderer render = null;
    [SerializeField] Sprite deadFish;
    [SerializeField] float angleOfTip = 15.0f;

    [Header("Fish Animation Settings")]
    [SerializeField] float growthRate = 0.1f;
    [SerializeField] int splits = 20;

    [Header("Fish Animation Settings")]
    [SerializeField] float timeBetweenBreeding = 15.0f;

    // Handle the movement 
    Vector3 m_MovementVector = new Vector3(0.0f, 0.0f, 0.0f);
    Quaternion angle = new Quaternion();
    bool alive = true;
    bool ableToBreed = false;
    private float m_speed;

    private void Start()
    {
        m_speed = Random.Range(0.8f, 1.2f);
        float size = Random.Range(0.8f, 1.2f);
        transform.localScale = new Vector3(size, size, 0.0f);
    }

    // Call to setup the fish
    public void Setup(float initSize, float initSpeed, Vector3 initPosition)
    {
        transform.localScale = new Vector3(initSize, initSize, 0.0f);
        m_speed = initSpeed;
        transform.position = initPosition;
    }

    public float GetCurrentSpeed()
    {
        return m_speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (alive)
        {

            timer += Time.deltaTime;

            if (timer > timeBetweenBreeding)
            {
                timer = 0.0f;
                ableToBreed = true;
            }

            // State machine
            switch (currentState)
            {
                case STATE.NORMAL:
                    // Follwow a path left to right randomly moving up or down
                    Normal();
                    break;
                case STATE.BREED:
                    Breed();
                    // Move toward mate and spawn new fish
                    break;
                case STATE.FLEE:
                    // Run away from bigger fish
                    Flee();
                    break;
                case STATE.ATTACK:
                    // Move to fish until out of range or caught
                    Attack();
                    break;
            }

        }

        // Move the fish 
        Move();

    }

    // Check with something is spotted 
    public void SomethingSpotted(GameObject spotted)
    {
        // State machine
        switch (spotted.tag)
        {
            case "LevelLimits":
                target = spotted.transform;
                currentState = STATE.FLEE;
                break;

            case "Food":
                target = spotted.transform;
                currentState = STATE.ATTACK;
                break;

            case "Fish":

                IFish otherFish = spotted.gameObject.GetComponent<IFish>();

                if (otherFish.GetType() == type)
                {
                    if (ableToBreed)
                    {
                        target = spotted.transform;
                        currentState = STATE.BREED;
                    }
                    else
                    {
                        staticPosition = transform.position;
                        currentState = STATE.NORMAL;
                    }
                }
                else
                {
                    // Check which is bigger
                    if (otherFish.GetSize() < transform.localScale.x)
                    {
                        // Eat them 
                        target = spotted.transform;
                        currentState = STATE.ATTACK;
                    }
                    else
                    {
                        target = spotted.transform;
                        currentState = STATE.FLEE;
                    }
                }

                break;

        }
    }

    // Check with something is spotted 
    public void SomethingLost(GameObject lost)
    {
        // State machine
        switch (lost.tag)
        {
            case "Food":
                target = null;
                staticPosition = transform.position;
                currentState = STATE.NORMAL;
                break;

            case "Fish":
                target = null;
                staticPosition = transform.position;
                currentState = STATE.NORMAL;
                break;

        }
    }

    // IFish requirement
    public float GetSize()
    {
        return transform.localScale.x;
    }

    public TYPE GetType()
    {
        return type;
    }

    public void Eaten()
    {
        Die();
    }

    // Coroutine to increase the size of the fish 
    IEnumerator GrowUP(float amount)
    {

        // split the growth to steps 
        float SizeIncrease = amount / (float)splits;

        // Grown the fish
        for (int i = 0; i < splits; i++)
        {
            Vector3 scale = transform.localScale;
            scale.x += SizeIncrease;
            scale.y += SizeIncrease;
            transform.localScale = scale;
            yield return new WaitForSeconds(growthRate);
        }

    }

    // State machine 
    void Move()
    {

        if (alive)
        {

            // Horizontal 
            if (m_MovementVector.x > 0.0f)
            {
                render.flipX = false;
            }
            else
            {
                render.flipX = true;
            }

            // Vertical 
            if (m_MovementVector.y > 0.0f)
            {
                // set the angle to +
                if (render.flipX)
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, -angleOfTip);
                else
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, angleOfTip);
            }
            else
            {
                // set the angle to +
                if (render.flipX)
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, angleOfTip);
                else
                    angle.eulerAngles = new Vector3(0.0f, 0.0f, -angleOfTip);
            }

            // Apply your new direction 
            m_MovementVector *= m_speed;

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

    void Die()
    {
        // You are dead 
        alive = false;
        render.sprite = deadFish;
    }

    void Normal()
    {
        // If your close to target pick another one
        if ((staticPosition - transform.position).sqrMagnitude < 0.01f)
        {
            staticPosition += new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0.0f);
        }
        m_MovementVector = (staticPosition - transform.position).normalized;
    }

    void Breed()
    {

        m_MovementVector = (target.position - transform.position).normalized;

        // If your close to target pick another one
        if ((target.position - transform.position).sqrMagnitude < 0.01f)
        {
            // 50% change of creating a baby
            if (Random.Range(0.0f, 100.0f) > 50.0f)
            {
                // Create a baby 
                GameObject baby = Instantiate(this.gameObject);
                baby.GetComponent<Fish>().Setup(1.0f, Random.Range(m_speed, target.gameObject.GetComponent<Fish>().GetCurrentSpeed()), transform.position);
            }

            // reset
            ableToBreed = false;
            timer = 0.0f;
            staticPosition = transform.position;
            currentState = STATE.NORMAL;

        }

    }

    void Flee()
    {

        if (target == null)
        {
            staticPosition = transform.position;
            currentState = STATE.NORMAL;
            return;
        }

        m_MovementVector = (transform.position - target.position).normalized;
    }

    void Attack()
    {
        if(target == null)
        {
            staticPosition = transform.position;
            currentState = STATE.NORMAL;
            return;
        }

        m_MovementVector = (target.position - transform.position).normalized;
    }

    // Colliision 
    private void OnTriggerEnter2D(Collider2D collision)
    {


        // Check what it is 
        switch (collision.gameObject.tag)
        {

            case "LevelLimits":

                // Invert your speed 
                m_MovementVector = -m_MovementVector * 2.0f;
                staticPosition += 1 * m_MovementVector;

                break;

            case "Food":

                // Eat the food and grow
                StartCoroutine(GrowUP(foodSizeIncrease));

                break;

            case "Fish":

                IFish otherFish = collision.gameObject.GetComponent<IFish>();

                if (otherFish.GetType() == type)
                    return;

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

        }

    }

}
