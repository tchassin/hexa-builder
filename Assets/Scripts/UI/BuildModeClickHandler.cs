using UnityEngine;

public class BuildModeClickHandler : IGridClickHandler
{
    private BuildingData m_buildingData;
    private HexGrid m_grid;
    private GameUI m_gameUI;

    public BuildingData buildingData => m_buildingData;

    public BuildModeClickHandler(BuildingData buildingData)
    {
        Debug.Assert(buildingData != null);
        m_buildingData = buildingData;
        m_gameUI = Object.FindObjectOfType<GameUI>();
        m_grid = Object.FindObjectOfType<HexGrid>();

        BuildModeManager.instance.EnterBuildMode(m_buildingData.buildingPrefab, m_buildingData.GetFacingDirection());
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        BuildModeManager.instance.UpdatePreview(m_buildingData, cell);

        if (m_gameUI.tooltip == null)
            return;

        m_gameUI.tooltip.ClearContent();
        if (!buildingData.CanBePlacedOn(cell))
        {
            m_gameUI.tooltip.AddText($"Can't be placed here!");
            m_gameUI.tooltip.Show(Input.mousePosition);
        }
        else if (!buildingData.CanBeAfforded())
        {
            m_gameUI.tooltip.AddText($"Not enough resources!");
            m_gameUI.tooltip.Show(Input.mousePosition);
        }
        else if (!m_grid.accessLevels.HasAccessToRoad(cell))
        {
            m_gameUI.tooltip.AddText($"Buildings need road access!");
            m_gameUI.tooltip.Show(Input.mousePosition);
        }
        else if (buildingData is ProductionBuildingData productionBuildingData)
        {
            float maxEfficiency = productionBuildingData.GetEfficiency(cell);
            m_gameUI.tooltip.AddText($"Max efficiency: {maxEfficiency}");
            m_gameUI.tooltip.Show(Input.mousePosition);
        }
    }

    public void OnCellHoverEnd(HexCell cell)
    {
        if (m_gameUI.tooltip != null)
            m_gameUI.tooltip.Hide();
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        BuildModeManager.instance.BuyBuilding(buildingData, cell);
    }

    public void OnLeftClickEnd(HexCell cell)
    {
    }

    public void OnRightClickBegin(HexCell cell)
    {
    }

    public void OnRightClickEnd(HexCell cell)
    {
        m_gameUI.ToggleSelectMode();
    }
}
