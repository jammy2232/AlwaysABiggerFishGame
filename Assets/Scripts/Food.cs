using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour 
{

    public float gravity = 0.5f;

    private void Start()
    {
        gravity = Random.Range(0.25f, 2.0f);
    }

    // Update is called once per frame
    void FixedUpdate () 
    {
        // Apply that position and if not sink
        transform.position += new Vector3(0.0f, -gravity * Time.deltaTime, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check what it is 
        switch (collision.gameObject.tag)
        {

            case "Fish":

                Destroy(this.gameObject);

                break;

            case "Dead":

                Destroy(this.gameObject);

                break;
        }

    }

}
