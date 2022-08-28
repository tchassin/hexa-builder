using System.Collections.Generic;
using UnityEngine;

public class RoadClickHandler : IGridClickHandler
{
    private bool isPlacingRoads => m_selectedCells.Count > 0;

    private Cell m_start;
    private Map m_map;
    private GameUI m_gameUI;
    private readonly List<UICell> m_selectedCells = new List<UICell>();

    public RoadClickHandler()
    {
        m_map = Object.FindObjectOfType<Map>();
        m_gameUI = Object.FindObjectOfType<GameUI>();
    }

    public void OnCellHoverBegin(Cell cell)
    {
        if (!isPlacingRoads)
            return;

        if (cell == null)
            return;

        foreach (var uiCell in m_selectedCells)
            uiCell.SetState(UICell.State.None);
        m_selectedCells.Clear();

        var path = m_map.ShortestPath(m_start, cell, c => c.DistanceTo(cell));
        foreach (var pathCell in path)
        {
            var uiCell = m_gameUI.GetUICell(pathCell);
            uiCell.SetState((pathCell.terrainType == TerrainType.Ground && !pathCell.hasRoad)
                ? UICell.State.Selected
                : UICell.State.Invalid);

            m_selectedCells.Add(uiCell);
        }
    }

    public void OnCellHoverEnd(Cell cell) { }

    public void OnLeftClickBegin(Cell cell)
    {
        if (cell == null)
            return;

        m_start = cell;

        var uiCell = m_gameUI.GetUICell(cell);
        m_selectedCells.Add(uiCell);
    }

    public void OnLeftClickEnd(Cell cell)
    {
        if (!isPlacingRoads)
            return;

        foreach (var uiCell in m_selectedCells)
        {
            if (uiCell.cell.terrainType == TerrainType.Ground && !uiCell.cell.hasRoad)
                uiCell.cell.AddRoad();
        }

        EndBuildMode();
    }

    public void OnRightClickBegin(Cell cell) { }

    public void OnRightClickEnd(Cell cell)
    {
        if (isPlacingRoads)
        {
            EndBuildMode();
            return;
        }

        if (cell == null || !cell.hasRoad)
            return;

        cell.RemoveRoad();
    }

    private void EndBuildMode()
    {
        foreach (var uiCell in m_selectedCells)
            uiCell.SetState(UICell.State.None);
        m_selectedCells.Clear();
    }
}
