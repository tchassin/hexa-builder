using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Building/Production")]
public class ProductionBuildingData : BuildingData
{
    [Header("Workers")]
    [SerializeField] private int m_minWorkers = 1;
    [SerializeField] private int m_maxWorkers = 3;

    public int minWorkers => m_minWorkers;
    public override int maxWorkers => m_maxWorkers;

    public float GetEfficiency(Building building)
    {
        if (building.workers < minWorkers)
            return 0.0f;

        float efficiency = GetEfficiency(building.cell);
        efficiency *= (float)building.workers / m_maxWorkers;

        return efficiency;
    }

    public float GetEfficiency(HexCell cell)
    {
        if (!IsCompatibleCell(cell) && cell.content != null && cell.content.data != this)
            return 0.0f;

        if (requiredNeighborData == null)
            return 1.0f;

        cell.GetNeighbors(requiredNeighborData, out List<HexCellContent> neighbors);

        return Mathf.Clamp01((float)neighbors.Count / requiredNeighborCount);
    }

    public override void OnInstanceUpdated(Building building)
    {
        base.OnInstanceUpdated(building);

        // Production buildings need to be near the road and have access to workers
        if (building.cell.HasAccessToRoad() && building.cell.HasAccessToWorkers())
        {
            // Auto-assign workers if possible
            if (Player.instance.assignedJobs > Player.instance.population)
            {
                int removedWorkers = Mathf.Min(Player.instance.assignedJobs - Player.instance.population, building.workers);
                building.RemoveWorkers(removedWorkers);
            }
            else if (building.workers < maxWorkers && Player.instance.idlePopulation > 0)
            {
                int newWorkers = Mathf.Min(Player.instance.idlePopulation, maxWorkers - building.workers);
                building.AddWorkers(newWorkers);
            }
        }
        else
        {
            building.RemoveWorkers(building.workers);
        }

        // Check that we have enough workers
        if (building.workers < m_minWorkers)
            return;

        // Check that we have access to the required resources
        if (input.resource != null && !building.cell.HasAccessToResource(input.resource))
            return;

        // If a production cycle is in progress advance
        if (building.progress > 0.0f)
        {
            float efficiency = GetEfficiency(building);
            building.progress += efficiency * Time.deltaTime / cycleDuration;

            if (building.progress < 1.0f)
                return;

            // Output directly to player resources for now
            if (output.resource != null)
                Player.instance.resources.AddResource(output);
        }

        // Take input from storage if necessary and possible
        if (input.resource != null)
        {
            // Check that we have enough input resource to process
            if (!Player.instance.resources.HasResource(input))
            {
                // Reset progress if a production cycle can't be started
                OnCycleCompleted(building, false);

                return;
            }

            // Consume input resource and start new production cycle
            Player.instance.resources.UseResource(input);
        }

        if (building.progress >= 1.0f)
            OnCycleCompleted(building);
        else
            building.progress = Time.deltaTime / cycleDuration;
    }

    public override void OnInstanceBuilt(Building building)
    {
        base.OnInstanceBuilt(building);
        Player.instance.AddJobs(m_maxWorkers);
    }

    public override void OnInstanceDemolished(Building building)
    {
        base.OnInstanceDemolished(building);
        Player.instance.RemoveJobs(m_maxWorkers);
    }

    public override void OnInstanceDowngradedFrom(Building building, BuildingData newData)
    {
        base.OnInstanceDowngradedFrom(building, newData);
        Player.instance.RemoveJobs(m_maxWorkers);
    }

    public override void OnInstanceDowngradedTo(Building building, BuildingData previousData)
    {
        base.OnInstanceDowngradedTo(building, previousData);
        Player.instance.AddJobs(m_maxWorkers);
    }

    public override void OnInstanceUpgradedFrom(Building building, BuildingData newData)
    {
        base.OnInstanceUpgradedFrom(building, newData);
        Player.instance.RemoveJobs(m_maxWorkers);
    }

    public override void OnInstanceUpgradedTo(Building building, BuildingData previousData)
    {
        base.OnInstanceUpgradedTo(building, previousData);
        Player.instance.AddJobs(m_maxWorkers);
    }
}
