using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ObjectMover : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Rigidbody rb;
    private Camera mainCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 0.1f;
    public float rotateSpeed = 300f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                ObjectMover mover = hit.collider.GetComponentInParent<ObjectMover>();
                if (mover != null && mover == this)
                {
                    StartDragging();
                }
            }
        }

        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPos() + offset;
            rb.MovePosition(Vector3.Lerp(transform.position, newPosition, moveSpeed));
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isDragging)
            {
                isDragging = false;
                Debug.Log($"✅ Stopped Dragging: {gameObject.name}");
            }
        }

        if (isDragging && (Keyboard.current.leftShiftKey.isPressed || Mouse.current.rightButton.isPressed))
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float rotateY = mouseDelta.x * rotateSpeed * Time.deltaTime;
            float rotateX = -mouseDelta.y * rotateSpeed * Time.deltaTime;

            Quaternion yaw = Quaternion.Euler(0f, rotateY, 0f);
            Quaternion pitch = Quaternion.Euler(rotateX, 0f, 0f);

            transform.rotation = yaw * transform.rotation * pitch;
        }
    }

    void StartDragging()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
        Debug.Log($"✅ Started Dragging: {gameObject.name}");
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
}
