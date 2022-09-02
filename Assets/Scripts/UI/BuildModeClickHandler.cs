using UnityEngine;

public class BuildModeClickHandler : IGridClickHandler
{
    private HexGrid m_grid;

    private BuildingData m_buildingData;

    public BuildingData buildingData => m_buildingData;

    public BuildModeClickHandler(HexGrid grid, BuildingData buildingData)
    {
        Debug.Assert(buildingData != null);
        m_buildingData = buildingData;
        Debug.Assert(grid != null);
        m_grid = grid;
    }

    public void OnCellHoverBegin(HexCell cell)
    {
    }

    public void OnCellHoverEnd(HexCell cell)
    {
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        if (cell == null)
            return;

        m_grid.SetBuilding(m_buildingData, cell);
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
