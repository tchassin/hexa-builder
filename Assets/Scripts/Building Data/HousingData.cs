using UnityEngine;

[CreateAssetMenu(menuName = "Building Data/Housing")]
public class HousingData : BuildingData
{
    [SerializeField] private int m_capacity;

    public int capacity => m_capacity;

    public override void OnInstanceBuilt(Building building)
    {
        base.OnInstanceBuilt(building);
        Player.instance.IncreasePopulation(m_capacity);
    }

    public override void OnInstanceDemolished(Building building)
    {
        base.OnInstanceDemolished(building);
        Player.instance.DecreasePopulation(m_capacity);
    }

    public override void OnInstanceDowngradedFrom(Building building)
    {
        base.OnInstanceDowngradedFrom(building);
        Player.instance.DecreasePopulation(m_capacity);
    }

    public override void OnInstanceDowngradedTo(Building building)
    {
        base.OnInstanceDowngradedTo(building);
        Player.instance.IncreasePopulation(m_capacity);
    }

    public override void OnInstanceUpgradedFrom(Building building)
    {
        base.OnInstanceUpgradedFrom(building);
        Player.instance.DecreasePopulation(m_capacity);
    }

    public override void OnInstanceUpgradedTo(Building building)
    {
        base.OnInstanceUpgradedTo(building);
        Player.instance.IncreasePopulation(m_capacity);
    }
}
