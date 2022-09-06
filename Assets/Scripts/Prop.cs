using UnityEngine;

public class Prop : HexCellContent
{
    [SerializeField] private TerrainType m_requiredTerrainType;

    public override TerrainType requiredTerrainType => m_requiredTerrainType;
}
