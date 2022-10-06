using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Building/Production")]
public class ProductionBuildingData : BuildingData
{
    [SerializeField] private float m_productionTime = 1.0f;

    [Header("Workers")]
    [SerializeField] private int m_minWorkers = 1;
    [SerializeField] private int m_maxWorkers = 3;

    [Header("Resources")]
    [SerializeField] private ResourceNumber m_input;
    [SerializeField] private ResourceNumber m_output;

    [Header("Required props & building")]
    [SerializeField] private CellContentData m_requiredContent;
    [SerializeField] private int m_maxContent = 3;

    public CellContentData requiredNeighborData => m_requiredContent;
    public int requiredNeighborCount => m_maxContent;
    public int minWorkers => m_minWorkers;
    public override int maxWorkers => m_maxWorkers;
    public ResourceData outputResource => m_output.resource;
    public ResourceData inputResource => m_input.resource;
    // Amount of resource consummed per seconds at max efficiency
    public float maxResourceConsumption
        => (m_input.resource != null && m_productionTime > 0.0f) ? m_input.count / m_productionTime : 0.0f;
    // Amount of resource produced per seconds at max efficiency
    public float maxResourceProduction
        => (m_output.resource != null && m_productionTime > 0.0f) ? m_output.count / m_productionTime : 0.0f;

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

        if (m_requiredContent == null)
            return 1.0f;

        cell.GetNeighbors(m_requiredContent, out List<HexCellContent> neighbors);

        return Mathf.Clamp01((float)neighbors.Count / m_maxContent);
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
        if (m_input.resource != null && !building.cell.HasAccessToResource(m_input.resource))
            return;

        // If a production cycle is in progress advance
        if (building.progress > 0.0f)
        {
            float efficiency = GetEfficiency(building);
            building.progress += efficiency * Time.deltaTime / m_productionTime;

            if (building.progress < 1.0f)
                return;

            // Output directly to player resources for now
            if (m_output.resource != null)
                Player.instance.resources.AddResource(m_output);
        }

        // Take input from storage if necessary and possible
        if (m_input.resource != null)
        {
            // Check that we have enough input resource to process
            if (!building.storedResources.HasResource(m_input))
            {
                // Reset progress if a production cycle can't be started
                building.progress = 0.0f;

                return;
            }

            // Consume input resource and start new production cycle
            building.storedResources.UseResource(m_input);
        }

        building.progress = building.progress >= 1.0f
            ? Mathf.Max(float.Epsilon, building.progress - 1.0f)
            : Time.deltaTime / m_productionTime;
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

    public override void OnInstanceDowngradedFrom(Building building)
    {
        base.OnInstanceDowngradedFrom(building);
        Player.instance.RemoveJobs(m_maxWorkers);
    }

    public override void OnInstanceDowngradedTo(Building building)
    {
        base.OnInstanceDowngradedTo(building);
        Player.instance.AddJobs(m_maxWorkers);
    }

    public override void OnInstanceUpgradedFrom(Building building)
    {
        base.OnInstanceUpgradedFrom(building);
        Player.instance.RemoveJobs(m_maxWorkers);
    }

    public override void OnInstanceUpgradedTo(Building building)
    {
        base.OnInstanceUpgradedTo(building);
        Player.instance.AddJobs(m_maxWorkers);
    }
}
