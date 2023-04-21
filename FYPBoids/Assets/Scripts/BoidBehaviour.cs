using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehaviour : MonoBehaviour
{
    [SerializeField]
    public int SwarmIndex { get; set; }
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }


    public float SeparationWeight = 0.5f;
    public float AlignmentWeight = 0.25f;
    public float CohesionWeight = 0.25f;
    /// <summary>
    /// The boid's alert level for danger/anxiety of being in a group
    /// </summary>
    public float FearFactor { get; set; }

    public void DeepCopy(BoidBehaviour other)
    {
        //NEEDS FIX
        SwarmIndex = other.SwarmIndex;
        Speed = other.Speed;
        SteeringSpeed = other.SteeringSpeed;
        NoClumpingRadius = other.NoClumpingRadius;
        LocalAreaRadius = other.LocalAreaRadius;
        FearFactor = other.FearFactor;
    }


    public void SimulateMovement(List<BoidBehaviour> other, float time)
    {
        //Steering vars
        var steering = Vector3.zero;

        //Separation vars and process
        Vector3 separationDirection = Vector3.zero;
        int separationCount = 0;

        //Alignment vars and process
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentCount = 0;

        //Cohesion vars and process
        Vector3 cohesionDirection = Vector3.zero;
        int cohesionCount = 0;

        //Boid leadership/priority to follow
        var leaderBoid = (BoidBehaviour)null;
        var leaderAngle = 180f;

        foreach (BoidBehaviour boid in other)
        {
            //skip itself
            if (boid == this)
                continue;

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);

            //if another boid is within close proximity - SEPARATION
            if (distance < NoClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            //if another boid is generally nearby - ALIGNMENT + COHESION
            if (distance < LocalAreaRadius * FearFactor && boid.SwarmIndex == this.SwarmIndex)
            {
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;

                //identify leading boid
                var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if (angle < leaderAngle && angle < 90f)
                {
                    leaderBoid = boid;
                    leaderAngle = angle;
                }
            }
        }

        //Calculate average position dependant on the boid(s) that are too close
        //Because if there's more than one, the direction will be too high, getting the average gives the direction that is away from ALL of them
        if (separationCount > 0)
            separationDirection /= separationCount;

        //flip and normalize direction
        separationDirection = -separationDirection.normalized;

        //Calculate average position dependant on the boid(s) that are too close
        if (alignmentCount > 0)
            alignmentDirection /= alignmentCount;

        //Calculate average position dependant on the boid(s) that are too close
        if (cohesionCount > 0)
            cohesionDirection /= cohesionCount;

        //Keep direction relative to boid's center
        cohesionDirection -= transform.position;

        //apply calculated directions, with weight to imapct which rule has more effect
        steering += separationDirection.normalized * SeparationWeight;
        steering += alignmentDirection.normalized * AlignmentWeight;
        steering += cohesionDirection.normalized * CohesionWeight;

        //follow leader if there is one
        if (leaderBoid != null)
            steering += (leaderBoid.transform.position - transform.position).normalized;

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