using UnityEngine;

public abstract class CellContentData : ScriptableObject
{
    [SerializeField] private string m_name;
    [SerializeField, Multiline] private string m_description;
    [SerializeField] private TerrainType m_requiredTerrain = TerrainType.Ground;

    public string displayName => m_name;
    public string description => m_description;
    public TerrainType requiredTerrainType => m_requiredTerrain;

    public virtual bool CanBePlacedOn(HexCell cell)
        => cell != null && m_requiredTerrain == cell.terrainType && !cell.isOccupied;
}
