using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private UICell m_uiCellPrefab;
    [SerializeField] private Canvas m_gridCanvas;

    private readonly List<UICell> m_uiCells = new List<UICell>();
    private IGridClickHandler m_clickHandler;
    private Cell m_lastHighlightedCell;

    private void Update()
    {
        if (m_clickHandler == null)
            return;

        Cell cell = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            hit.transform.TryGetComponent(out cell);

        if (m_lastHighlightedCell != cell)
        {
            m_clickHandler.OnCellHoverEnd(m_lastHighlightedCell);
            m_clickHandler.OnCellHoverBegin(cell);
            m_lastHighlightedCell = cell;
        }

        if (Input.GetMouseButtonDown(0))
            m_clickHandler.OnLeftClickBegin(cell);
        else if (Input.GetMouseButtonUp(0))
            m_clickHandler.OnLeftClickEnd(cell);
        else if (Input.GetMouseButtonDown(1))
            m_clickHandler.OnRightClickBegin(cell);
        else if (Input.GetMouseButtonUp(1))
            m_clickHandler.OnRightClickEnd(cell);
    }

    public void ToggleTerrainMode(bool isEnabled)
    {
        m_clickHandler = isEnabled ? new TerrainClickHandler() : null;
    }

    public void ToggleBuildRoadMode(bool isEnabled)
    {
        m_clickHandler = isEnabled ? new RoadClickHandler() : null;
    }

    public UICell GetUICell(Cell cell)
        => m_uiCells.Find(uiCell => uiCell.cell == cell);

    public void Initialize()
    {
        var map = FindObjectOfType<Map>();
        var size = map.size;
        int cellCount = size.x * size.y;

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
