using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehaviour : MonoBehaviour
{
    [SerializeField]
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }


    public float SeparationWeight = 1.0f;
    //public float AlignmentWeight = 0.25f;
    //public float CohesionWeight = 0.25f;

    public void SimulateMovement(List<PredatorBehaviour> other, float time)
    {
        //Steering vars
        var steering = Vector3.zero;

        //Separation vars and process
        Vector3 separationDirection = Vector3.zero;
        int separationCount = 0;

        //Boid leadership/priority to follow
        var leaderBoid = (BoidBehaviour)null;
        var leaderAngle = 180f;

        foreach (PredatorBehaviour predator in other)
        {
            //skip itself
            if (predator == this)
                continue;

            var distance = Vector3.Distance(predator.transform.position, this.transform.position);

            //if another predator is within close proximity - SEPARATION
            if (distance < NoClumpingRadius)
            {
                separationDirection += predator.transform.position - transform.position;
                separationCount++;
            }

        }

        //Calculate average position dependant on the predator(s) that are too close
        //Because if there's more than one, the direction will be too high, getting the average gives the direction that is away from ALL of them
        if (separationCount > 0)
            separationDirection /= separationCount;

        //flip and normalize direction
        separationDirection = -separationDirection.normalized;

        //apply calculated directions, with weight to imapct which rule has more effect
        steering += separationDirection.normalized * SeparationWeight;

        //apply steering
        if (steering != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);

        //move 
        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }
}