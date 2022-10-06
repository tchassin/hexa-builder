using UnityEngine;

[CreateAssetMenu(menuName = "Data/Building/Housing")]
public class HousingData : BuildingData
{
    [SerializeField] private int m_capacity;

    public override int maxWorkers => m_capacity;

    public override void OnInstanceUpdated(Building building)
    {
        base.OnInstanceUpdated(building);

        if (building.cell.HasAccessToRoad())
        {
            if (building.workers == maxWorkers)
                return;

            building.progress += Random.Range(0.01f, 0.2f) * Time.deltaTime;

            if (building.progress < 1.0f)
                return;

            building.AddWorkers(1);
            building.progress = building.workers < maxWorkers ? building.progress - 1 : 0.0f;
        }
        else
        {
            building.RemoveWorkers(building.workers);
            building.progress = 0;
        }
    }

    public override void OnInstanceBuilt(Building building)
    {
        base.OnInstanceBuilt(building);
        Player.instance.IncreaseMaxPopulation(m_capacity);
    }

    public override void OnInstanceDemolished(Building building)
    {
        base.OnInstanceDemolished(building);
        Player.instance.DecreaseMaxPopulation(m_capacity);
    }

    public override void OnInstanceDowngradedFrom(Building building)
    {
        base.OnInstanceDowngradedFrom(building);
        Player.instance.DecreaseMaxPopulation(m_capacity);
    }

    public override void OnInstanceDowngradedTo(Building building)
    {
        base.OnInstanceDowngradedTo(building);
        Player.instance.IncreaseMaxPopulation(m_capacity);
    }

    public override void OnInstanceUpgradedFrom(Building building)
    {
        base.OnInstanceUpgradedFrom(building);
        Player.instance.DecreaseMaxPopulation(m_capacity);
    }

    public override void OnInstanceUpgradedTo(Building building)
    {
        base.OnInstanceUpgradedTo(building);
        Player.instance.IncreaseMaxPopulation(m_capacity);
    }
}
