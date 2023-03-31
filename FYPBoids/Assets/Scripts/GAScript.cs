using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Chromosome
{
    float speed;
    float steeringSpeed;
    float noClumpingArea;
    float localArea;
    float fearFactor;

    public List<BoidBehaviour> boidGroup;
}

public class GAScript : MonoBehaviour
{
    //const int m_MutationRate = 10; //Percentage

    List<Chromosome> chromoVector;

    public Chromosome GenerateChromo()
    {
        Chromosome chromo = new Chromosome();
        chromo.boidGroup = new List<BoidBehaviour>();
        return chromo;
    }

    public void GenerateChromoVec(int swarmNo)
    {
        for (int i = 0; i < swarmNo; i++)
        {
            chromoVector.Add(GenerateChromo());
        }
    }

   // vector<Chromosome> GetChromoVec() { return m_ChromoVector; }
}
