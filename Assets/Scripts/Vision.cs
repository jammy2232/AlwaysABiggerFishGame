using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour 
{

    // Reference to Fish Brain
    [SerializeField] Fish brain;

    // Add and remove items as they are spoted 
    private void OnTriggerEnter2D(Collider2D other)
    {
        brain.SomethingSpotted(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        brain.SomethingLost(other.gameObject);
    }

}
