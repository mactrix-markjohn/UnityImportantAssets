using UnityEngine;

public enum CameraMode
{
    Move,
    Rotate
}
public class CameraController : MonoBehaviour
{
    public CameraMode Mode = CameraMode.Move;
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float zoomRate = 10.0f;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;
    //private Vector3 CameraPosition;
    //private Vector3 Targetposition;
    //private Vector3 MoveDistance;


    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void NewTarget(Transform newTarget)
    {
        target = newTarget;
        Init();
    }


    public void Init()
    {
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    void LateUpdate()
    {
        HandleInput();
        Rotate();
        Move();
    }

    private void Move()
    {
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        position = target.position - (rotation * Vector3.forward * currentDistance);
        position = position - targetOffset;
        transform.position = position;
    }

    private void Rotate()
    {
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;
        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;
    }

    private void HandleInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonDown(1))
            {
                FirstPosition = Input.mousePosition;
                lastOffset = targetOffset;
            }

            if (Input.GetMouseButton(1))
            {
                SecondPosition = Input.mousePosition;
                delta = SecondPosition - FirstPosition;

                if (Mode == CameraMode.Rotate)
                {
                    if (delta.magnitude > .1)
                    {
                        xDeg += delta.x * xSpeed / 2 * 0.002f;
                        yDeg -= delta.y * ySpeed / 2 * 0.002f;
                        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                    }
                }
                else if (Mode == CameraMode.Move)
                {
                    targetOffset = lastOffset + transform.right * delta.x * 0.003f + transform.up * delta.y * 0.003f;
                }
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount == 2)
            { // Zoom
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
                float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;
                float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

                desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
            }

            if (Mode == CameraMode.Rotate)
            {
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
                { // Orbit
                    Vector2 touchposition = Input.GetTouch(0).deltaPosition;
                    xDeg += touchposition.x * xSpeed * 0.002f;
                    yDeg -= touchposition.y * ySpeed * 0.002f;
                    yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                }
            }
            else if (Mode == CameraMode.Move)
            {
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    lastOffset = targetOffset;
                    Vector2 touchposition = Input.GetTouch(0).deltaPosition;
                    targetOffset = lastOffset + transform.right * touchposition.x * 0.003f + transform.up * touchposition.y * 0.003f;
                }
            }

        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}