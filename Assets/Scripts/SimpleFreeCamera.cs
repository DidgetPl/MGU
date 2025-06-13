using UnityEngine;

public class SimpleFreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float verticalSpeed = 5f;
    public float mouseSensitivity = 2f;

    public float zoomSpeed = 50f;
    public float minFOV = 20f;
    public float maxFOV = 90f;

    private float pitch = 0f;
    private float yaw = 0f;

    public bool canMove = true;

    private Camera cam;
    private int screenshotCount = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (canMove)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = (transform.forward * moveZ + transform.right * moveX) * moveSpeed;

            if (Input.GetKey(KeyCode.Q))
                move += Vector3.up * verticalSpeed;
            if (Input.GetKey(KeyCode.E))
                move += Vector3.down * verticalSpeed;

            transform.position += move * Time.deltaTime;
        }

        HandleZoom();

        if (Input.GetKeyDown(KeyCode.F12))
        {
            string filename = $"Screenshots/Screenshot_{screenshotCount}.png";
            ScreenCapture.CaptureScreenshot(filename);
            screenshotCount++;
            Debug.Log("Zrzut ekranu zapisany: " + filename);
        }
    }

    void HandleZoom()
    {
        if (cam != null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                cam.fieldOfView -= zoomSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                cam.fieldOfView += zoomSpeed * Time.deltaTime;
            }

            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
}
