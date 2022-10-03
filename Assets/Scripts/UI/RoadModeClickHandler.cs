using System.Collections.Generic;
using UnityEngine;

public class RoadModeClickHandler : IGridClickHandler
{
    public RoadData roadData => m_roadData;

    private bool isPlacingRoads => m_selectedCells.Count > 0;

    private RoadData m_roadData;
    private HexCell m_start;
    private HexGrid m_grid;
    private GameUI m_gameUI;
    private readonly List<UICell> m_selectedCells = new List<UICell>();

    public RoadModeClickHandler(RoadData roadData)
    {
        Debug.Assert(roadData != null);
        m_roadData = roadData;
        m_grid = Object.FindObjectOfType<HexGrid>();
        m_gameUI = Object.FindObjectOfType<GameUI>();
    }

    public void OnCellHoverBegin(HexCell cell)
    {
        if (!isPlacingRoads)
            return;

        if (cell == null)
            return;

        foreach (var uiCell in m_selectedCells)
            uiCell.SetState(UICell.State.None);
        m_selectedCells.Clear();

        var path = m_grid.ShortestPath(m_start, cell, c => c.DistanceTo(cell));
        foreach (var pathCell in path)
        {
            var uiCell = m_gameUI.GetUICell(pathCell);
            uiCell.SetState(m_roadData.CanBePlacedOn(pathCell) ? UICell.State.Selected : UICell.State.Invalid);

            m_selectedCells.Add(uiCell);
        }
    }

    public void OnCellHoverEnd(HexCell cell) { }

    public void OnLeftClickBegin(HexCell cell)
    {
        if (cell == null)
            return;

        m_start = cell;

        var uiCell = m_gameUI.GetUICell(cell);
        m_selectedCells.Add(uiCell);
    }

    public void OnLeftClickEnd(HexCell cell)
    {
        if (!isPlacingRoads)
            return;

        var path = new List<HexCell>(m_selectedCells.Count);
        foreach (var uiCell in m_selectedCells)
            path.Add(uiCell.cell);

        BuildModeManager.instance.PlaceRoads(m_roadData, path);

        EndBuildMode();
    }

    public void OnRightClickBegin(HexCell cell) { }

    public void OnRightClickEnd(HexCell cell)
    {
        if (isPlacingRoads)
            EndBuildMode();
        else
            m_gameUI.ToggleSelectMode();
    }

    private void EndBuildMode()
    {
        foreach (var uiCell in m_selectedCells)
            uiCell.SetState(UICell.State.None);
        m_selectedCells.Clear();
    }
}
