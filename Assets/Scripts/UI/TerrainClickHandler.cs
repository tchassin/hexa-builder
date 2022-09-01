public class TerrainClickHandler : IGridClickHandler
{
    public void OnCellHoverBegin(HexCell cell) { }

    public void OnCellHoverEnd(HexCell cell) { }

    public void OnLeftClickBegin(HexCell cell) { }

    public void OnLeftClickEnd(HexCell cell)
    {
        if (cell == null || cell.terrainType == TerrainType.Ground)
            return;

        cell.SetTerrainType(TerrainType.Ground);
    }

    public void OnRightClickBegin(HexCell cell) { }

    public void OnRightClickEnd(HexCell cell)
    {
        if (cell == null || cell.terrainType == TerrainType.Water)
            return;

        cell.SetTerrainType(TerrainType.Water);
    }
}
