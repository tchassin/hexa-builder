public class DestroyModeClickHandler : IGridClickHandler
{
    private GameUI m_gameUI;

    public DestroyModeClickHandler()
    {
        m_gameUI = Object.FindObjectOfType<GameUI>();
    }

    public void OnCellHoverBegin(HexCell cell) { }

    public void OnCellHoverEnd(HexCell cell) { }

    public void OnLeftClickBegin(HexCell cell)
    {
        if (cell == null || cell.content == null)
            return;

        cell.SetContent(null);
    }

    public void OnLeftClickEnd(HexCell cell) { }

    public void OnRightClickBegin(HexCell cell)
    {
        m_gameUI.ToggleSelectMode();
    }

    public void OnRightClickEnd(HexCell cell) { }
}
