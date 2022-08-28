public class TerrainClickHandler : IGridClickHandler
{
    public void OnCellHoverBegin(Cell cell) { }

    public void OnCellHoverEnd(Cell cell) { }

    public void OnLeftClickBegin(Cell cell) { }

    public void OnLeftClickEnd(Cell cell)
    {
        if (cell == null || cell.terrainType == TerrainType.Ground)
            return;

        cell.SetTerrainType(TerrainType.Ground);
    }

    public void OnRightClickBegin(Cell cell) { }

    public void OnRightClickEnd(Cell cell)
    {
        if (cell == null || cell.terrainType == TerrainType.Water)
            return;

        cell.SetTerrainType(TerrainType.Water);
    }
}
