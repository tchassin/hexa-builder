using UnityEngine;

public class BuildModeClickHandler : IGridClickHandler
{
    private BuildingData m_buildingData;
    private Tooltip m_tooltip;

    public BuildingData buildingData => m_buildingData;

    public BuildModeClickHandler(BuildingData buildingData)
    {
        Debug.Assert(buildingData != null);
        m_buildingData = buildingData;
        m_tooltip = Object.FindObjectOfType<Tooltip>();

        BuildModeManager.instance.EnterBuildMode(m_buildingData.buildingPrefab, m_buildingData.GetFacingDirection());
    }

    ~BuildModeClickHandler()
    {
        BuildModeManager.instance.LeaveBuildMode();
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        BuildModeManager.instance.UpdatePreview(m_buildingData, cell);

        if (m_tooltip == null)
            return;

        if (!buildingData.CanBePlacedOn(cell))
        {
            m_tooltip.AddText($"Can't be placed here!");
            m_tooltip.Show();
        }
        else if (!buildingData.CanBeAfforded())
        {
            m_tooltip.AddText($"Not enough resources!");
            m_tooltip.Show();
        }
        else if (buildingData is ProductionBuildingData productionBuildingData)
        {
            float maxEfficiency = productionBuildingData.GetEfficiency(cell);
            m_tooltip.AddText($"Max efficiency: {maxEfficiency}");
            m_tooltip.Show();
        }
    }

    public void OnCellHoverEnd(HexCell cell)
    {
        if (m_tooltip != null)
            m_tooltip.Hide();
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        BuildModeManager.instance.PlaceBuilding(buildingData, cell);
    }

    public void OnLeftClickEnd(HexCell cell)
    {
    }

    public void OnRightClickBegin(HexCell cell)
    {
    }

    public void OnRightClickEnd(HexCell cell)
    {
    }
}
