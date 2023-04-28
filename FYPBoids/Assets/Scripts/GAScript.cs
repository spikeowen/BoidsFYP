using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    //List<Chromosome> NewGenList;

    List<Chromosome> BestChromoTournament;
    List<Chromosome> BestChromoRank;

    public Chromosome GenerateChromo()
    {
        Chromosome chromo = new Chromosome();
        chromo.boidGroup = new List<BoidBehaviour>();
        chromo.swarmScore = 0;
        return chromo;
    }

    public void RecordLine(string entry)
    {
        string path = "Results.txt";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(entry);
        writer.Close();
        //StreamReader reader = new StreamReader(path);
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
    }

    public void TournamentSelection(List<Chromosome> chromoVector)
    {
        List<Chromosome> tempVector = new List<Chromosome>();
        tempVector = chromoVector;
        int score = 0;
        RecordLine("------------------------------");
        for (int i = 0; i < tempVector.Count; i++)
        {
            score = 0;
            for (int j = 0; j < tempVector[i].boidGroup.Count; j++)
            {
                bool isActive = tempVector[i].boidGroup[j].gameObject.activeSelf;
                if (isActive == true)
                {
                    score++;
                }
            }
            Chromosome temp = tempVector[i];
            temp.swarmScore = score;
            tempVector[i] = temp;
            string result = "Swarm No: " + i.ToString() + " Swarm Score: " + score.ToString();
            RecordLine(result);
            //Debug.Log("Swarm No: " + i + " Swarm Score: " + score);
        }

        List<Chromosome> bestChromos = new List<Chromosome>();
        int tScore1, tScore2, tScore3;

        for (int i = 0; i < tempVector.Count; i++)
        {
            int a = Random.Range(0, tempVector.Count);
            int b = Random.Range(0, tempVector.Count);
            int c = Random.Range(0, tempVector.Count);
            tScore1 = tempVector[a].swarmScore;
            tScore2 = tempVector[b].swarmScore;
            tScore3 = tempVector[c].swarmScore;

            if (tScore1 >= tScore2)
            {
                if (tScore1 >= tScore3)
                {
                    bestChromos.Add(tempVector[a]);
                }
                else
                {
                    bestChromos.Add(tempVector[c]);
                }
            }
            else if (tScore2 >= tScore3)
            {
                bestChromos.Add(tempVector[b]);
            }
            else
            {
                bestChromos.Add(tempVector[c]);
            }
        }
        BestChromoTournament = bestChromos;
    }

    public List<Chromosome> TournamentCrossover()
    {
        List<Chromosome> newChromoList = new List<Chromosome>();

        for (int i = 0; i < BestChromoTournament.Count; i+=2)
        {
            Chromosome parent1 = BestChromoTournament[i];
            Chromosome parent2 = BestChromoTournament[i + 1];
            Chromosome child1 = new Chromosome();
            Chromosome child2 = new Chromosome();
            child1.boidGroup = parent1.boidGroup;
            child2.boidGroup = parent2.boidGroup;

            for (int j = 0; j < parent1.boidGroup.Count; j++)
            {
                //Child 1 has speed and no clumping radius of P1
                child1.boidGroup[j].SteeringSpeed = parent2.boidGroup[j].SteeringSpeed;
                child1.boidGroup[j].LocalAreaRadius = parent2.boidGroup[j].LocalAreaRadius;
                //Child 2 has speed and no clumping radius of P2
                child2.boidGroup[j].SteeringSpeed = parent1.boidGroup[j].SteeringSpeed;
                child2.boidGroup[j].LocalAreaRadius = parent1.boidGroup[j].LocalAreaRadius;
            }
            //ADD MUTATION FUNC BEFORE ADDING TO LIST
            child1 = Mutation(child1);
            child2 = Mutation(child2);
            newChromoList.Add(child1);
            newChromoList.Add(child2);
            //CHILDREN HAVE WRONG SWARM INDEX BUT DOESN'T MATTER
            //Debug.Log("Child1 Swarm: " + child1.boidGroup[0].SwarmIndex);
            //Debug.Log("Child2 Swarm: " + child2.boidGroup[0].SwarmIndex);
        }

        return newChromoList;
    }

    public Chromosome Mutation(Chromosome newChromo)
    {
        Chromosome child = newChromo;
        //Rate is a percentage
        int mutationRate = 10;
        for (int i = 0; i < child.boidGroup.Count; i++)
        {
            int chance = Random.Range(0, 10);//Result = 0-9
            if (chance < mutationRate / 10)
            {
                bool changed = false;

                bool speedChanged = true;
                bool steeringChanged = true;
                bool clumpChanged = true;
                bool localChanged = true;
                bool fearChanged = true;

                while (changed == false)
                {
                    int random = Random.Range(0, 5);//Result = 0-4
                    switch (random)
                    {
                        case 0://Speed (Not above 10, while pred at 11)
                            {
                                if (child.boidGroup[i].Speed <= 10)
                                {
                                    child.boidGroup[i].Speed += 0.2f;
                                    changed = true;
                                }
                                else
                                    speedChanged = false;
                                break;
                            }
                        case 1://SteeringSpeed (Turning Speed- limit 100.0f)
                            {
                                if (child.boidGroup[i].SteeringSpeed <= 100.0f)
                                {
                                    child.boidGroup[i].SteeringSpeed += 1.0f;
                                }
                                else
                                    steeringChanged = false;
                                break;
                            }
                        case 2://NoClumpingRadius (Closeness to other boids- limit 10f)
                            {
                                if (child.boidGroup[i].NoClumpingRadius <= 10.0f)
                                {
                                    child.boidGroup[i].NoClumpingRadius += 0.1f;
                                    changed = true;
                                }
                                else
                                    clumpChanged = false;
                                break;
                            }
                        case 3://LocalAreaRadius (Range of vision- limit 25f)
                            {
                                if (child.boidGroup[i].LocalAreaRadius <= 25.0f)
                                {
                                    child.boidGroup[i].LocalAreaRadius += 0.5f;
                                    changed = true;
                                }
                                else
                                    localChanged = false;
                                break;
                            }
                        case 4://FearFactor (Probably don't go above 3 as is multiplier)
                            {
                                if (child.boidGroup[i].FearFactor <= 3.0f)
                                {
                                    child.boidGroup[i].FearFactor += 0.1f;
                                    changed = true;
                                }
                                else
                                    fearChanged = false;
                                break;
                            }
                    }

                    if (speedChanged == false && steeringChanged == false && clumpChanged == false && localChanged == false && fearChanged == false)
                    {
                        changed = true;
                    }
                }
            }
        }
        return child;
    }

}
