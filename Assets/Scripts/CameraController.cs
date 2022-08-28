using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rect m_cameraBounds;
    [SerializeField] private Vector2 m_moveSpeed = Vector2.one;

    [Header("Zoom")]
    [SerializeField] private float m_defaultOrthoSize = 4.0f;
    [SerializeField] private float m_minOrthoSize = 2.0f;
    [SerializeField] private float m_maxOrthoSize = 8.0f;
    [SerializeField] private float m_zoomSpeed = 1.0f;

    private Camera m_camera;

    private void Awake()
    {
        TryGetComponent(out m_camera);
        m_camera.orthographic = true;
        m_camera.orthographicSize = m_defaultOrthoSize;
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
        float yMin = m_cameraBounds.yMin + halfVerticalExtent;
        float yMax = m_cameraBounds.yMax - halfVerticalExtent;

        float halfHorizontalExtent = halfVerticalExtent * m_camera.aspect;
        float xMin = m_cameraBounds.xMin + halfHorizontalExtent;
        float xMax = m_cameraBounds.xMax - halfHorizontalExtent;

        Vector3 targetPosition = transform.position;
        Vector2 scaledSpeed = m_moveSpeed * Time.deltaTime;
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * scaledSpeed;

        targetPosition.x = (xMin < xMax) ? Mathf.Clamp(targetPosition.x + movement.x, xMin, xMax) : (xMin + xMax) * 0.5f;
        targetPosition.z = (yMin < yMax) ? Mathf.Clamp(targetPosition.z + movement.y, yMin, yMax) : (yMin + yMax) * 0.5f;

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
        m_camera.orthographicSize = Mathf.Clamp(orthographicSize, 1 / m_minOrthoSize, m_maxOrthoSize);
    }
}
