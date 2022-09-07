using UnityEngine;

public abstract class CellContentData : ScriptableObject
{
    [SerializeField] private TerrainType m_requiredTerrain = TerrainType.Ground;

    public virtual bool CanBePlacedOn(HexCell cell)
        => cell != null && m_requiredTerrain == cell.terrainType && !cell.isOccupied;
}
