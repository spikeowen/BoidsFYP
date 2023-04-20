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

    //List<Chromosome> GAChromoVector;
    //public void GenerateChromoVec(int swarmNo)
    //{
    //    for (int i = 0; i < swarmNo; i++)
    //    {
    //        GAChromoVector.Add(GenerateChromo());
    //    }
    //}

    List<Chromosome> BestChromoTournament;
    List<Chromosome> BestChromoRank;

    public Chromosome GenerateChromo()
    {
        Chromosome chromo = new Chromosome();
        chromo.boidGroup = new List<BoidBehaviour>();
        chromo.swarmScore = 0;
        return chromo;
    }

    public void RankedSelection(List<Chromosome> chromoVector)
    {
        int score;
        for (int i = 0; i < chromoVector.Count; i++)
        {
            score = 0;
            for (int j = 0; j < chromoVector[i].boidGroup.Count; j++)
            {
                bool isActive = chromoVector[i].boidGroup[j].gameObject.activeSelf;
                if (isActive == true)
                {
                    score++;
                }
            }
            Chromosome temp = chromoVector[i];
            temp.swarmScore = score;
            chromoVector[i] = temp;
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

        if (bestChromos.Count < 1)
        {
            bestChromos.Add(first);
            bestChromos.Add(second);
            bestChromos.Add(third);
            bestChromos.Add(fourth);
            bestChromos.Add(fifth);
        }
        else
        {
            bestChromos[0] = first;
            bestChromos[1] = second;
            bestChromos[2] = third;
            bestChromos[3] = fourth;
            bestChromos[4] = fifth;
        }

        //if (BestChromoVector.Count < 1)
        //{
        //    BestChromoVector.Add(first);
        //    BestChromoVector.Add(second);
        //    BestChromoVector.Add(third);
        //    BestChromoVector.Add(fourth);
        //    BestChromoVector.Add(fifth);
        //}
        //else
        //{
        //    BestChromoVector[0] = first;
        //    BestChromoVector[1] = second;
        //    BestChromoVector[2] = third;
        //    BestChromoVector[3] = fourth;
        //    BestChromoVector[4] = fifth;
        //}
    }

    public void TournamentSelection(List<Chromosome> chromoVector)
    {
        int score;
        for (int i = 0; i < chromoVector.Count; i++)
        {
            score = 0;
            for (int j = 0; j < chromoVector[i].boidGroup.Count; j++)
            {
                bool isActive = chromoVector[i].boidGroup[j].gameObject.activeSelf;
                if (isActive == true)
                {
                    score++;
                }
            }
            Chromosome temp = chromoVector[i];
            temp.swarmScore = score;
            chromoVector[i] = temp;
        }

        List<Chromosome> bestChromos = new List<Chromosome>();
        int tScore1, tScore2, tScore3;

        for (int i = 0; i < chromoVector.Count / 2; i++)
        {
            int a = Random.Range(0, chromoVector.Count);
            int b = Random.Range(0, chromoVector.Count);
            int c = Random.Range(0, chromoVector.Count);
            tScore1 = chromoVector[a].swarmScore;
            tScore2 = chromoVector[b].swarmScore;
            tScore3 = chromoVector[c].swarmScore;

            if (tScore1 >= tScore2)
            {
                if (tScore1 >= tScore3)
                {
                    bestChromos.Add(chromoVector[a]);
                }
                else
                {
                    bestChromos.Add(chromoVector[c]);
                }
            }
            else if (tScore2 >= tScore3)
            {
                bestChromos.Add(chromoVector[b]);
            }
            else
            {
                bestChromos.Add(chromoVector[c]);
            }
        }
        BestChromoTournament = bestChromos;
    }
}


