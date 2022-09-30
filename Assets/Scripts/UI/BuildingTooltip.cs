using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BuildingData m_buildingData;

    [Header("Prefabs")]
    [SerializeField] private TextMeshProUGUI m_nameLabelPrefab;
    [SerializeField] private TextMeshProUGUI m_descriptionLabelPrefab;
    [SerializeField] private ResourceNumberDisplay m_costDisplayPrefab;

    private Tooltip m_tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_tooltip == null)
            m_tooltip = FindObjectOfType<Tooltip>();

        if (m_tooltip == null)
            return;

        var tooltipContent = new List<GameObject>();

        if (m_nameLabelPrefab != null)
        {
            var nameLabel = Instantiate(m_nameLabelPrefab);
            nameLabel.text = m_buildingData.displayName;
            tooltipContent.Add(nameLabel.gameObject);
        }

        if (m_descriptionLabelPrefab != null)
        {
            var descriptionLabel = Instantiate(m_descriptionLabelPrefab);
            descriptionLabel.text = m_buildingData.description;
            tooltipContent.Add(descriptionLabel.gameObject);

            if (m_buildingData is HousingData housingData)
            {
                var populationLabel = Instantiate(m_descriptionLabelPrefab);
                populationLabel.text = $"Room for {housingData.maxWorkers} people.";
                tooltipContent.Add(populationLabel.gameObject);
            }

            if (m_buildingData is ProductionBuildingData productionBuildingData)
            {
                var workersLabel = Instantiate(m_descriptionLabelPrefab);
                workersLabel.text = $"Requires {productionBuildingData.maxWorkers} workers.";
                tooltipContent.Add(workersLabel.gameObject);

                if (productionBuildingData.inputResource != null)
                {
                    var inputLabel = Instantiate(m_descriptionLabelPrefab);
                    inputLabel.text = $"Uses {productionBuildingData.maxResourceConsumption:N2} {productionBuildingData.inputResource.displayName}/s.";
                    tooltipContent.Add(inputLabel.gameObject);
                }

                if (productionBuildingData.outputResource != null)
                {
                    var outputLabel = Instantiate(m_descriptionLabelPrefab);
                    outputLabel.text = $"Produces {productionBuildingData.maxResourceProduction:N2} {productionBuildingData.outputResource.displayName}/s.";
                    tooltipContent.Add(outputLabel.gameObject);
                }
            }
        }

        if (m_costDisplayPrefab != null)
        {
            foreach (var resourceNumber in m_buildingData.resourceCost)
            {
                var costDisplay = Instantiate(m_costDisplayPrefab);
                costDisplay.SetResourceCost(resourceNumber);
                tooltipContent.Add(costDisplay.gameObject);
            }
        }

        m_tooltip.Show(tooltipContent);
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
