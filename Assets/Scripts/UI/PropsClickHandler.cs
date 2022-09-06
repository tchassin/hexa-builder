using UnityEngine;

public class PropsClickHandler : IGridClickHandler
{
    private Prop m_propPrefab;

    public Prop propPrefab => m_propPrefab;

    public PropsClickHandler(Prop propPrefab)
    {
        Debug.Assert(propPrefab != null);
        m_propPrefab = propPrefab;
        BuildModeManager.instance.EnterBuildMode(propPrefab.gameObject);
    }

    ~PropsClickHandler()
    {
        BuildModeManager.instance.LeaveBuildMode();
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        BuildModeManager.instance.UpdatePreview(propPrefab, cell);
    }

    public void OnCellHoverEnd(HexCell cell)
    {
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        BuildModeManager.instance.PlaceProp(m_propPrefab, cell);
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