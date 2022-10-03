using UnityEngine;

public class SelectionClickHandler : IGridClickHandler
{
    private GameUI m_gameUI;
    private Building m_selectedBuilding;

    public SelectionClickHandler()
    {
        m_gameUI = Object.FindObjectOfType<GameUI>();
    }

    public void OnCellHoverBegin(HexCell cell)
    {
    }

    public void OnCellHoverEnd(HexCell cell)
    {
    }

    public void OnLeftClickBegin(HexCell cell)
    {
        if (cell != null && cell.content is Building building)
            SelectBuilding(building);
        else
            DeselectBuilding();
    }

    public void OnLeftClickEnd(HexCell cell)
    {
    }

    public void OnRightClickBegin(HexCell cell)
    {
        DeselectBuilding();
    }

    public void OnRightClickEnd(HexCell cell)
    {
    }

    private void SelectBuilding(Building building)
    {
        if (building == m_selectedBuilding)
            return;

        DeselectBuilding();

        m_selectedBuilding = building;

        if (building != null)
        {
            if (building.TryGetComponent(out BuildingStateDisplay stateDisplay))
                stateDisplay.OnSelected();

            m_gameUI.GetUICell(building.cell).SetState(UICell.State.Selected);
        }
    }

    private void DeselectBuilding()
    {
        if (m_selectedBuilding == null)
            return;

        m_gameUI.GetUICell(m_selectedBuilding.cell).SetState(UICell.State.None);
        m_gameUI.tooltip.Hide();

        m_selectedBuilding = null;
    }
}
