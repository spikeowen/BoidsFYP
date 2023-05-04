using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Current STATUS: Program runs, boids don't reactivate and yet new gens have diff swarm scores
//Need to pass boids back to overwrite and reactivate boids in scene

public class SceneController : MonoBehaviour
{
    public BoidBehaviour BoidPrefab;

    public List<BoidBehaviour> boidList;
    //List to store swarms as chromos
    private List<Chromosome> chromoList;
    //Ticker to regulate what swarm the next boid belongs to
    private int swarmIterator = 0;

    public List<BoidBehaviour> newGenList;
    //C# Rule: You can't call a method defined inside a class without creating an instance of that class
    //(Unless you declare the method static)
    private GAScript GAInstance = new GAScript();
    private float simReset;

    [SerializeField]
    public bool tournamentMode = true;
    public bool rouletteMode = false;
    public bool spawnBySwarm = false;
    //Debug to see swarms better
    public bool randomMode = true;

    public int spawnBoids = 100;
    public float boidSpeed = 10f;
    public float boidSteeringSpeed = 100f;
    //How close before steering away from others/Swarm density/Vision distance
    public float boidNoClumpingArea = 10f;
    //Range of influence/Swarm Size/Pack Loyalty
    public float boidLocalArea = 10f;
    //Boid viewing area
    public float boidSimulationArea = 50f;
    //Number of swarms
    public int swarmCount = 10;
    //Fear of others/Clumping multiplier
    public float fearFactor = 1f;
    public float predatorFleeArea = 5f;
    //Timer to limit how long 1 simulation lasts for
    public float simTimer = 60.0f;
    public Text timerText;
    //Counter to know what generation the fish are on
    public int generationCount = 0;
    public Text generationText;

    private PredatorSceneController predatorSceneController;

