using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private UICell m_uiCellPrefab;
    [SerializeField] private Canvas m_gridCanvas;
    [SerializeField] private ToggleGroup m_toggleGroup;
    [SerializeField] private CanvasGroup m_toolbar;

    [Header("Population")]
    [SerializeField] private TextMeshProUGUI m_populationLabel;
    [SerializeField] private TextMeshProUGUI m_jobsLabel;

    [Header("Resources")]
    [SerializeField] private ResourceData m_gold;
    [SerializeField] private TextMeshProUGUI m_goldLabel;
    [SerializeField] private ResourceData m_wood;
    [SerializeField] private TextMeshProUGUI m_woodLabel;
    [SerializeField] private ResourceData m_food;
    [SerializeField] private TextMeshProUGUI m_foodLabel;

    public Tooltip tooltip => m_tooltip;

    private readonly List<UICell> m_uiCells = new List<UICell>();
    private IGridClickHandler m_clickHandler;
    private HexCell m_lastHighlightedCell;
    private HexGrid m_grid;
    private Tooltip m_tooltip;

    private void Awake()
    {
        m_tooltip = FindObjectOfType<Tooltip>();
    }

    private void Start()
    {
        m_clickHandler = new SelectionClickHandler();
    }

    private void Update()
    {
        if (m_populationLabel)
            m_populationLabel.text = $"{Player.instance.population}/{Player.instance.maxPopulation}";

        if (m_jobsLabel)
            m_jobsLabel.text = $"{Player.instance.assignedJobs}/{Player.instance.totalJobs}";

        // TODO: proper resource display
        if (m_goldLabel != null && m_gold != null)
            m_goldLabel.text = $"{Player.instance.resources.GetResource(m_gold)}";

        if (m_woodLabel != null && m_wood != null)
            m_woodLabel.text = $"{Player.instance.resources.GetResource(m_wood)}";

        if (m_foodLabel != null && m_food != null)
            m_foodLabel.text = $"{Player.instance.resources.GetResource(m_food)}";

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

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public HexCell GetCellUnderMouse()
    {
        HexCell cell = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            hit.transform.TryGetComponent(out cell);

        return cell;
    }

    public void ToggleSelectMode()
    {
        if (m_toggleGroup.AnyTogglesOn())
            m_toggleGroup.SetAllTogglesOff();
        else
            SetClickHandler(null);
    }

    public void ToggleTerrainMode(bool isEnabled)
    {
        if (isEnabled)
            SetClickHandler(new TerrainClickHandler());
        else
            SetClickHandler(null);
    }

    public void ToggleBuildRoadMode(RoadData roadData)
    {
        if (roadData != null)
        {
            if (m_clickHandler is RoadModeClickHandler builder && builder.roadData == roadData)
                SetClickHandler(null);
            else
                SetClickHandler(new RoadModeClickHandler(roadData));
        }
        else
        {
            SetClickHandler(null);
        }
    }

    public void TogglePropMode(PropData propData)
    {
        if (propData != null)
        {
            if (m_clickHandler is PropsClickHandler builder && builder.propData == propData)
                SetClickHandler(null);
            else
                SetClickHandler(new PropsClickHandler(propData));
        }
        else
        {
            SetClickHandler(null);
        }
    }

    public void ToggleBuildMode(BuildingData buildingData)
    {
        if (buildingData != null)
        {
            if (m_clickHandler is BuildModeClickHandler builder && builder.buildingData == buildingData)
                SetClickHandler(null);
            else
                SetClickHandler(new BuildModeClickHandler(buildingData));
        }
        else
        {
            SetClickHandler(null);
        }
    }

    public void ToggleDestroyMode(bool isEnabled)
    {
        SetClickHandler(isEnabled ? new DestroyModeClickHandler() : null);
    }

    private void SetClickHandler(IGridClickHandler clickHandler)
    {
        m_clickHandler = clickHandler ?? new SelectionClickHandler();

        bool isSelection = m_clickHandler is SelectionClickHandler;
        m_toolbar.alpha = isSelection ? 1.0f : 0.0f;
        m_toolbar.interactable = isSelection;
        m_toolbar.blocksRaycasts = isSelection;

        // Hide tooltip if we were placing buildings
        m_tooltip.Hide();

        // Exit building mode if necessary
        if (m_clickHandler is not BuildModeClickHandler && m_clickHandler is not PropsClickHandler)
            BuildModeManager.instance.LeaveBuildMode();
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
