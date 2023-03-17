using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorSceneController : MonoBehaviour
{
    public PredatorBehaviour PredatorPrefab;
    [SerializeField]
    //How many predators to spawn
    public int spawnPredators = 1;
    public float predatorSpeed = 11f;
    public float predatorSteeringSpeed = 80f;
    //How close before steering away from others/Swarm density/Vision distance
    public float predatorNoClumpingArea = 10f;
    //Range of influence/Swarm Size/Pack Loyalty
    public float predatorLocalArea = 20f;
    //predator viewing area
    public float predatorSimulationArea = 50f;
    //Area for focusing one boid
    public float predatorHuntArea = 10f;

    public List<PredatorBehaviour> predatorList;

    private SceneController boidSceneController;

    // Start is called before the first frame update
    void Start()
    {
        predatorList = new List<PredatorBehaviour>();

        for (int i = 0; i < spawnPredators; i++)
        {
            SpawnPredator(PredatorPrefab.gameObject);
        }

        boidSceneController = GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PredatorBehaviour predator in predatorList)
        {
            predator.SimulateMovement(predatorList, Time.deltaTime, boidSceneController.boidList);

            //Keeps predators within set area, and loops them to the other side like pacman
            var predatorPos = predator.transform.position;

            if (predatorPos.x > predatorSimulationArea)
                predatorPos.x -= predatorSimulationArea * 2;
            else if (predatorPos.x < -predatorSimulationArea)
                predatorPos.x += predatorSimulationArea * 2;

            if (predatorPos.y > predatorSimulationArea)
                predatorPos.y -= predatorSimulationArea * 2;
            else if (predatorPos.y < -predatorSimulationArea)
                predatorPos.y += predatorSimulationArea * 2;

            if (predatorPos.z > predatorSimulationArea)
                predatorPos.z -= predatorSimulationArea * 2;
            else if (predatorPos.z < -predatorSimulationArea)
                predatorPos.z += predatorSimulationArea * 2;

            predator.transform.position = predatorPos;
        }
    }

    private void SpawnPredator(GameObject prefab)
    {
        var predatorInstance = Instantiate(prefab);
        //Randomize spawn location
        predatorInstance.transform.localPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        //Randomize spawn rotation
        predatorInstance.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        var predatorBehaviour = predatorInstance.GetComponent<PredatorBehaviour>();


        //Note: .Range is inclusive of minimum, exclusive of maximum
        predatorBehaviour.Speed = predatorSpeed;
        predatorBehaviour.SteeringSpeed = predatorSteeringSpeed;
        predatorBehaviour.LocalAreaRadius = predatorLocalArea;
        predatorBehaviour.NoClumpingRadius = predatorNoClumpingArea;
        predatorBehaviour.HuntAreaRadius = predatorHuntArea;

        predatorList.Add(predatorBehaviour);
    }
}