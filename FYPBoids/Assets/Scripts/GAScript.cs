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

    public List<Chromosome> BestChromoTournament;
    public List<Chromosome> BestChromoRank;
    public List<Chromosome> BestChromoRoulette;

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
        List<Chromosome> tempVector = new List<Chromosome>();
        tempVector = GetScores(chromoVector);

        List<Chromosome> bestChromos = new List<Chromosome>();
        Chromosome first = new Chromosome(), second = new Chromosome(), third = new Chromosome(), fourth = new Chromosome(), fifth = new Chromosome();

        for (int i = 0; i < tempVector.Count; i++)
        {
            int scoreToCompare = tempVector[i].swarmScore;
            if (scoreToCompare > first.swarmScore)
            {
                fifth = fourth;
                fourth = third;
                third = second;
                second = first;
                first = tempVector[i];
            }
            else if (scoreToCompare > second.swarmScore)
            {
                fifth = fourth;
                fourth = third;
                third = second;
                second = tempVector[i];
            }
            else if (scoreToCompare > third.swarmScore)
            {
                fifth = fourth;
                fourth = third;
                third = tempVector[i];
            }
            else if (scoreToCompare > fourth.swarmScore)
            {
                fifth = fourth;
                fourth = tempVector[i];
            }
            else if (scoreToCompare > fifth.swarmScore)
            {
                fifth = tempVector[i];
            }
        }

        bestChromos.Add(first);
        bestChromos.Add(second);
        bestChromos.Add(third);
        bestChromos.Add(fourth);
        bestChromos.Add(fifth);
        bestChromos.Add(first);
        bestChromos.Add(second);
        bestChromos.Add(third);
        bestChromos.Add(fourth);
        bestChromos.Add(fifth);

        //Stops the need to clear and reset lists every time
        BestChromoRank = bestChromos;
    }

    public void TournamentSelection(List<Chromosome> chromoVector)
    {
        List<Chromosome> tempVector = new List<Chromosome>();
        tempVector = GetScores(chromoVector);

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
        //Stops the need to clear and reset lists every time
        BestChromoTournament = bestChromos;
    }

    public List<Chromosome> Crossover(List<Chromosome> bestList)
    {
        List<Chromosome> newChromoList = new List<Chromosome>();

        for (int i = 0; i < bestList.Count; i += 2)
        {
            Chromosome parent1;
            Chromosome parent2;
            //IN CASE ODD NUMBER OF SWARMS
            if (i + 1 > bestList.Count)
            {
                parent1 = bestList[i];
                parent2 = bestList[i - 1];
            }
            else
            {
                parent1 = bestList[i];
                parent2 = bestList[i + 1];
            }
            Chromosome child1 = new Chromosome();
            Chromosome child2 = new Chromosome();
            child1.boidGroup = parent1.boidGroup;
            child2.boidGroup = parent2.boidGroup;

            for (int j = 0; j < parent1.boidGroup.Count; j++)
            {
                //Child 1 has speed, fear factor and no clumping radius of P1
                child1.boidGroup[j].SteeringSpeed = parent2.boidGroup[j].SteeringSpeed;
                child1.boidGroup[j].LocalAreaRadius = parent2.boidGroup[j].LocalAreaRadius;
                child1.boidGroup[j].PredatorFleeArea = parent2.boidGroup[j].PredatorFleeArea;
                //Child 2 has speed, fear factor and no clumping radius of P2
                child2.boidGroup[j].SteeringSpeed = parent1.boidGroup[j].SteeringSpeed;
                child2.boidGroup[j].LocalAreaRadius = parent1.boidGroup[j].LocalAreaRadius;
                child2.boidGroup[j].PredatorFleeArea = parent1.boidGroup[j].PredatorFleeArea;
            }
            //MUST ADD MUTATION FUNC BEFORE ADDING TO LIST
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
                bool predAreaChanged = true;

                while (changed == false)
                {
                    int random = Random.Range(0, 6);//Result = 0-5
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
                        case 5://PredArea (Probably don't go above 4)
                            {
                                if (child.boidGroup[i].PredatorFleeArea <= 4.0f)
                                {
                                    child.boidGroup[i].PredatorFleeArea += 0.1f;
                                    changed = true;
                                }
                                else
                                    predAreaChanged = false;
                                break;
                            }
                    }

                    if (speedChanged == false && steeringChanged == false && clumpChanged == false && localChanged == false && fearChanged == false && predAreaChanged == false)
                    {
                        changed = true;
                    }
                }
            }
        }
        return child;
    }

    public void RouletteSelection(List<Chromosome> chromoVector)
    {
        List<Chromosome> tempVector = new List<Chromosome>();
        tempVector = GetScores(chromoVector);

        //Have to get the survivor total out side of getscores() for roulette to work
        int totalSurvivors = 0;
        for (int i = 0; i < tempVector.Count; i++)
        {
            Chromosome temp = tempVector[i];
            totalSurvivors += temp.swarmScore; 
        }

        List<Chromosome> bestChromos = new List<Chromosome>();
        for (int i = 0; i < tempVector.Count; i++)
        {
            int random = Random.Range(0, totalSurvivors);
            int runningSurvivors = 0;
            for (int j = 0; j < tempVector.Count; j++)
            {
                Chromosome temp = tempVector[j];
                runningSurvivors += temp.swarmScore;
                if (random <= runningSurvivors)
                {
                    bestChromos.Add(temp);
                    break;
                }
            }
        }
        //Stops the need to clear and reset lists every time
        BestChromoRoulette = bestChromos;
    }

    public List<Chromosome> GetScores(List<Chromosome> chromo)
    {
        List<Chromosome> tempVector = new List<Chromosome>();
        tempVector = chromo;
        int score = 0;
        int totalSurvivors = 0;
        int totalInput = 0;
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
                totalInput++;
            }
            totalSurvivors += score;
            Chromosome temp = tempVector[i];
            temp.swarmScore = score;
            tempVector[i] = temp;
            string result = "Swarm No: " + i.ToString() + " Swarm Score: " + score.ToString();
            RecordLine(result);
        }

        float percentage = (float)totalSurvivors / (float)totalInput;
        percentage = percentage * 100;
        
        string result2 = "Total Survived: " + totalSurvivors.ToString() + "/" + totalInput.ToString() + " Percentage: " + percentage.ToString() + "%";
        RecordLine(result2);

        return tempVector;
    }
}