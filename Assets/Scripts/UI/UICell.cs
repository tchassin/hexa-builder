using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICell : MonoBehaviour
{
    [SerializeField] private Color m_defaultColor = Color.white;
    [SerializeField] private Color m_highlightColor = Color.red;

    public Cell cell => m_cell;

    private Cell m_cell;
    private Image m_image;

    private void Awake()
    {
        TryGetComponent(out m_image);
    }

    public void SetCell(Cell cell)
    {
        if (m_cell == cell)
            return;

        if (m_cell != null)
        {
            m_cell.onTerrainChanged.RemoveListener(UpdatePosition);
            m_cell.onMouseEnter.RemoveListener(BeginHighlight);
            m_cell.onMouseExit.RemoveListener(EndHighlight);
        }

        m_cell = cell;
        UpdatePosition();

        m_cell.onTerrainChanged.AddListener(UpdatePosition);
        m_cell.onMouseEnter.AddListener(BeginHighlight);
        m_cell.onMouseExit.AddListener(EndHighlight);
    }

    public void UpdatePosition()
    {
        transform.position = cell.transform.position + Vector3.up * 0.01f;
    }

    private void BeginHighlight()
    {
        m_image.color = m_highlightColor;
    }

    private void EndHighlight()
    {
        m_image.color = m_defaultColor;
    }
}
