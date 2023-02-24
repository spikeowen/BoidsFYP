using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehaviour : MonoBehaviour
{
    public int SwarmIndex { get; set; }
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }

    public void SimulateMovement(List<BoidBehaviour> other, float time)
    {
        //Steering vars
        var steering = Vector3.zero;

        //Separation vars and process
        Vector3 separationDirection = Vector3.zero;
        int separationCount = 0;
        foreach (BoidBehaviour boid in other)
        {
            //skip self
            if (boid == this)
                continue;

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);

            //if another boid is within close proximity
            if (distance < NoClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }
        }

        //Calculate average position dependant on the boid(s) that are too close
        //Because if there's more than one, the direction will be too high, getting the average gives the direction that is away from ALL of them
        if (separationCount > 0)
            separationDirection /= separationCount;

        //flip and normalize
        separationDirection = -separationDirection.normalized;

        //apply direction to steering
        steering = separationDirection;

        //apply steering
        if (steering != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);

        //move 
        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }
}


// Research Links in case
//https://blog.yarsalabs.com/flock-simulation-using-boids-in-unity/#:~:text=Boid%20simulation%20is%20the%20simulation,controlled%20by%20a%20single%20creature.
//https://dawn-studio.de/tutorials/boids/