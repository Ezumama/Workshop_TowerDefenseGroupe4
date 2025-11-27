using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Angle preset (Clash-of-Clans like)")]
    public bool usePresetAngle = true;
    [Range(0f, 85f)] public float tilt = 50f;
    [Range(0f, 360f)] public float rotationY = 45f;

    [Header("Panning (drag)")]
    public int mouseButtonToDrag = 0;
    public float dragSpeed = 1f;
    public float dragSmoothTime = 0.08f;
    public bool invertDrag = false;

    [Header("Keyboard pan")]
    public bool allowKeyboardPan = true;
    public float keyboardSpeed = 10f;

    [Header("Zoom (camera moves forward/back on XZ plane)")]
    public float zoomSpeed = 20f;
    public float minDistance = 5f;   // distance minimale (en unités XZ) depuis startPositionXZ
    public float maxDistance = 60f;  // distance maximale (en unités XZ) depuis startPositionXZ

    [Header("Limits on world plane (X,Z)")]
    public Vector2 limitX = new Vector2(-50f, 50f);
    public Vector2 limitZ = new Vector2(-50f, 50f);

    Camera cam;
    Vector3 targetPosition;
    Vector3 velocityPos;
    Vector2 lastMousePos;

    // position initiale projetée en XZ, utilisée pour clamp de zoom (optionnel)
    Vector2 startPositionXZ;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (usePresetAngle)
            transform.rotation = Quaternion.Euler(tilt, rotationY, 0f);

        targetPosition = transform.position;
        startPositionXZ = new Vector2(transform.position.x, transform.position.z);
    }

    void Update()
    {
        HandleMouseDrag();
        HandleKeyboardPan();
        HandleZoom();
        ApplySmoothing();
        ClampPosition();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(mouseButtonToDrag))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(mouseButtonToDrag))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 delta = mousePos - lastMousePos;
            lastMousePos = mousePos;

            if (delta.sqrMagnitude <= 0.000001f) return;

            Vector3 camForward = transform.forward; camForward.y = 0f; camForward.Normalize();
            Vector3 camRight = transform.right; camRight.y = 0f; camRight.Normalize();

            float screenFactor = Mathf.Max(Screen.width, Screen.height) / 1000f;
            float dir = invertDrag ? 1f : -1f;

            Vector3 worldMove = (camRight * delta.x + camForward * delta.y) *
                (dragSpeed * 0.01f) * screenFactor * dir;

            targetPosition += worldMove;
        }
    }

    void HandleKeyboardPan()
    {
        if (!allowKeyboardPan) return;

        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input += Vector3.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input += Vector3.back;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input += Vector3.right;

        if (input.sqrMagnitude > 0.001f)
        {
            Vector3 camForward = transform.forward; camForward.y = 0f; camForward.Normalize();
            Vector3 camRight = transform.right; camRight.y = 0f; camRight.Normalize();

            Vector3 move = (camRight * input.x + camForward * input.z).normalized *
                           keyboardSpeed * Time.deltaTime;

            targetPosition += move;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.00001f) return;

        // Déplacement de la caméra le long de son axe local Y (forward incliné)
        Vector3 zoomDelta = transform.forward * scroll * zoomSpeed;

        targetPosition += zoomDelta;

        // Si tu veux limiter la distance, tu peux clamp la hauteur locale
        targetPosition.y = Mathf.Clamp(targetPosition.y, minDistance, maxDistance);
    }



    void ApplySmoothing()
    {
        // On lisse uniquement la position (X,Y,Z). La rotation est fixée par le preset.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityPos, dragSmoothTime);
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
        pos.z = Mathf.Clamp(pos.z, limitZ.x, limitZ.y);
        transform.position = pos;

        targetPosition.x = Mathf.Clamp(targetPosition.x, limitX.x, limitX.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, limitZ.x, limitZ.y);
    }

    // Centrer la caméra sur un point (préserve la hauteur/current Y)
    public void CenterOn(Vector3 worldPos)
    {
        targetPosition = new Vector3(worldPos.x, targetPosition.y, worldPos.z);
    }
}



