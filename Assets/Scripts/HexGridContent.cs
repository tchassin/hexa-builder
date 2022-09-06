
using UnityEngine;

public abstract class HexCellContent : MonoBehaviour
{
    public HexCell cell => m_cell;
    public HexCoordinates gridPosition => m_cell.position;
    public abstract TerrainType requiredTerrainType { get; }

    private HexCell m_cell;

    public virtual void OnPlacedOn(HexCell cell)
    {
        Debug.Assert(CanBePlacedOn(cell), this);
        m_cell = cell;
    }

    public virtual void OnRemoved() { }

    public virtual bool CanBePlacedOn(HexCell cell)
        => cell != null && cell.terrainType == requiredTerrainType && (cell.content == this || !cell.isOccupied);
}
