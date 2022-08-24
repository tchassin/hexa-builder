using UnityEngine;

public class UICell : MonoBehaviour
{
    private Cell m_cell;

    public Cell cell => m_cell;

    public void SetCell(Cell cell)
    {
        if (m_cell == cell)
            return;

        if (m_cell != null)
            m_cell.onTerrainChanged.RemoveListener(UpdatePosition);

        m_cell = cell;
        UpdatePosition();

        m_cell.onTerrainChanged.AddListener(UpdatePosition);
    }

    public void UpdatePosition()
    {
        transform.position = cell.transform.position + Vector3.up * 0.01f;
    }
}
