using UnityEngine;

public class PropsClickHandler : IGridClickHandler
{
    private Prop m_propPrefab;
    private GameUI m_gameUI;

    public Prop propPrefab => m_propPrefab;

    public PropsClickHandler(Prop propPrefab)
    {
        Debug.Assert(propPrefab != null);
        m_gameUI = Object.FindObjectOfType<GameUI>();
        m_propPrefab = propPrefab;
        BuildModeManager.instance.EnterBuildMode(propPrefab.gameObject, (HexDirection)Random.Range(0, 6));
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
        m_gameUI.ToggleSelectMode();
    }
}
