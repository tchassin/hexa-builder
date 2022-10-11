using UnityEngine;

public class PropsClickHandler : IGridClickHandler
{
    private PropData m_propData;
    private GameUI m_gameUI;

    public PropData propData => m_propData;

    public PropsClickHandler(PropData propData)
    {
        Debug.Assert(propData != null);
        m_propData = propData;
        m_gameUI = Object.FindObjectOfType<GameUI>();
        BuildModeManager.instance.EnterBuildMode(BuildModeManager.instance.treePrefab.gameObject, (HexDirection)Random.Range(0, 6));
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        BuildModeManager.instance.UpdatePreview(propData, cell);
    }

    public void OnCellHoverEnd(HexCell cell)
    {
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        BuildModeManager.instance.PlaceProp(m_propData, cell);
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
