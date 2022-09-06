using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Road : HexCellContent
{
    private struct CellPatternMesh
    {
        public List<HexDirection> pattern;
        public Mesh mesh;
    }
    public override TerrainType requiredTerrainType => TerrainType.Ground;

    [SerializeField] private int m_cost;

    [Header("Meshes")]
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

    private MeshFilter m_meshFilter;

    private void Awake()
    {
        TryGetComponent(out m_meshFilter);
    }

    public List<HexDirection> GetNeighborRoads()
    {
        var directions = new List<HexDirection>();

        for (int i = 0; i < 6; i++)
        {
            if (cell.neighbors[i] != null && cell.neighbors[i].content is Road)
                directions.Add((HexDirection)i);
        }

        return directions;
    }

    public override void OnPlacedOn(HexCell cell)
    {
        base.OnPlacedOn(cell);

        UpdateMesh();
        foreach (var neighbor in cell.neighbors)
        {
            if (neighbor != null && neighbor.content is Road neighborRoad)
                neighborRoad.UpdateMesh();
        }
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        foreach (var neighbor in cell.neighbors)
        {
            if (neighbor != null && neighbor.content is Road neighborRoad)
                neighborRoad.UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        Debug.Assert(m_meshFilter != null, this);

        var neighborRoads = GetNeighborRoads();
        if (neighborRoads.Count == 6)
        {
            m_meshFilter.mesh = m_sixWayRoadMesh;
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
            m_meshFilter.mesh = m_singleRoadMesh;
            RotateMeshToward(neighborRoads[0]);
        }
        else
        {
            m_meshFilter.mesh = m_isolatedRoadMesh;
        }

        bool UpdateRoad(List<CellPatternMesh> cellPatterns)
        {
            foreach (var cellPatternMesh in cellPatterns)
            {
                if (!neighborRoads.IsPermutationOf(cellPatternMesh.pattern, out int distance, out bool isFlipped))
                    continue;

                m_meshFilter.mesh = cellPatternMesh.mesh;
                m_meshFilter.transform.localScale = isFlipped ? new Vector3(1, 1, -1) : Vector3.one;
                RotateMesh(-distance);

                return true;
            }

            Debug.LogError("No matching pattern found", this);

            return false;
        }
    }

    private void RotateMeshToward(HexDirection direction)
        => RotateMesh(HexDirection.E.DistanceTo(direction));

    private void RotateMesh(int distance)
    {
        // Clamp angle to ]-180; 180]
        int clampedDistance = distance > 3 ? distance - 6 : distance <= -3 ? distance + 6 : distance;
        m_meshFilter.transform.localEulerAngles = Vector3.up * (clampedDistance * 60.0f);
    }
}
