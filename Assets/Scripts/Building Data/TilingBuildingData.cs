using System.Collections.Generic;
using UnityEngine;

public abstract class TilingBuildingData : BuildingData
{
    private struct CellPatternMesh
    {
        public List<HexDirection> pattern;
        public Mesh mesh;
    }

    [Header("Meshes")]
    [SerializeField] private Mesh m_singleMesh;
    [Space]
    [SerializeField] private Mesh m_oneWayMesh;
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
    [SerializeField] private Mesh m_fourWayKRoadMesh;
    [SerializeField] private Mesh m_fourWayXRoadMesh;
    [Space]
    [SerializeField] private Mesh m_fiveWayMesh;
    [Space]
    [SerializeField] private Mesh m_sixWayMesh;

    public override void OnInstanceBuilt(Building building)
    {
        base.OnInstanceBuilt(building);

        building.cell.GetNeighbors(this, out List<Building> neighbors);

        UpdateBuildingMesh(building);

        foreach (var neighbor in neighbors)
            UpdateBuildingMesh(neighbor);
    }

    public override void OnInstanceDemolished(Building building)
    {
        base.OnInstanceDemolished(building);

        building.cell.GetNeighbors(this, out List<Building> neighbors);
        foreach (var neighbor in neighbors)
            UpdateBuildingMesh(neighbor);
    }

    private bool UpdateBuildingMesh(Building building)
    {
        if (building.meshFilter == null)
        {
            Debug.Log($"Missing MeshFilter on {building.name}", building);

            return false;
        }

        building.cell.GetNeighborDirections(this, out List<HexDirection> directions);
        if (directions.Count == 6)
        {
            building.meshFilter.mesh = m_sixWayMesh;
        }
        else if (directions.Count == 5)
        {
            UpdateMesh(new List<CellPatternMesh>
            {
                new CellPatternMesh
                {
                    pattern = new List<HexDirection> { HexDirection.E, HexDirection.SE, HexDirection.SW, HexDirection.W, HexDirection.NW },
                    mesh = m_fiveWayMesh
                },
            });
        }
        else if (directions.Count == 4)
        {
            UpdateMesh(new List<CellPatternMesh>
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
        else if (directions.Count == 3)
        {
            UpdateMesh(new List<CellPatternMesh>
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
        else if (directions.Count == 2)
        {
            UpdateMesh(new List<CellPatternMesh>
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
        else if (directions.Count == 1)
        {
            building.meshFilter.mesh = m_oneWayMesh;
            building.RotateMeshToward(directions[0]);
        }
        else
        {
            building.meshFilter.mesh = m_singleMesh;
        }

        bool UpdateMesh(List<CellPatternMesh> cellPatterns)
        {
            foreach (var cellPatternMesh in cellPatterns)
            {
                if (!directions.IsPermutationOf(cellPatternMesh.pattern, out int distance, out bool isFlipped))
                    continue;

                building.meshFilter.mesh = cellPatternMesh.mesh;
                building.meshFilter.transform.localScale = isFlipped ? new Vector3(1, 1, -1) : Vector3.one;
                building.RotateMesh(-distance);

                return true;
            }

            Debug.LogError("No matching pattern found", this);

            return false;
        }

        return true;
    }
}
