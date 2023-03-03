using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public BoidBehaviour BoidPrefab;
    [SerializeField]
    public int spawnBoids = 100;
    public float boidSpeed = 10f;
    public float boidSteeringSpeed = 100f;
    //How close before steering away from others/Swarm density
    public float boidNoClumpingArea = 10f;
    //Range of influence/Swarm Size
    public float boidLocalArea = 10f;
    //Boid viewing area
    public float boidSimulationArea = 50f;
    //Number of swarms
    public int swarmCount = 3;

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
        //Limit number of swarms
        if (swarmCount < 1)
            swarmCount = 1;
        else if (swarmCount > 10)
            swarmCount = 10;

        //Swarm index, set what swarm it should belong to and its stats
        //Note: .Range is inclusive of minimum, exclusive of maximum
        boidBehaviour.SwarmIndex = Random.Range(0, swarmCount);
        boidBehaviour.Speed = boidSpeed;
        boidBehaviour.SteeringSpeed = boidSteeringSpeed;
        boidBehaviour.LocalAreaRadius = boidLocalArea;
        boidBehaviour.NoClumpingRadius = boidNoClumpingArea;

        //Set to swarm's colour for visibility
        var boidRenderer = boidInstance.GetComponent<Renderer>();

        //Custom color using the Color constructor
        Color orange = new Color(1.0f, 0.55f, 0.0f, 1.0f);
        Color purple = new Color(0.6f, 0.0f, 1.0f, 1.0f);
        Color pink = new Color(1.0f, 0.0f, 0.78f, 1.0f);

        switch (boidBehaviour.SwarmIndex)
        {
            case (0):
                boidRenderer.material.SetColor("_Color", Color.red);
                break;
            case (1):
                boidRenderer.material.SetColor("_Color", Color.blue);
                break;
            case (2):
                boidRenderer.material.SetColor("_Color", Color.green);
                break;
            case (3):
                boidRenderer.material.SetColor("_Color", orange);
                break;
            case (4):
                boidRenderer.material.SetColor("_Color", purple);
                break;
            case (5):
                boidRenderer.material.SetColor("_Color", Color.cyan);
                break;
            case (6):
                boidRenderer.material.SetColor("_Color", Color.yellow);
                break;
            case (7):
                boidRenderer.material.SetColor("_Color", pink);
                break;
            case (8):
                boidRenderer.material.SetColor("_Color", Color.black);
                break;
            case (9):
                boidRenderer.material.SetColor("_Color", Color.grey);
                break;
            default:
                boidRenderer.material.SetColor("_Color", Color.white);
                break;
        }

            boidList.Add(boidBehaviour);
    }
}
