public class DestroyModeClickHandler : IGridClickHandler
{
    public void OnCellHoverBegin(HexCell cell) { }

    public void OnCellHoverEnd(HexCell cell) { }

    public void OnLeftClickBegin(HexCell cell)
    {
        if (cell == null || cell.content == null)
            return;

        cell.SetContent(null);
    }

    public void OnLeftClickEnd(HexCell cell) { }

    public void OnRightClickBegin(HexCell cell) { }

    public void OnRightClickEnd(HexCell cell) { }
}
