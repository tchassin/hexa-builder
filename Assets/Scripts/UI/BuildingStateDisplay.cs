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

        if (!string.IsNullOrEmpty(buildingData.description))
            m_tooltip.AddText(buildingData.description);

        if (buildingData is HousingData housingData)
        {
            string text = $"Pop: {m_building.workers}/{housingData.maxWorkers}";
            m_tooltip.AddText(text);
        }
        else if (buildingData is ProductionBuildingData productionBuildingData)
        {
            float efficiency = productionBuildingData.GetEfficiency(m_building);
            m_tooltip.AddText($"Eff.: {Mathf.RoundToInt(efficiency * 100)}%");

            string workersText = $"Workers: {m_building.workers}/{productionBuildingData.maxWorkers} (min {productionBuildingData.minWorkers})"; ;
            m_tooltip.AddText(workersText);

            if (productionBuildingData.inputResource != null)
            {
                string inputText = $"Uses {efficiency * productionBuildingData.maxResourceConsumption:N2}/{productionBuildingData.maxResourceConsumption:N2} {productionBuildingData.inputResource.displayName}/s.";
                m_tooltip.AddText(inputText);
            }

            if (productionBuildingData.outputResource != null)
            {
                string outputText = $"Produces {efficiency * productionBuildingData.maxResourceProduction:N2}/{productionBuildingData.maxResourceProduction:N2} {productionBuildingData.outputResource.displayName}/s.";
                m_tooltip.AddText(outputText);
            }
        }

        if (buildingData.hasUpgrade)
        {
            m_tooltip.AddText("");
            m_tooltip.AddText($"Upgrade: {buildingData.upgrade.displayName}");
            if (m_costDisplayPrefab != null)
            {
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
