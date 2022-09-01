using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICell : MonoBehaviour
{
    public enum State
    {
        None,
        Selected,
        Invalid,
    }

    [SerializeField] private Color m_defaultColor = Color.white;
    [SerializeField] private Color m_validSelectionColor = Color.white;
    [SerializeField] private Color m_invalidSelectionColor = Color.white;
    [SerializeField] private Color m_highlightColor = Color.red;

    public HexCell cell => m_cell;

    private HexCell m_cell;
    private Image m_image;
    private State m_state = State.None;
    private bool m_isHighlighted = false;

    private void Awake()
    {
        TryGetComponent(out m_image);
    }

    public void SetCell(HexCell cell)
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

        UpdateColor();
    }

    public void UpdatePosition()
    {
        transform.position = cell.transform.position + Vector3.up * 0.01f;
    }

    public void SetState(State state)
    {
        if (state == m_state)
            return;

        m_state = state;
        UpdateColor();
    }

    private void BeginHighlight()
    {
        m_isHighlighted = true;
        UpdateColor();
    }

    private void EndHighlight()
    {
        m_isHighlighted = false;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (m_isHighlighted)
            m_image.color = m_highlightColor;
        else if (m_state == State.Selected)
            m_image.color = m_validSelectionColor;
        else if (m_state == State.Invalid)
            m_image.color = m_invalidSelectionColor;
        else
            m_image.color = m_defaultColor;
    }
}
