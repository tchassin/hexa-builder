public interface IGridClickHandler
{
    public void OnLeftClickBegin(HexCell cell);
    public void OnLeftClickEnd(HexCell cell);
    public void OnRightClickBegin(HexCell cell);
    public void OnRightClickEnd(HexCell cell);
    public void OnCellHoverBegin(HexCell cell);
    public void OnCellHoverEnd(HexCell cell);
}