    // Start is called before the first frame update
    void Start()
    {
        boidList = new List<BoidBehaviour>();
        chromoList = new List<Chromosome>();
        simReset = simTimer;

        //Generates Chromos for GA to use later
        for (int i = 0; i < swarmCount; i++)
        {
            chromoList.Add(GAInstance.GenerateChromo());
        }


        for (int i = 0; i < spawnBoids; i++)
        {
            SpawnBoid(BoidPrefab.gameObject, swarmIterator);
            swarmIterator++;
            if (swarmIterator > swarmCount - 1)
            {
                swarmIterator = 0;
            }
        }

        if (spawnBySwarm == true)
        {
            for (int i = 0; i < swarmCount; i++)
            {
                for (int j = 1; j < chromoList[i].boidGroup.Count; j++)
                {
                    chromoList[i].boidGroup[j] = chromoList[i].boidGroup[0];
                }
                //Debug.Log("Swarm Number: " + i.ToString());
                //Debug.Log("Speed: " + chromoList[i].boidGroup[0].Speed);
                //Debug.Log("Steering speed: " + chromoList[i].boidGroup[0].SteeringSpeed);
                //Debug.Log("Clumping: " + chromoList[i].boidGroup[0].NoClumpingRadius);
                //Debug.Log("Local Area: " + chromoList[i].boidGroup[0].LocalAreaRadius);
            }

            //Copy the overwritten chromos back into scene
            chromoList.ToList().ForEach(i => newGenList.AddRange(i.boidGroup));
            chromoList.Clear();
            //Recreate scene's chromo list and refill with updated boid sets
            for (int i = 0; i < swarmCount; i++)
            {
                chromoList.Add(GAInstance.GenerateChromo());
            }

            for (int i = 0; i < boidList.Count; i++)
            {
                boidList[i].gameObject.GetComponent<BoidBehaviour>().DeepCopy(newGenList[i]);
                boidList[i].gameObject.SetActive(true);
                chromoList[boidList[i].SwarmIndex].boidGroup.Add(boidList[i]);
            }

            newGenList.Clear();
        }

        predatorSceneController = GetComponent<PredatorSceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BoidBehaviour boid in boidList)
        {
            boid.SimulateMovement(boidList, Time.deltaTime, predatorSceneController.predatorList);

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

        simTimer -= Time.deltaTime;
        timerText.text = "Timer: " + simTimer.ToString();
        generationText.text = "Generation: " + generationCount.ToString();

        if (simTimer <= 0.0f)
        {
            //Run GA and reset
            if (tournamentMode == true)
            {
                if(generationCount == 0)
                {
                    GAInstance.RecordLine("-----TOURNAMENT SELECTION-----");
                }
                GAInstance.RecordLine("Generation Number: " + generationCount.ToString());
                GAInstance.TournamentSelection(chromoList);
                chromoList = GAInstance.Crossover(GAInstance.BestChromoTournament);
            }
            else if (rouletteMode == true)
            {
                if (generationCount == 0)
                {
                    GAInstance.RecordLine("-----ROULETTE SELECTION-----");
                }
                GAInstance.RouletteSelection(chromoList);
                chromoList = GAInstance.Crossover(GAInstance.BestChromoRoulette);
            }
            else
            {
                if (generationCount == 0)
                {
                    GAInstance.RecordLine("-----RANKED SELECTION-----");
                }
                GAInstance.RankedSelection(chromoList);
                chromoList = GAInstance.Crossover(GAInstance.BestChromoRank);
            }

            //Copy the new chromos back into scene
            chromoList.ToList().ForEach(i => newGenList.AddRange(i.boidGroup));
            chromoList.Clear();
            //Recreate scene's chromo list and refill with new generation ready for next cycle
            for (int i = 0; i < swarmCount; i++)
            {
                chromoList.Add(GAInstance.GenerateChromo());
            }

            for (int i = 0; i < boidList.Count; i++)
            {
                boidList[i].gameObject.GetComponent<BoidBehaviour>().DeepCopy(newGenList[i]);
                boidList[i].gameObject.SetActive(true);
                chromoList[boidList[i].SwarmIndex].boidGroup.Add(boidList[i]);
            }

            newGenList.Clear();
            generationCount++;
            simTimer = simReset;
            //Debug.Log(boidList.Count);
            //chromoList[swarmIndex].boidGroup.Add(boidBehaviour);
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
        //Limit number of swarms/Stop crash if 0
        if (swarmCount < 1)
            swarmCount = 1;
        else if (swarmCount > 10)
            swarmCount = 10;

        //Swarm index, sets what swarm it should belong to
        //Note to self: .Range is inclusive of minimum, exclusive of maximum
        if (randomMode == true)
        {
            boidBehaviour.SwarmIndex = swarmIndex;
            boidBehaviour.Speed = Random.Range(1, boidSpeed);
            boidBehaviour.SteeringSpeed = Random.Range(0, boidSteeringSpeed);
            boidBehaviour.LocalAreaRadius = Random.Range(0, boidLocalArea);

            boidBehaviour.NoClumpingRadius = boidNoClumpingArea;
            boidBehaviour.FearFactor = Random.Range(0.5f, fearFactor + 0.5f);
            boidBehaviour.PredatorFleeArea = Random.Range(0, predatorFleeArea);
        }
        else
        {
            boidBehaviour.SwarmIndex = swarmIndex;
            boidBehaviour.Speed = boidSpeed;
            boidBehaviour.SteeringSpeed = boidSteeringSpeed;
            boidBehaviour.LocalAreaRadius = boidLocalArea;

            boidBehaviour.NoClumpingRadius = boidNoClumpingArea;
            boidBehaviour.FearFactor = fearFactor;
            boidBehaviour.PredatorFleeArea = predatorFleeArea;
        }

        //Set to swarm's colour for visibility
        var boidRenderer = boidInstance.transform.GetChild(0).GetComponent<Renderer>();

        //Custom color using the Color constructor (RGBA)
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
        //SHOULD add new boid to a chromosome depending on swarmID
        chromoList[swarmIndex].boidGroup.Add(boidBehaviour);

    }
}


//SKYBOX: https://sketchfab.com/3d-models/underwater-skybox-14920ce3ffcc4cb79fb58738341f2c00#download