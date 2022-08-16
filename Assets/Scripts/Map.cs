using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject m_tilePrefab;
    [SerializeField] private Material m_groundMaterial;
    [SerializeField] private Material m_waterMaterial;

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

    public void Generate(int[,] terrainData)
    {
        int width = terrainData.GetLength(0);
        int height = terrainData.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isGround = terrainData[y, x] != 0;

                // Instantiate tile and set position
                Vector3 position = HexagonUtils.HexGridToWorldPosition(x, y);
                var tile = Instantiate(m_tilePrefab, position, Quaternion.identity, transform);

                // Add noise for ground tiles
                float tileHeight = isGround ? 1.0f : 0.8f;
                tile.transform.localScale = new Vector3(1.0f, tileHeight, 1.0f);

                // Set material
                if (tile.TryGetComponent(out Renderer renderer))
                    renderer.material = isGround ? m_groundMaterial : m_waterMaterial;
            }
        }
    }
}
