using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Angle preset (Clash-of-Clans like)")]
    public bool usePresetAngle = true;
    [Range(0f, 85f)] public float tilt = 50f;
    [Range(0f, 360f)] public float rotationY = 45f;

    [Header("Panning (drag)")]
    public int mouseButtonToDrag = 1;
    public float dragSpeed = 1f;
    public float dragSmoothTime = 0.08f;
    public bool invertDrag = false;

    [Header("Keyboard pan")]
    public bool allowKeyboardPan = true;
    public float keyboardSpeed = 10f;

    [Header("Zoom (camera moves forward/back on XZ plane)")]
    public float zoomSpeed = 20f;
    // Ces valeurs devraient être les limites en Y de la caméra.
    public float minDistance = 5f;   // Hauteur minimale (Zoom In Max)
    public float maxDistance = 60f;  // Hauteur maximale (Dézoom Max)

    [Header("Limits on world plane (X,Z)")]
    public Vector2 limitX = new Vector2(-50f, 50f);
    public Vector2 limitZ = new Vector2(-50f, 50f);

    Camera cam;
    Vector3 targetPosition;
    Vector3 velocityPos; // Utilisée par Vector3.SmoothDamp
    Vector2 lastMousePos;

    // position initiale projetée en XZ, utilisée pour clamp de zoom (optionnel)
    Vector2 startPositionXZ;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (usePresetAngle)
            transform.rotation = Quaternion.Euler(tilt, rotationY, 0f);

        targetPosition = transform.position;
        // On s'assure que la cible Y de départ est dans les limites
        targetPosition.y = Mathf.Clamp(targetPosition.y, minDistance, maxDistance);

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

        // 0.01f est un petit epsilon pour compenser l'imprécision des flottants

        // Blocage au Dézoom Max (scroller vers l'arrière, scroll < 0, hauteur max atteinte)
        bool isBlockedAtMax = (targetPosition.y >= maxDistance - 0.01f && scroll < 0);

        // Blocage au Zoom Max (scroller vers l'avant, scroll > 0, hauteur min atteinte)
        bool isBlockedAtMin = (targetPosition.y <= minDistance + 0.01f && scroll > 0);

        if (isBlockedAtMin || isBlockedAtMax)
        {
            // Annuler l'inertie sur les 3 axes pour stopper tout glissement (y compris xz)
            // L'inertie XZ vient souvent du drag, mais si le zoom est bloqué, l'inertie peut devenir visible.
            velocityPos = Vector3.zero;

            // On s'assure que targetPosition.y est sur la limite.
            targetPosition.y = Mathf.Clamp(targetPosition.y, minDistance, maxDistance);

            // Bloquer la molette : on quitte sans appliquer le zoomDelta.
            return;
        }

        // --- Si non bloqué, on applique le mouvement ---

        Vector3 zoomDelta = transform.forward * scroll * zoomSpeed;
        targetPosition += zoomDelta;

        targetPosition.y = Mathf.Clamp(targetPosition.y, minDistance, maxDistance);
    }


    void ApplySmoothing()
    {
        // On lisse uniquement la position (X,Y,Z). La rotation est fixée par le preset.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityPos, dragSmoothTime);
    }

    void ClampPosition()
    {
        float targetXBeforeClamp = targetPosition.x;
        float targetZBeforeClamp = targetPosition.z;

        targetPosition.x = Mathf.Clamp(targetPosition.x, limitX.x, limitX.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, limitZ.x, limitZ.y);

        // Annuler la vélocité XZ si la limite de la carte (X/Z) a été atteinte (pour stopper le glissement du drag aux limites XZ)
        if (Mathf.Abs(targetPosition.x - targetXBeforeClamp) > 0.001f)
        {
            velocityPos.x = 0f;
        }
        if (Mathf.Abs(targetPosition.z - targetZBeforeClamp) > 0.001f)
        {
            velocityPos.z = 0f;
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
        pos.z = Mathf.Clamp(pos.z, limitZ.x, limitZ.y);
        transform.position = pos;
    }

    // Centrer la caméra sur un point (préserve la hauteur/current Y)
    public void CenterOn(Vector3 worldPos)
    {
        targetPosition = new Vector3(worldPos.x, targetPosition.y, worldPos.z);
    }
}



