using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    private struct CellPatternMesh
    {
        public List<HexDirection> pattern;
        public Mesh mesh;
    }

    [SerializeField] private UnityEvent m_onTerrainChanged;

    [Header("Material")]
    [SerializeField] private Material m_groundMaterial;
    [SerializeField] private Material m_waterMaterial;

    [Header("Roads")]
    [SerializeField] private MeshFilter m_roadPrefab;
    [Space]
    [SerializeField] private Mesh m_isolatedRoadMesh;
    [Space]
    [SerializeField] private Mesh m_singleRoadMesh;
    [Space]
    [SerializeField] private Mesh m_twoWayAcrossMesh;
    [SerializeField] private Mesh m_twoWaySharpTurnMesh;
    [SerializeField] private Mesh m_twoWayTurnMesh;
    [Space]
    [SerializeField] private Mesh m_threeWayEvenMesh;
    [SerializeField] private Mesh m_threeWaySameSideMesh;
    [SerializeField] private Mesh m_threeWaySplitMesh;
    [Space]
    [SerializeField] private Mesh m_fourWayForkRoadMesh;
    [SerializeField] private Mesh m_fourWayXRoadMesh;
    [SerializeField] private Mesh m_fourWayKRoadMesh;
    [Space]
    [SerializeField] private Mesh m_fiveWayRoadMesh;
    [Space]
    [SerializeField] private Mesh m_sixWayRoadMesh;

    public Vector2Int position => m_position;
    public TerrainType terrainType => m_terrainType;
    public bool hasRoad => m_road != null;

    public UnityEvent onTerrainChanged => m_onTerrainChanged;

    private Renderer m_renderer;
    private TerrainType m_terrainType;
    private Vector2Int m_position;
    private MeshFilter m_road;
    private readonly List<Cell> m_neighbors = new List<Cell>();

    private void Awake()
    {
        TryGetComponent(out m_renderer);
        for (int i = 0; i < 6; i++)
            m_neighbors.Add(null);
    }

    public void Initialize(Vector2Int gridPosition, TerrainType terrainType)
    {
        m_position = gridPosition;
        SetTerrainType(terrainType);
    }

    public void SetTerrainType(TerrainType terrainType)
    {
        m_terrainType = terrainType;

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

    public void SetNeighbor(Cell neighbor, HexDirection direction)
    {
        SetNeighbor(neighbor, (int)direction);
        if (neighbor != null)
            neighbor.SetNeighbor(this, (int)direction.GetOpposite());
    }

    private void SetNeighbor(Cell neighbor, int index)
    {
        Debug.Assert(m_neighbors[index] == null, this);
        m_neighbors[index] = neighbor;
    }

    public Cell GetNeighbor(HexDirection direction)
        => m_neighbors[(int)direction];

    public List<HexDirection> GetNeighborRoads()
    {
        var directions = new List<HexDirection>();

        for (int i = 0; i < 6; i++)
        {
            if (m_neighbors[i] != null && m_neighbors[i].hasRoad)
                directions.Add((HexDirection)i);
        }

        return directions;
    }

    public void AddRoad()
    {
        if (terrainType == TerrainType.Water)
            return;

        if (m_road != null)
            return;

        m_road = Instantiate(m_roadPrefab, transform);

        UpdateRoad();
        foreach (var neighbor in m_neighbors)
        {
            if (neighbor != null && neighbor.hasRoad)
                neighbor.UpdateRoad();
        }
    }

    public void RemoveRoad()
    {
        Destroy(m_road.gameObject);
        m_road = null;

        foreach (var neighbor in m_neighbors)
        {
            if (neighbor != null && neighbor.hasRoad)
                neighbor.UpdateRoad();
        }
    }

    private void UpdateRoad()
    {
        Debug.Assert(m_road != null, this);

        var neighborRoads = GetNeighborRoads();
        if (neighborRoads.Count == 6)
        {
            m_road.mesh = m_sixWayRoadMesh;
        }
        else if (neighborRoads.Count == 5)
        {
            UpdateRoad(new List<CellPatternMesh>
            {
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SE, HexDirection.SW, HexDirection.W, HexDirection.NW },
                    mesh = m_fiveWayRoadMesh
                },
            });
        }
        else if (neighborRoads.Count == 4)
        {
            UpdateRoad(new List<CellPatternMesh>
            {
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SE, HexDirection.SW, HexDirection.W },
                    mesh = m_fourWayKRoadMesh
                },
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SE, HexDirection.W, HexDirection.NW },
                    mesh = m_fourWayXRoadMesh
                },
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SW, HexDirection.W, HexDirection.NW },
                    mesh = m_fourWayForkRoadMesh
                },
            });
        }
        else if (neighborRoads.Count == 3)
        {
            UpdateRoad(new List<CellPatternMesh>
            {
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SW, HexDirection.NW },
                    mesh = m_threeWayEvenMesh
                },
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SE, HexDirection.SW },
                    mesh = m_threeWaySameSideMesh
                },
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SW, HexDirection.W },
                    mesh = m_threeWaySplitMesh
                },
            });
        }
        else if (neighborRoads.Count == 2)
        {
            UpdateRoad(new List<CellPatternMesh>
            {
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.W },
                    mesh = m_twoWayAcrossMesh
                },
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SW },
                    mesh = m_twoWayTurnMesh
                },
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SE },
                    mesh = m_twoWaySharpTurnMesh
                },
            });
        }
        else if (neighborRoads.Count == 1)
        {
            m_road.mesh = m_singleRoadMesh;
            RotateRoadToward(neighborRoads[0]);
        }
        else
        {
            m_road.mesh = m_isolatedRoadMesh;
        }

        bool UpdateRoad(List<CellPatternMesh> cellPatterns)
        {
            foreach (var cellPatternMesh in cellPatterns)
            {
                if (!neighborRoads.IsPermutationOf(cellPatternMesh.pattern, out int distance, out bool isFlipped))
                    continue;

                m_road.mesh = cellPatternMesh.mesh;
                m_road.transform.localScale = isFlipped ? new Vector3(1, 1, -1) : Vector3.one;
                RotateRoad(-distance);

                return true;
            }

            Debug.LogError("No matching pattern found", this);

            return false;
        }
    }

    private void RotateRoadToward(HexDirection direction)
        => RotateRoad(HexDirection.E.DistanceTo(direction));

    private void RotateRoad(int distance)
    {
        // Clamp angle to ]-180; 180]
        int clampedDistance = distance > 3 ? distance - 6 : distance <= -3 ? distance + 6 : distance;
        m_road.transform.localEulerAngles = Vector3.up * (clampedDistance * 60.0f);
    }
}
