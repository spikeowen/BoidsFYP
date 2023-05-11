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
    public float HuntAreaRadius { get; set; }

    public float SeparationWeight = 0.4f;
    public float AlignmentWeight = 0.6f;
    public float CohesionWeight = 0.0f;

    public Vector3 StartingPosition { get; set; }
    public Quaternion StartingRotation { get; set; }

    public void SimulateMovement(List<PredatorBehaviour> other, float time, List<BoidBehaviour> other2)
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

        //Boid priority to follow
        var priorityBoid = (BoidBehaviour)null;
        var priorityAngle = 180f;

        foreach (PredatorBehaviour predator in other)
        {
            foreach (BoidBehaviour boid in other2)
            {
                var preyDistance = Vector3.Distance(boid.transform.position, predator.transform.position);

                //if boid is within close proximity - ATTACK ONE
                if (preyDistance < HuntAreaRadius)
                {
                    //identify angle to boid
                    var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                    if (angle < priorityAngle && angle < 90f)
                    {
                        priorityBoid = boid;
                        priorityAngle = angle;
                    }
                }
                //if boid is generally nearby - HUNT SWARM
                else if (preyDistance < LocalAreaRadius)
                {
                    ////Goes in the swarm's direction
                    //alignmentDirection += boid.transform.forward;
                    //alignmentCount++;
                    ////Goes towards swarm's center
                    //cohesionDirection += boid.transform.position - transform.position;
                    //cohesionCount++;

                    //identify leading boid
                    var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                    if (angle < priorityAngle && angle < 90f)
                    {
                        priorityBoid = boid;
                        priorityAngle = angle;
                    }
                }
            }

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

        //Calculate average position dependant on the boid(s) that are close
        if (alignmentCount > 0)
            alignmentDirection /= alignmentCount;

        //Calculate average position dependant on the boid(s) that are close
        if (cohesionCount > 0)
            cohesionDirection /= cohesionCount;
        //Keep direction relative to predator's center
        cohesionDirection -= transform.position;

        //apply calculated directions, with weight to imapct which rule has more effect
        steering += separationDirection.normalized * SeparationWeight;
        steering += alignmentDirection.normalized * AlignmentWeight;
        steering += cohesionDirection.normalized * CohesionWeight;

        //follow close boid if there is one, or swarm leader otherwise
        if (priorityBoid != null)
            steering += (priorityBoid.transform.position - transform.position).normalized;

        //apply steering
        if (steering != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);

        //move 
        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        //Debug.Log("Eating");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, LocalAreaRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HuntAreaRadius);

    }
}