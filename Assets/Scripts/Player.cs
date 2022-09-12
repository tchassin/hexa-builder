using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    [SerializeField] private int m_gold;
    [SerializeField] private List<ResourceNumber> m_resources;

    public int gold => m_gold;
    public List<ResourceNumber> resources => m_resources;

    public int population => m_population;
    public int maxPopulation => m_maxPopulation;
    public int idlePopulation => population - assignedJobs;

    public int assignedJobs => m_assignedJobs;
    public int totalJobs => m_totalJobs;
    public int unassignedJobs => totalJobs - assignedJobs;

    public float unassignedJobRate => m_totalJobs > 0 ? (float)unassignedJobs / m_totalJobs : 0;

    private int m_population = 0;
    private int m_maxPopulation = 0;

    private int m_assignedJobs = 0;
    private int m_totalJobs = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddGold(int gold)
    {
        m_gold += gold;
    }

    public bool UseGold(int gold)
    {
        Debug.Assert(gold <= m_gold, this);

        m_gold -= gold;

        return true;
    }

    public void IncreaseMaxPopulation(int value)
    {
        Debug.Assert(value >= 0, this);
        m_maxPopulation += value;
    }

    public void DecreaseMaxPopulation(int value)
    {
        Debug.Assert(value >= 0, this);
        Debug.Assert(value <= maxPopulation, this);
        Debug.Assert(population <= maxPopulation - value, this);
        m_maxPopulation -= value;
    }

    public void AddPopulation(int value)
    {
        Debug.Assert(value >= 0, this);
        Debug.Assert(m_population + value <= maxPopulation, this);
        m_population += value;
    }

    public bool RemovePopulation(int value)
    {
        Debug.Assert(value >= 0, this);
        Debug.Assert(m_population - value >= 0, this);
        if (value > m_population)
            return false;

        m_population -= value;

        return true;
    }

    public void AddJobs(int value)
    {
        Debug.Assert(value >= 0, this);
        m_totalJobs += value;
    }

    public void RemoveJobs(int value)
    {
        Debug.Assert(value >= 0, this);
        Debug.Assert(value <= m_totalJobs, this);
        Debug.Assert(assignedJobs <= m_totalJobs - value, this);
        m_totalJobs -= value;
    }

    public void AssignWorkers(int value)
    {
        Debug.Assert(value >= 0, this);
        m_assignedJobs += value;
    }

    public void FreeWorkers(int value)
    {
        Debug.Assert(value >= 0, this);
        m_assignedJobs -= value;
    }
}
