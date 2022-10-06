using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDataTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BuildingData m_buildingData;

    [Header("Prefabs")]
    [SerializeField] private TextMeshProUGUI m_nameLabelPrefab;
    [SerializeField] private ResourceNumberDisplay m_costDisplayPrefab;

    private Tooltip m_tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_tooltip == null)
            m_tooltip = FindObjectOfType<GameUI>().tooltip;

        if (m_tooltip == null)
            return;

        m_tooltip.ClearContent();

        if (!string.IsNullOrEmpty(m_buildingData.description))
        {
            if (m_nameLabelPrefab != null)
            {
                var nameLabel = Instantiate(m_nameLabelPrefab);
                nameLabel.text = m_buildingData.displayName;
                m_tooltip.AddContent(nameLabel.gameObject);
            }
            else
            {
                m_tooltip.AddText(m_buildingData.displayName);
            }
        }

        if (!string.IsNullOrEmpty(m_buildingData.description))
            m_tooltip.AddText(m_buildingData.description);

        if (m_buildingData is HousingData housingData)
        {
            string text = $"Room for {housingData.maxWorkers} people.";
            m_tooltip.AddText(text);
        }
        else if (m_buildingData is ProductionBuildingData productionBuildingData)
        {
            string workersText = $"Requires {productionBuildingData.maxWorkers} workers."; ;
            m_tooltip.AddText(workersText);

            if (productionBuildingData.requiredNeighborData != null)
            {
                string neighborsText = $"Requires {productionBuildingData.requiredNeighborCount}" +
                    $" {productionBuildingData.requiredNeighborData.displayName} on neigbor cells."; ;
                m_tooltip.AddText(neighborsText);
            }

            if (productionBuildingData.input.resource != null)
            {
                string inputText = $"Uses {productionBuildingData.maxResourceConsumption:N2}" +
                    $" {productionBuildingData.input.resource.displayName}/s.";
                m_tooltip.AddText(inputText);
            }

            if (productionBuildingData.output.resource != null)
            {
                string outputText = $"Produces {productionBuildingData.maxResourceProduction:N2}" +
                    $" {productionBuildingData.output.resource.displayName}/s.";
                m_tooltip.AddText(outputText);
            }
        }

        if (m_costDisplayPrefab != null)
        {
            foreach (var resourceNumber in m_buildingData.resourceCost)
            {
                var costDisplay = Instantiate(m_costDisplayPrefab);
                costDisplay.SetResourceCost(resourceNumber);
                m_tooltip.AddContent(costDisplay.gameObject);
            }
        }

        m_tooltip.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_tooltip == null)
            return;

        var raycastObject = eventData.pointerCurrentRaycast.gameObject;
        if (raycastObject != null && raycastObject.transform.IsChildOf(transform))
            return;

        m_tooltip.Hide();
    }
}
