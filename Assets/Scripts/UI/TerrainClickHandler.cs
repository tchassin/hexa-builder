using UnityEngine;

public class TerrainClickHandler : IGridClickHandler
{
    private GameUI m_gameUI;

    public TerrainClickHandler()
    {
        m_gameUI = Object.FindObjectOfType<GameUI>();
    }

    public void OnCellHoverBegin(HexCell cell) { }

    public void OnCellHoverEnd(HexCell cell) { }

    public void OnLeftClickBegin(HexCell cell) { }

    public void OnLeftClickEnd(HexCell cell)
    {
        if (cell == null)
            return;

        if (cell.terrainType == TerrainType.Water)
            cell.SetTerrainType(TerrainType.Ground);
        else
            cell.SetTerrainType(TerrainType.Water);
    }

    public void OnRightClickBegin(HexCell cell) { }

    public void OnRightClickEnd(HexCell cell)
    {
        m_gameUI.ToggleSelectMode();
    }
}
