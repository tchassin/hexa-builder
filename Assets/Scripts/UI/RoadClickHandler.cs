public class RoadClickHandler : IGridClickHandler
{
    public void OnLeftClick(Cell cell)
    {
        if (cell == null || cell.hasRoad)
            return;

        cell.AddRoad();
    }

    public void OnRightClick(Cell cell)
    {
        if (cell == null || !cell.hasRoad)
            return;

        cell.RemoveRoad();
    }
}
