using System.Collections.Generic;
using UnityEngine;

public enum TerrainType : int
{
    Water,
    Ground,
}

public class Map : MonoBehaviour
{
    [SerializeField] private Tile m_tilePrefab;

    public int width => m_size.x;
    public int depth => m_size.y;

    private Vector2Int m_size;
    private readonly List<Tile> m_tiles = new List<Tile>();

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

            var tile = m_tiles.Find(tile => tile.gameObject == hit.transform.gameObject);
            if (tile == null)
                return;

            if (hasLeftClicked)
            {
                var newTerrainType = (tile.terrainType == TerrainType.Water) ? TerrainType.Ground : TerrainType.Water;
                tile.SetTerrainType(newTerrainType);
                if (newTerrainType == TerrainType.Water && tile.hasRoad)
                    tile.RemoveRoad();
            }

            if (hasRightClicked)
            {
                if (tile.hasRoad)
                    tile.RemoveRoad();
                else
                    tile.AddRoad();
            }
        }
    }

    public void Generate(int[,] terrainData)
    {
        m_size = new Vector2Int(terrainData.GetLength(0), terrainData.GetLength(1));
        int tileCount = m_size.x * m_size.y;

        // Clear previous objects and initialize array
        if (m_tiles != null)
        {
            foreach (var tileObject in m_tiles)
                Destroy(tileObject);
        }
        m_tiles.Clear();

        // Set terrain data
        for (int i = 0; i < tileCount; i++)
        {
            int x = i % width;
            int y = i / width;

            // Instantiate tile and set position
            Vector3 position = HexagonUtils.HexGridToWorldPosition(x, y);
            var tile = Instantiate(m_tilePrefab, position, Quaternion.identity, transform);
            tile.Initialize(new Vector2Int(x, y), (terrainData[y, x] != 0) ? TerrainType.Ground : TerrainType.Water);
            tile.gameObject.name = $"Tile ({x}; {y})";
            m_tiles.Add(tile);

            if (x > 0)
                tile.SetNeighbor(m_tiles[i - 1], HexDirection.W);

            if (y > 0)
            {
                if ((y & 1) == 0) // even rows
                {
                    tile.SetNeighbor(m_tiles[i - width], HexDirection.NE);
                    if (x > 0)
                        tile.SetNeighbor(m_tiles[i - width - 1], HexDirection.NW);
                }
                else // odd rows
                {
                    tile.SetNeighbor(m_tiles[i - width], HexDirection.NW);
                    if (x < width - 1)
                        tile.SetNeighbor(m_tiles[i - width + 1], HexDirection.NE);
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
