using UnityEngine;

public abstract class CellContentData : ScriptableObject
{
    [SerializeField] private TerrainType m_requiredTerrain = TerrainType.Ground;

    public TerrainType requiredTerrainType => m_requiredTerrain;

    public virtual bool CanBePlacedOn(HexCell cell)
        => cell != null && m_requiredTerrain == cell.terrainType && !cell.isOccupied;
}
