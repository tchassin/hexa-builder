using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class BuildingStateDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_nameLabelPrefab;
    [SerializeField] private ResourceNumberDisplay m_costDisplayPrefab;

    private Building m_building;
    private Tooltip m_tooltip;

    private void Awake()
    {
        TryGetComponent(out m_building);
        m_tooltip = FindObjectOfType<GameUI>().tooltip;
    }

    public void OnSelected()
    {
        m_tooltip.ClearContent();

        var buildingData = m_building.data as BuildingData;
        if (!string.IsNullOrEmpty(buildingData.description))
        {
            if (m_nameLabelPrefab != null)
            {
                var nameLabel = Instantiate(m_nameLabelPrefab);
                nameLabel.text = buildingData.displayName;
                m_tooltip.AddContent(nameLabel.gameObject);
            }
            else
            {
                m_tooltip.AddText(buildingData.displayName);
            }
        }

        var inputResource = buildingData.input.resource;
        var outputResource = buildingData.output.resource;
        float efficiency = 1.0f;

        if (!string.IsNullOrEmpty(buildingData.description))
            m_tooltip.AddText(buildingData.description);

        if (buildingData is not RoadData && !m_building.cell.HasAccessToRoad())
            m_tooltip.AddText("This building has no access to roads", Color.red);

        if (buildingData is HousingData housingData)
        {
            string text = $"Pop: {m_building.workers}/{housingData.maxWorkers}";
            efficiency = m_building.workers;
            m_tooltip.AddText(text);
        }
        else if (buildingData is ProductionBuildingData productionBuildingData)
        {
            if (!m_building.cell.HasAccessToWorkers())
                m_tooltip.AddText("This building has no access to workers", Color.red);

            bool needsResourceAccess = inputResource != null && !m_building.cell.HasAccessToResource(inputResource);
            if (needsResourceAccess)
                m_tooltip.AddText($"This building has no access to {inputResource.displayName}", Color.red);

            efficiency = needsResourceAccess ? 0 : productionBuildingData.GetEfficiency(m_building);
            m_tooltip.AddText($"Eff.: {Mathf.RoundToInt(efficiency * 100)}%");

            string workersText = $"Workers: {m_building.workers}/{productionBuildingData.maxWorkers} (min {productionBuildingData.minWorkers})"; ;
            m_tooltip.AddText(workersText);

            if (productionBuildingData.requiredNeighborData != null)
            {
                m_building.cell.GetNeighbors(productionBuildingData.requiredNeighborData, out List<HexCellContent> neighbors);
                string neighborsText = $"{neighbors.Count}/{productionBuildingData.requiredNeighborCount} {productionBuildingData.requiredNeighborData.displayName} on neigbor cells."; ;
                m_tooltip.AddText(neighborsText);
            }
        }

        if (inputResource != null)
        {
            string inputText = $"Uses {efficiency * buildingData.maxResourceConsumption:N2}" +
                $"/{buildingData.maxResourceConsumption:N2}" +
                $" {inputResource.displayName}/s.";
            m_tooltip.AddText(inputText);
        }

        if (outputResource != null)
        {
            string outputText = $"Produces {efficiency * buildingData.maxResourceProduction:N2}" +
                $"/{buildingData.maxResourceProduction:N2}" +
                $" {outputResource.displayName}/s.";
            m_tooltip.AddText(outputText);
        }

        if (buildingData.hasUpgrade)
        {
            m_tooltip.AddText("");
            string upgradeText = $"Upgrade: {buildingData.upgrade.buildingData.displayName}";
            if (m_nameLabelPrefab != null)
            {
                var upgradeLabel = Instantiate(m_nameLabelPrefab);
                upgradeLabel.text = upgradeText;
                m_tooltip.AddContent(upgradeLabel.gameObject);
            }
            else
            {
                m_tooltip.AddText(upgradeText);
            }

            if (m_costDisplayPrefab != null)
            {
                m_tooltip.AddText("Req. access:");
                if (buildingData.upgrade.requiresAccessToWokers)
                    m_tooltip.AddText($"- Workers");

                foreach (var resource in buildingData.upgrade.requiredResourceAccess)
                    m_tooltip.AddText($"- {resource.displayName}");

                m_tooltip.AddText("Cost:");
                foreach (var resourceNumber in buildingData.upgrade.resourceCost)
                {
                    var costDisplay = Instantiate(m_costDisplayPrefab);
                    costDisplay.SetResourceCost(resourceNumber);
                    m_tooltip.AddContent(costDisplay.gameObject);
                }
            }
        }

        m_tooltip.Show();
    }
}
