
using UnityEngine;

public abstract class HexCellContent : MonoBehaviour
{
    public HexCell cell => m_cell;
    public HexCoordinates gridPosition => m_cell.position;
    public abstract TerrainType requiredTerrainType { get; }

    private HexCell m_cell;
    public MeshFilter meshFilter => m_meshFilter;

    private GameObject m_model;
    private MeshFilter m_meshFilter;

    public virtual void OnPlacedOn(HexCell cell)
    {
        Debug.Assert(CanBePlacedOn(cell), this);
        m_cell = cell;
    }

    public virtual void OnRemoved() { }

    public virtual bool CanBePlacedOn(HexCell cell)
        => cell != null && cell.terrainType == requiredTerrainType && (cell.content == this || !cell.isOccupied);

    protected void UpdateModel(GameObject prefab)
    {
        if (m_model != null)
        {
            Destroy(m_model);
            m_meshFilter = null;
        }

        if (prefab != null)
        {
            m_model = Instantiate(prefab, transform);
            m_model.TryGetComponent(out m_meshFilter);
        }
    }

    public void RotateMeshToward(HexDirection direction)
    {
        float angle = direction.ToAngle();
        m_meshFilter.transform.localEulerAngles = Vector3.up * angle;
    }

    public void RotateMesh(int distance)
    {
        float angle = DirectionExtensions.DistanceToAngle(distance);
        m_meshFilter.transform.localEulerAngles = Vector3.up * angle;
    }
}
