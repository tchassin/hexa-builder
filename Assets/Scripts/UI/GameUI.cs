using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private UICell m_uiCellPrefab;
    [SerializeField] private Canvas m_gridCanvas;

    [Header("Label")]
    [SerializeField] private TextMeshProUGUI m_goldLabel;
    [SerializeField] private TextMeshProUGUI m_populationLabel;

    private readonly List<UICell> m_uiCells = new List<UICell>();
    private IGridClickHandler m_clickHandler;
    private HexCell m_lastHighlightedCell;
    private HexGrid m_grid;

    private void Update()
    {
        if (m_goldLabel)
            m_goldLabel.text = Player.instance.gold.ToString();

        if (m_populationLabel)
            m_populationLabel.text = Player.instance.population.ToString();

        if (m_clickHandler == null)
            return;

        HexCell cell = GetCellUnderMouse();
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

    public HexCell GetCellUnderMouse()
    {
        HexCell cell = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            hit.transform.TryGetComponent(out cell);

        return cell;
    }

    public void ToggleTerrainMode(bool isEnabled)
    {
        m_clickHandler = isEnabled ? new TerrainClickHandler() : null;
    }

    public void ToggleBuildRoadMode(RoadData roadData)
    {
        m_clickHandler = roadData != null
            ? m_clickHandler is BuildModeClickHandler builder && builder.buildingData == roadData
                ? null
                : new RoadModeClickHandler(roadData)
            : null;
    }
    public void ToggleAddTreeMode(bool isEnabled)
    {
        m_clickHandler = isEnabled ? new PropsClickHandler(BuildModeManager.instance.treePrefab) : null;
    }

    public void ToggleBuildMode(BuildingData buildingData)
    {
        m_clickHandler = buildingData != null
            ? m_clickHandler is BuildModeClickHandler builder && builder.buildingData == buildingData
                ? null
                : new BuildModeClickHandler(buildingData)
            : null;
    }

    public UICell GetUICell(HexCell cell)
        => m_uiCells.Find(uiCell => uiCell.cell == cell);

    public void Initialize()
    {
        m_grid = FindObjectOfType<HexGrid>();
        var size = m_grid.size;
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
            uiCell.SetCell(m_grid.GetCell(i));

            m_uiCells.Add(uiCell);
        }
    }
}
