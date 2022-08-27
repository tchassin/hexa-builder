using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private UICell m_uiCellPrefab;
    [SerializeField] private Canvas m_gridCanvas;

    private readonly List<UICell> m_uiCells = new List<UICell>();
    private IGridClickHandler m_clickHandler;

    private void Update()
    {
        if (m_clickHandler == null)
            return;

        bool hasLeftClicked = Input.GetMouseButtonDown(0);
        bool hasRightClicked = Input.GetMouseButtonDown(1);
        bool hasClicked = hasLeftClicked || hasRightClicked;
        if (hasClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
                return;

            hit.transform.TryGetComponent(out Cell cell);
            if (hasLeftClicked)
                m_clickHandler.OnLeftClick(cell);
            else
                m_clickHandler.OnRightClick(cell);
        }
    }

    public void ToggleTerrainMode(bool isEnabled)
    {
        m_clickHandler = isEnabled ? new TerrainClickHandler() : null;
    }

    public void ToggleBuildRoadMode(bool isEnabled)
    {
        m_clickHandler = isEnabled ? new RoadClickHandler() : null;
    }

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
            var uiCell = Instantiate(m_uiCellPrefab, m_gridCanvas.transform);
            uiCell.SetCell(map.GetCell(i));

            m_uiCells.Add(uiCell);
        }
    }
}
