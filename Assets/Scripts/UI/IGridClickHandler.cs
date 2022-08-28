public interface IGridClickHandler
{
    public void OnLeftClickBegin(Cell cell);
    public void OnLeftClickEnd(Cell cell);
    public void OnRightClickBegin(Cell cell);
    public void OnRightClickEnd(Cell cell);
    public void OnCellHoverBegin(Cell cell);
    public void OnCellHoverEnd(Cell cell);
}
