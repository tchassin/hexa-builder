using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    [SerializeField] private int m_gold;

    public int gold => m_gold;
    public int population => m_population;

    private int m_population = 0;

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

    public void IncreasePopulation(int population)
    {
        m_population += population;
    }

    public bool DecreasePopulation(int population)
    {
        if (population > m_population)
            return false;

        m_population -= population;

        return true;
    }
}
