using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Chromosome
{
    public List<BoidBehaviour> boidGroup;
}

public class GAScript
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

    public void SelectionPerSwarm(List<Chromosome> chromoVector)
    {
        for (int i = 0; i < chromoVector.Count; i++)
        {
            for (int j = 0; j < chromoVector[i].boidGroup.Count; j++)
            {
                bool isActive = chromoVector[i].boidGroup[i].gameObject.activeSelf;
            }
        }
    }
}
