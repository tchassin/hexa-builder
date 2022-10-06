using UnityEngine;

[CreateAssetMenu(menuName = "Data/Building/Housing")]
public class HousingData : BuildingData
{
    [Header("Population")]
    [SerializeField] private int m_capacity;

    public override int maxWorkers => m_capacity;

    public override void OnInstanceUpdated(Building building)
    {
        base.OnInstanceUpdated(building);

        if (!building.cell.HasAccessToRoad())
            return;

        building.progress += Time.deltaTime / cycleDuration;

        if (building.progress < 1.0f)
            return;

        if (input.resource == null
        || (building.cell.HasAccessToResource(input.resource)
            && Player.instance.resources.HasResource(input)))
        {
            if (input.resource != null)
                Player.instance.resources.UseResource(input);

            if (output.resource != null)
            {
                var scaledOutput = new ResourceNumber(output.resource, output.count * building.workers);
                Player.instance.resources.AddResource(scaledOutput);
            }

            if (building.workers < maxWorkers)
                building.AddWorkers(1);
        }
        else
        {
            // If the building does not have the require resources, downgrade it
            if (building.canBeDowngraded)
            {
                building.Downgrade();

                if (building.workers > building.maxWorkers)
                    building.RemoveWorkers(building.maxWorkers - building.workers);
            }
        }

        OnCycleCompleted(building);
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

    public override void OnInstanceDowngradedFrom(Building building, BuildingData newData)
    {
        base.OnInstanceDowngradedFrom(building, newData);

        // If downgrading to other housing data, the population will updated after change
        if (newData is not HousingData)
        {
            building.RemoveWorkers(building.workers);
            Player.instance.DecreaseMaxPopulation(m_capacity);
        }
    }

    public override void OnInstanceDowngradedTo(Building building, BuildingData previousData)
    {
        base.OnInstanceDowngradedTo(building, previousData);

        if (previousData is HousingData)
            OnReplacingHousing(building, previousData);
        else
            OnReplacingOtherBuilding(building);
    }

    public override void OnInstanceUpgradedFrom(Building building, BuildingData newData)
    {
        base.OnInstanceUpgradedFrom(building, newData);

        // If upgrading to other housing data, the population will updated after change
        if (newData is not HousingData)
        {
            building.RemoveWorkers(building.workers);
            Player.instance.DecreaseMaxPopulation(m_capacity);
        }
    }

    public override void OnInstanceUpgradedTo(Building building, BuildingData previousData)
    {
        base.OnInstanceUpgradedTo(building, previousData);

        if (previousData is HousingData)
            OnReplacingHousing(building, previousData);
        else
            OnReplacingOtherBuilding(building);
    }

    private void OnReplacingHousing(Building building, BuildingData previousData)
    {
        if (building.workers > maxWorkers)
            building.RemoveWorkers(building.workers - maxWorkers);

        if (previousData.maxWorkers > maxWorkers)
            Player.instance.DecreaseMaxPopulation(previousData.maxWorkers - maxWorkers);
        else if (previousData.maxWorkers < maxWorkers)
            Player.instance.IncreaseMaxPopulation(maxWorkers - previousData.maxWorkers);
    }

    private void OnReplacingOtherBuilding(Building building)
    {
        building.RemoveWorkers(building.workers);
        Player.instance.IncreaseMaxPopulation(m_capacity);
    }
}
