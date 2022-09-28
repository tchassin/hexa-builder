using UnityEngine;

public class BuildModeClickHandler : IGridClickHandler
{
    private BuildingData m_buildingData;

    public BuildingData buildingData => m_buildingData;

    public BuildModeClickHandler(BuildingData buildingData)
    {
        Debug.Assert(buildingData != null);
        m_buildingData = buildingData;

        BuildModeManager.instance.EnterBuildMode(m_buildingData.buildingPrefab, m_buildingData.GetFacingDirection());
    }

    ~BuildModeClickHandler()
    {
        BuildModeManager.instance.LeaveBuildMode();
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        BuildModeManager.instance.UpdatePreview(m_buildingData, cell);
    }

    public void OnCellHoverEnd(HexCell cell)
    {
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
