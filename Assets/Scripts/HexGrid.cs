using System;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType : int
{
    Water,
    Ground,
}

public class HexGrid : MonoBehaviour
{
    [SerializeField] private HexCell m_cellPrefab;
    [SerializeField] private Building m_buildingPrefab;
    [SerializeField] private GameUI m_gameUI;

    public Vector2Int size => m_size;
    public Rect worldBounds => m_worldBounds;

    private Vector2Int m_size;
    private readonly List<HexCell> m_cells = new List<HexCell>();
    private Rect m_worldBounds;

    private void Start()
    {
        int[,] data = new int[10, 10]
        {
            { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 0, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 1, 1, 0, 1, 0, 1, 1, 1, 1 },
            { 1, 1, 0, 0, 1, 0, 1, 1, 1, 1 },
        };

        Generate(data);

        m_gameUI.Initialize();
    }

    public HexCell GetCell(int index)
        => m_cells[index];

    public HexCell GetCell(int x, int y)
        => GetCell(GridPositionToIndex(x, y));

    public HexCell GetCell(HexCoordinates position)
        => GetCell(position.x, position.z);

    public List<HexCell> ShortestPath(HexCell start, HexCell end, Func<HexCell, float> heuristic = null)
    {
        int maxDistance = start.DistanceTo(end);
        var searchFrontier = new HexCellPriorityQueue();
        var previousCells = new Dictionary<HexCell, HexCell>();
        var scores = new Dictionary<HexCell, int>();
        searchFrontier.Enqueue(start, 0);

        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();

            if (current == end)
                return ReconstructPath(end);

            //bool hasPrevious = previousCells.TryGetValue(current, out var previous);
            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null)
                    continue;

                int distance = start.DistanceTo(neighbor);

                // Stop instead of going too far
                if (distance > maxDistance)
                    continue;

                // Stop if a better path to this node exists
                if (scores.ContainsKey(neighbor))
                {
                    if (scores[neighbor] < distance)
                        searchFrontier.Change(neighbor, scores[neighbor], distance);

                    continue;
                }

                scores[neighbor] = distance;
                previousCells[neighbor] = current;

                float priority = distance;
                if (heuristic != null)
                    priority += heuristic(neighbor);

                searchFrontier.Enqueue(neighbor, priority);
            }
        }

        return new List<HexCell>();

        List<HexCell> ReconstructPath(HexCell end)
        {
            var path = new List<HexCell> { end };
            while (end != start)
            {
                end = previousCells[end];
                path.Add(end);
            }

            path.Reverse();

            return path;
        }
    }

    public void Generate(int[,] terrainData)
    {
        m_size = new Vector2Int(terrainData.GetLength(0), terrainData.GetLength(1));
        int cellCount = m_size.x * m_size.y;

        // Clear previous objects and initialize array
        if (m_cells != null)
        {
            foreach (var cellObject in m_cells)
                Destroy(cellObject);
        }
        m_cells.Clear();

        var xOffset = m_size.y > 1 ? HexMetrics.innerRadius : 0.0f;
        var worldSize = HexMetrics.ToWorldPosition(m_size.x, m_size.y);
        var positionOffset = -worldSize * 0.5f;
        transform.position = positionOffset;
        m_worldBounds = new Rect(positionOffset.x, positionOffset.z, worldSize.x + xOffset, worldSize.z);

        // Set terrain data
        for (int i = 0; i < cellCount; i++)
        {
            int x = i % m_size.x;
            int z = i / m_size.x;
            HexCoordinates hexCoordinates = new HexCoordinates(x, z);

            // Instantiate cell and set position
            Vector3 position = hexCoordinates.ToPosition() + positionOffset + Vector3.right * xOffset;
            var cell = Instantiate(m_cellPrefab, position, Quaternion.identity, transform);
            cell.Initialize(hexCoordinates, (terrainData[hexCoordinates.z, hexCoordinates.x] != 0) ? TerrainType.Ground : TerrainType.Water);
            cell.gameObject.name = $"Cell ({hexCoordinates.x}; {hexCoordinates.z})";
            m_cells.Add(cell);

            if (x > 0)
                cell.SetNeighbor(m_cells[i - 1], HexDirection.W);

            if (z > 0)
            {
                if ((z & 1) == 0) // even rows
                {
                    cell.SetNeighbor(m_cells[i - m_size.x], HexDirection.NE);
                    if (x > 0)
                        cell.SetNeighbor(m_cells[i - m_size.x - 1], HexDirection.NW);
                }
                else // odd rows
                {
                    cell.SetNeighbor(m_cells[i - m_size.x], HexDirection.NW);
                    if (x < m_size.x - 1)
                        cell.SetNeighbor(m_cells[i - m_size.x + 1], HexDirection.NE);
                }
            }
        }
    }

    public bool SetBuilding(BuildingData buildingData, HexCell cell)
    {
        if (buildingData == null)
        {
            if (!cell.isOccupied)
                return false;

            cell.building.Demolish();

            return true;
        }

        if (cell.isOccupied || !buildingData.CanBeBuiltOn(cell))
            return false;

        var building = Instantiate(m_buildingPrefab, cell.transform);
        building.Build(buildingData, cell);

        return true;
    }

    public Vector2Int IndexToGridPosition(int id)
        => new Vector2Int(id % m_size.x, id / m_size.x);

    public int GridPositionToIndex(Vector2Int position)
        => GridPositionToIndex(position.x, position.y);

    public int GridPositionToIndex(int x, int y)
        => y * m_size.x + x;
}