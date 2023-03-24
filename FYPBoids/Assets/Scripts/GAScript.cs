using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Chromosome
{
    float speed;
    float steeringSpeed;
    float noClumpingArea;
    float localArea;
    float fearFactor;
}

public class GAScript : MonoBehaviour
{

    const float m_MutationRate = 0.01f;

   // static vector<Chromosome> m_ChromoVector;
    //static vector<Chromosome> m_BestChromoVector;

    Chromosome GenerateChromo()
    {
        Chromosome chromo = new Chromosome();
        return chromo;
    }
    void GenerateChromoVec()
    {

    }

   // vector<Chromosome> GetChromoVec() { return m_ChromoVector; }
}
