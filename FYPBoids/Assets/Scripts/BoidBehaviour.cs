using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehaviour : MonoBehaviour
{
    Vector3 position;
    GameObject gameObject;

    float speed;
    Vector3 direction;
    public List<BoidBehaviour> boidsInScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        transform.Translate(direction * (speed * Time.deltaTime));
    }
}

//https://blog.yarsalabs.com/flock-simulation-using-boids-in-unity/#:~:text=Boid%20simulation%20is%20the%20simulation,controlled%20by%20a%20single%20creature. Research Link