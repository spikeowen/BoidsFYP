using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public BoidBehaviour BoidPrefab;

    public int spawnBoids = 100;
    public float boidSpeed = 10f;
    public float boidSteeringSpeed = 100f;

    //How close before steering away from others/Swarm density
    public float boidNoClumpingArea = 10f;
    //Range of influence/Swarm Size
    public float boidLocalArea = 10f;

    //Boid viewing area
    public float boidSimulationArea = 50f;

    private List<BoidBehaviour> boidList;

    // Start is called before the first frame update
    void Start()
    {
        boidList = new List<BoidBehaviour>();

        for (int i = 0; i < spawnBoids; i++)
        {
            SpawnBoid(BoidPrefab.gameObject, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BoidBehaviour boid in boidList)
        {
            boid.SimulateMovement(boidList, Time.deltaTime);

            //Keeps boids within set area, and loops them to the other side like pacman
            var boidPos = boid.transform.position;

            if (boidPos.x > boidSimulationArea)
                boidPos.x -= boidSimulationArea * 2;
            else if (boidPos.x < -boidSimulationArea)
                boidPos.x += boidSimulationArea * 2;

            if (boidPos.y > boidSimulationArea)
                boidPos.y -= boidSimulationArea * 2;
            else if (boidPos.y < -boidSimulationArea)
                boidPos.y += boidSimulationArea * 2;

            if (boidPos.z > boidSimulationArea)
                boidPos.z -= boidSimulationArea * 2;
            else if (boidPos.z < -boidSimulationArea)
                boidPos.z += boidSimulationArea * 2;

            boid.transform.position = boidPos;
        }
    }

    private void SpawnBoid(GameObject prefab, int swarmIndex)
    {
        var boidInstance = Instantiate(prefab);
        //Randomize spawn location
        boidInstance.transform.localPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        //Randomize spawn rotation
        boidInstance.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        var boidBehaviour = boidInstance.GetComponent<BoidBehaviour>();
        //Swarm index, set what swarm it should belong to
        boidBehaviour.SwarmIndex = Random.Range(0, 2);
        boidBehaviour.Speed = boidSpeed;
        boidBehaviour.SteeringSpeed = boidSteeringSpeed;
        boidBehaviour.LocalAreaRadius = boidLocalArea;
        boidBehaviour.NoClumpingRadius = boidNoClumpingArea;

        boidList.Add(boidBehaviour);
    }
}
