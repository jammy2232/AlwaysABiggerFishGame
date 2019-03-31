using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour 
{

    public GameObject prefab;
    float timer = 0.0f;
    float time = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > time)
        {
            Instantiate(prefab).transform.position = transform.position + new Vector3(Random.Range(-100f, 100f), 0.0f, 0.0f);
            timer = 0.0f;
            time = Random.Range(0.3f, 1.0f);
        }        

    }

}
