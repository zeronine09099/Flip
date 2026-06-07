using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMoveScript : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private Camera targetCamera;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 15f;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    // WASD로 카메라 이동
    private void MoveCamera()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A, D
        float vertical = Input.GetAxisRaw("Vertical");     // W, S

        Vector3 moveDirection =
            transform.right * horizontal +
            transform.up * vertical;

        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }

    // 마우스 휠로 카메라 확대 / 축소
    private void ZoomCamera()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll == 0f)
        {
            return;
        }

        if (targetCamera.orthographic)
        {
            targetCamera.orthographicSize -= scroll * zoomSpeed;
            targetCamera.orthographicSize = Mathf.Clamp(
                targetCamera.orthographicSize,
                minZoom,
                maxZoom
            );
        }
        else
        {
            targetCamera.fieldOfView -= scroll * zoomSpeed;
            targetCamera.fieldOfView = Mathf.Clamp(
                targetCamera.fieldOfView,
                minZoom,
                maxZoom
            );
        }
    }
}