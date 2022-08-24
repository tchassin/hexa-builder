using System.Collections.Generic;
using UnityEngine;

public enum TerrainType : int
{
    Water,
    Ground,
}

public class Map : MonoBehaviour
{
    [SerializeField] private Cell m_cellPrefab;

    public int width => m_size.x;
    public int depth => m_size.y;

    private Vector2Int m_size;
    private readonly List<Cell> m_cells = new List<Cell>();

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
    }

    private void Update()
    {
        bool hasLeftClicked = Input.GetMouseButtonDown(0);
        bool hasRightClicked = Input.GetMouseButtonDown(1);
        bool hasClicked = hasLeftClicked || hasRightClicked;
        if (hasClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
                return;

            var cell = m_cells.Find(cell => cell.gameObject == hit.transform.gameObject);
            if (cell == null)
                return;

            if (hasLeftClicked)
            {
                var newTerrainType = (cell.terrainType == TerrainType.Water) ? TerrainType.Ground : TerrainType.Water;
                cell.SetTerrainType(newTerrainType);
                if (newTerrainType == TerrainType.Water && cell.hasRoad)
                    cell.RemoveRoad();
            }

            if (hasRightClicked)
            {
                if (cell.hasRoad)
                    cell.RemoveRoad();
                else
                    cell.AddRoad();
            }
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

        // Set terrain data
        for (int i = 0; i < cellCount; i++)
        {
            int x = i % width;
            int y = i / width;

            // Instantiate cell and set position
            Vector3 position = HexagonUtils.HexGridToWorldPosition(x, y);
            var cell = Instantiate(m_cellPrefab, position, Quaternion.identity, transform);
            cell.Initialize(new Vector2Int(x, y), (terrainData[y, x] != 0) ? TerrainType.Ground : TerrainType.Water);
            cell.gameObject.name = $"Cell ({x}; {y})";
            m_cells.Add(cell);

            if (x > 0)
                cell.SetNeighbor(m_cells[i - 1], HexDirection.W);

            if (y > 0)
            {
                if ((y & 1) == 0) // even rows
                {
                    cell.SetNeighbor(m_cells[i - width], HexDirection.NE);
                    if (x > 0)
                        cell.SetNeighbor(m_cells[i - width - 1], HexDirection.NW);
                }
                else // odd rows
                {
                    cell.SetNeighbor(m_cells[i - width], HexDirection.NW);
                    if (x < width - 1)
                        cell.SetNeighbor(m_cells[i - width + 1], HexDirection.NE);
                }
            }
        }
    }

    public Vector2Int IndexToGridPosition(int id)
        => new Vector2Int(id % width, id / width);

    public int GridPositionToIndex(Vector2Int position)
        => GridPositionToIndex(position.x, position.y);

    public int GridPositionToIndex(int x, int y)
        => y * width + x;
}
