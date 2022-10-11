using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public enum TerrainType : int
{
    Water,
    Ground,
}

public class HexGrid : MonoBehaviour
{
    [SerializeField] private GameStateData m_initialState;
    [SerializeField] private HexCell m_cellPrefab;
    [SerializeField] private Building m_buildingPrefab;
    [SerializeField] private GameUI m_gameUI;

    public Vector2Int size => m_size;
    public HexGridAccessLevels accessLevels => m_accessLevels;

    public Rect worldBounds => m_worldBounds;

    private Vector2Int m_size;
    private readonly List<HexCell> m_cells = new List<HexCell>();
    private Rect m_worldBounds;
    private HexGridAccessLevels m_accessLevels;

    private void Start()
    {
        if (m_initialState != null)
        {
            GameState.Load(m_initialState.state, this);
        }
        else
        {
            int[] data = new int[100]
            {
                 1, 1, 1, 1, 1, 1, 0, 1, 1, 1,
                 1, 1, 1, 1, 1, 1, 0, 1, 1, 1,
                 1, 1, 1, 1, 1, 0, 0, 1, 1, 1,
                 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
                 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
                 1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
                 1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
                 1, 1, 1, 0, 0, 0, 1, 1, 1, 1,
                 1, 1, 1, 0, 1, 0, 1, 1, 1, 1,
                 1, 1, 0, 0, 1, 0, 1, 1, 1, 1,
            };

            Initialize(data, 10, new List<SerializedCellContent>());
        }


        m_gameUI.Initialize();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        var gameStateData = ScriptableObject.CreateInstance<GameStateData>();
        gameStateData.Capture();

        AssetDatabase.CreateAsset(gameStateData, "Assets/GameState.asset");
        AssetDatabase.SaveAssets();
    }
#endif // UNITY_EDITOR

    public HexCell GetCell(int index)
        => m_cells[index];

    public HexCell GetCell(int x, int y)
        => GetCell(GridPositionToIndex(x, y));

    public HexCell GetCell(HexCoordinates position)
        => GetCell(position.x, position.z);

    public HexCell GetCell(Vector3 position)
        => GetCell(HexCoordinates.FromPosition(position));

    public int GetCellIndex(HexCell hexCell)
        => hexCell != null ? GridPositionToIndex(hexCell.position) : -1;

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

    public void Initialize(int[] terrainData, int width, List<SerializedCellContent> cellContents)
    {
        m_size = new Vector2Int(width, terrainData.Length / width);
        int cellCount = m_size.x * m_size.y;

        // must be initialized after size
        m_accessLevels = new(this);

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
            cell.Initialize(this, hexCoordinates, (terrainData[i] != 0) ? TerrainType.Ground : TerrainType.Water);
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

        var buildMode = BuildModeManager.instance;
        foreach (var cellContent in cellContents)
        {
            var cell = GetCell(cellContent.position);
            if (cellContent.data is RoadData road)
                buildMode.PlaceRoad(road, cell);
            else if (cellContent.data is BuildingData building)
                buildMode.PlaceBuilding(building, cell);
            else if (cellContent.data is PropData prop)
                buildMode.PlaceProp(prop, cell);
        }
    }

    public Vector2Int IndexToGridPosition(int id)
        => new Vector2Int(id % m_size.x, id / m_size.x);

    public int GridPositionToIndex(HexCoordinates position)
        => GridPositionToIndex(position.x, position.z);

    public int GridPositionToIndex(Vector2Int position)
        => GridPositionToIndex(position.x, position.y);

    public int GridPositionToIndex(int x, int y)
        => y * m_size.x + x;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_cells == null)
            return;

        foreach (var cell in m_cells)
        {
            if (cell == null)
                continue;

            string text = $"R={m_accessLevels.GetAccessToRoad(cell)}\nW={m_accessLevels.GetAccessToWorkers(cell)}";
            Handles.Label(cell.transform.position, text);
        }
    }
#endif // UNITY_EDITOR
}
