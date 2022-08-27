public class TerrainClickHandler : IGridClickHandler
{
    public void OnLeftClick(Cell cell)
    {
        if (cell == null || cell.terrainType == TerrainType.Ground)
            return;

        cell.SetTerrainType(TerrainType.Ground);
    }

    public void OnRightClick(Cell cell)
    {
        if (cell == null || cell.terrainType == TerrainType.Water)
            return;

        cell.SetTerrainType(TerrainType.Water);
    }
}
