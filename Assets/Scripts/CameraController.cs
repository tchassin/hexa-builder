using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_topPadding = 2.0f;
    [SerializeField] private float m_sidePadding = 2.0f;
    [SerializeField] private float m_bottomPadding = 2.0f;
    [SerializeField] private Vector2 m_moveSpeed = Vector2.one;

    [Header("Zoom")]
    [SerializeField] private float m_defaultOrthoSize = 4.0f;
    [SerializeField] private float m_minOrthoSize = 2.0f;
    [SerializeField] private float m_maxOrthoSize = 8.0f;
    [SerializeField] private float m_zoomSpeed = 1.0f;

    private Camera m_camera;
    private Rect m_bounds;
    private Vector2 m_initialPlanePosition;

    private void Awake()
    {
        TryGetComponent(out m_camera);
        m_camera.orthographic = true;
        m_camera.orthographicSize = m_defaultOrthoSize;
        m_initialPlanePosition = new Vector2(transform.position.x, transform.position.z);
    }

    public void Start()
    {
        var grid = FindObjectOfType<HexGrid>();
        Debug.Assert(grid != null, this);

        // Invert Y axis for camera
        var gridBounds = grid.worldBounds;
        gridBounds.position = new Vector2(gridBounds.x, -gridBounds.y);
        gridBounds.size = new Vector2(gridBounds.width, -gridBounds.height);

        m_bounds = new Rect(m_initialPlanePosition + gridBounds.position, gridBounds.size);
    }

    private void Update()
    {
        // Handle zoom before movement so the position can be adjusted after zooming
        HandleZoom();
        HandleMove();
    }

    private void HandleMove()
    {
        float halfVerticalExtent = m_camera.orthographicSize;
        float zMin = m_bounds.yMin - m_bottomPadding + halfVerticalExtent;
        float zMax = m_bounds.yMax + m_topPadding - halfVerticalExtent;

        float halfHorizontalExtent = halfVerticalExtent * m_camera.aspect;
        float xMin = m_bounds.xMin - m_sidePadding + halfHorizontalExtent;
        float xMax = m_bounds.xMax + m_sidePadding - halfHorizontalExtent;

        Vector3 targetPosition = transform.position;
        Vector2 scaledSpeed = m_moveSpeed * Time.deltaTime;
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * scaledSpeed;

        targetPosition.x = (xMin < xMax) ? Mathf.Clamp(targetPosition.x + movement.x, xMin, xMax) : (xMin + xMax) * 0.5f;
        targetPosition.z = (zMin < zMax) ? Mathf.Clamp(targetPosition.z + movement.y, zMin, zMax) : (zMin + zMax) * 0.5f;

        transform.position = targetPosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0.0f)
            return;

        // By default, the zoom get faster as we zoom in, and slower when zooming out.
        // This compensate the effect to make zooming smoother
        float speedFactor = m_camera.orthographicSize / m_defaultOrthoSize;
        float orthographicSize = m_camera.orthographicSize - scroll * m_zoomSpeed * speedFactor;
        m_camera.orthographicSize = Mathf.Clamp(orthographicSize, m_minOrthoSize, m_maxOrthoSize);
    }
}
