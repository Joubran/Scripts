using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 10f, -10f);
    [SerializeField] private float positionLerpSpeed = 5f;
    [SerializeField] private bool useInstantMovement = false;
    [SerializeField] private float touchPanSensitivity = 1f;
    [SerializeField] private float touchZoomSensitivity = 0.01f;
    [SerializeField] private float touchRotationSensitivity = 0.5f;

    private Vector2 lastMousePos;
    private bool isPanning;

    [SerializeField, Tooltip("Adjusts panning speed based on zoom level.")]
    private float zoomPanFactor = 0f;

    [Header("Bounds")]
    public Vector2 xBound = new Vector2(-100, 100);
    public Vector2 zBound = new Vector2(-100, 100);

    [Header("Zoom Limits")]
    [SerializeField] private float minZoomDistance = 1f;
    [SerializeField] private float maxZoomDistance = 50f;

    private Vector3 targetPosition;
    private Quaternion currentRotation = Quaternion.identity;
    private Camera cam;
    private bool isTwoFinger = false;
    private float prevDistance;
    private float prevAngle;
    private float initialOffsetMag;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        float dist = cameraOffset.magnitude;
        dist = Mathf.Clamp(dist, minZoomDistance, maxZoomDistance);
        cameraOffset = cameraOffset.normalized * dist;
        initialOffsetMag = dist;

        targetPosition = transform.position;
        if (Mouse.current != null)
            lastMousePos = Mouse.current.position.ReadValue();
    }

    void Update()
    {
        HandleTouchInput();
        HandleEditorInput();
        UpdateCameraPosition();
    }

    private void HandleTouchInput()
    {
        foreach (UnityEngine.Touch touch in Input.touches)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;
        }
        var touches = Touch.activeTouches;
        int count = touches.Count;
        if (count == 0)
        {
            isTwoFinger = false;
            return;
        }

        if (count == 1)
        {
            var t = touches[0];
            if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                PanWithScreenDelta(t.screenPosition, t.delta);

            isTwoFinger = false;
        }
        else
        {
            var t0 = touches[0];
            var t1 = touches[1];
            Vector2 p0 = t0.screenPosition;
            Vector2 p1 = t1.screenPosition;
            Vector2 pp0 = p0 - t0.delta;
            Vector2 pp1 = p1 - t1.delta;

            float currDist = Vector2.Distance(p0, p1);
            float currAng  = Vector2.SignedAngle(p1 - p0, Vector2.right);

            if (!isTwoFinger)
            {
                isTwoFinger   = true;
                prevDistance  = currDist;
                prevAngle     = currAng;
                return;
            }

            float distDelta = currDist - prevDistance;
            float angDelta  = Mathf.DeltaAngle(prevAngle, currAng);

            HandleZoom(distDelta * touchZoomSensitivity);
            RotateCamera(angDelta * touchRotationSensitivity);

            prevDistance = currDist;
            prevAngle    = currAng;
        }
    }

    private void HandleEditorInput()
    {
        if (Mouse.current == null || Keyboard.current == null)
            return;

        float zoomDelta = 0f;
        if (Keyboard.current.wKey.isPressed)
            zoomDelta += touchZoomSensitivity * 100f;
        if (Keyboard.current.sKey.isPressed)
            zoomDelta -= touchZoomSensitivity * 100f;

        if (zoomDelta != 0f)
            HandleZoom(zoomDelta);

        float rot = 0f;
        if (Keyboard.current.eKey.isPressed) rot += touchRotationSensitivity * 5f;
        if (Keyboard.current.qKey.isPressed) rot -= touchRotationSensitivity * 5f;
        if (rot != 0f)
            RotateCamera(rot);
    }

    private void PanWithScreenDelta(Vector2 screenPos, Vector2 delta)
    {
        Vector2 prevScreen = screenPos - delta;
        Plane plane = new Plane(Vector3.up, targetPosition);
        Ray rCurr = cam.ScreenPointToRay(screenPos);
        Ray rPrev = cam.ScreenPointToRay(prevScreen);

        if (plane.Raycast(rCurr, out float enterC) && plane.Raycast(rPrev, out float enterP))
        {
            Vector3 worldDelta = rCurr.GetPoint(enterC) - rPrev.GetPoint(enterP);
            float zoomMul = Mathf.Pow(initialOffsetMag / cameraOffset.magnitude, zoomPanFactor);
            targetPosition -= worldDelta * touchPanSensitivity * zoomMul;
            targetPosition.x = Mathf.Clamp(targetPosition.x, xBound.x, xBound.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, zBound.x, zBound.y);
        }
    }

    private void HandleZoom(float zoomDelta)
    {
        float dist = cameraOffset.magnitude;
        float newDist = dist - zoomDelta;
        newDist = Mathf.Clamp(newDist, minZoomDistance, maxZoomDistance);
        cameraOffset = cameraOffset.normalized * newDist;
    }

    private void RotateCamera(float rotationDelta)
    {
        cameraOffset    = Quaternion.AngleAxis(-rotationDelta, Vector3.up) * cameraOffset;
        currentRotation = Quaternion.AngleAxis(-rotationDelta, Vector3.up) * currentRotation;
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPos = targetPosition + (currentRotation * cameraOffset);
        Quaternion desiredRot = Quaternion.LookRotation(targetPosition - desiredPos);

        if (useInstantMovement)
            transform.SetPositionAndRotation(desiredPos, desiredRot);
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPos, positionLerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, positionLerpSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 min = new Vector3(xBound.x, 0, zBound.x);
        Vector3 max = new Vector3(xBound.y, 0, zBound.y);
        Vector3 p1 = min;
        Vector3 p2 = new Vector3(max.x, 0, min.z);
        Vector3 p3 = max;
        Vector3 p4 = new Vector3(min.x, 0, max.z);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
}