using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private UICell m_uiCellPrefab;
    [SerializeField] private Canvas m_canvas;

    private readonly List<UICell> m_uiCells = new List<UICell>();

    public void Initialize()
    {
        var map = FindObjectOfType<Map>();
        int width = map.width;
        int depth = map.depth;
        int cellCount = width * depth;

        // Clear previous objects and initialize array
        if (m_uiCells != null)
        {
            foreach (var cellObject in m_uiCells)
                Destroy(cellObject);
        }
        m_uiCells.Clear();

        // Initialize cells
        for (int i = 0; i < cellCount; i++)
        {
            var uiCell = Instantiate(m_uiCellPrefab, m_canvas.transform);
            uiCell.SetCell(map.GetCell(i));

            m_uiCells.Add(uiCell);
        }
    }
}
