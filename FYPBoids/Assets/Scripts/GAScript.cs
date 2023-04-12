using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Chromosome
{
    public List<BoidBehaviour> boidGroup;
    public int swarmScore { get; set; }
}

public class GAScript
{
    //const int m_MutationRate = 10; //Percentage

    List<Chromosome> GAChromoVector;
    //public void GenerateChromoVec(int swarmNo)
    //{
    //    for (int i = 0; i < swarmNo; i++)
    //    {
    //        GAChromoVector.Add(GenerateChromo());
    //    }
    //}

    public Chromosome GenerateChromo()
    {
        Chromosome chromo = new Chromosome();
        chromo.boidGroup = new List<BoidBehaviour>();
        chromo.swarmScore = 0;
        return chromo;
    }

    public void SelectionPerSwarm(List<Chromosome> chromoVector)
    {
        GAChromoVector = chromoVector;
        for (int i = 0; i < chromoVector.Count; i++)
        {
            for (int j = 0; j < chromoVector[i].boidGroup.Count; j++)
            {
                bool isActive = chromoVector[i].boidGroup[j].gameObject.activeSelf;
                if (isActive == true)
                {
                    Chromosome temp = chromoVector[i];
                    temp.swarmScore++;
                    chromoVector[i] = temp;
                }
            }
        }

        List<Chromosome> bestChromos = new List<Chromosome>();
        Chromosome first = new Chromosome(), second = new Chromosome(), third = new Chromosome(), fourth = new Chromosome(), fifth = new Chromosome();

        for (int i = 0; i < chromoVector.Count; i++)
        {
            int scoreToCompare = chromoVector[i].swarmScore;
            if (scoreToCompare > first.swarmScore)
            {
                fifth = fourth;
                fourth = third;
                third = second;
                second = first;
                first = chromoVector[i];
            }
            else if (scoreToCompare > second.swarmScore)
            {
                fifth = fourth;
                fourth = third;
                third = second;
                second = chromoVector[i];
            }
            else if (scoreToCompare > third.swarmScore)
            {
                fifth = fourth;
                fourth = third;
                third = chromoVector[i];
            }
            else if (scoreToCompare > fourth.swarmScore)
            {
                fifth = fourth;
                fourth = chromoVector[i];
            }
            else if (scoreToCompare > fourth.swarmScore)
            {
                fifth = chromoVector[i];
            }
        }
    }
}
