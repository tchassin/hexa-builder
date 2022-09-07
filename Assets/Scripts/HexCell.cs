using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexCell : MonoBehaviour
{

    [SerializeField] private UnityEvent m_onTerrainChanged;
    [SerializeField] private UnityEvent m_onMouseEnter;
    [SerializeField] private UnityEvent m_onMouseExit;

    [Header("Material")]
    [SerializeField] private Material m_groundMaterial;
    [SerializeField] private Material m_waterMaterial;

    public HexCoordinates position => m_position;
    public TerrainType terrainType => m_terrainType;
    public List<HexCell> neighbors => new List<HexCell>(m_neighbors);
    public HexCellContent content => m_content;
    public bool isOccupied => m_content != null;

    public UnityEvent onTerrainChanged => m_onTerrainChanged;
    public UnityEvent onMouseEnter => m_onMouseEnter;
    public UnityEvent onMouseExit => m_onMouseExit;

    private Renderer m_renderer;
    private TerrainType m_terrainType;
    private HexCoordinates m_position;
    private HexCellContent m_content;
    private readonly List<HexCell> m_neighbors = new List<HexCell>();

    private void Awake()
    {
        TryGetComponent(out m_renderer);
        for (int i = 0; i < 6; i++)
            m_neighbors.Add(null);
    }

    private void OnMouseEnter()
    {
        m_onMouseEnter.Invoke();
    }

    private void OnMouseExit()
    {
        m_onMouseExit.Invoke();
    }

    public void Initialize(HexCoordinates gridPosition, TerrainType terrainType)
    {
        m_position = gridPosition;
        SetTerrainType(terrainType);
    }

    public void SetTerrainType(TerrainType terrainType)
    {
        m_terrainType = terrainType;

        if (content != null && !content.CanBePlacedOn(this))
            SetContent(null);

        // Set height
        float cellHeight = (terrainType == TerrainType.Ground) ? 1.0f : 0.8f;
        transform.localScale = new Vector3(1.0f, cellHeight, 1.0f);
        var localPosition = transform.localPosition;
        localPosition.y = -1.0f + cellHeight;
        transform.localPosition = localPosition;

        // Set material
        if (m_renderer)
            m_renderer.material = (terrainType == TerrainType.Ground) ? m_groundMaterial : m_waterMaterial;

        // Notify listeners
        m_onTerrainChanged.Invoke();
    }

    public void SetContent(HexCellContent content)
    {
        Debug.Assert(m_content != content, this);
        Debug.Assert(m_content == null || content == null, this);

        var previousContent = m_content;
        m_content = content;

        if (previousContent != null)
        {
            previousContent.OnRemoved();
            Destroy(previousContent.gameObject);
        }

        if (m_content != null)
            m_content.OnPlacedOn(this);
    }

    public int DistanceTo(HexCell other)
        => HexCoordinates.Distance(m_position, other.m_position);

    public void SetNeighbor(HexCell neighbor, HexDirection direction)
    {
        SetNeighbor(neighbor, (int)direction);
        if (neighbor != null)
            neighbor.SetNeighbor(this, (int)direction.GetOpposite());
    }

    private void SetNeighbor(HexCell neighbor, int index)
    {
        Debug.Assert(m_neighbors[index] == null, this);
        m_neighbors[index] = neighbor;
    }

    public HexCell GetNeighbor(HexDirection direction)
        => m_neighbors[(int)direction];
}
