using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour
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
    [HideInInspector] public Dictionary<string, bool> targetSpotted = new Dictionary<string, bool>();
    string pythonScriptPath = "Assets/Python/classify_image.py";
    private string tempPath = "TempScreenshots";

    public GameObject uiRoot;
    public NotificationManager np;
    GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string screenshotFolderPath = Path.Combine(projectRoot, "TempScreenshots");
        if (Directory.Exists(screenshotFolderPath))
        {
            string[] files = Directory.GetFiles(screenshotFolderPath);
            screenshotCount = files.Length;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
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

        if (Input.GetKeyDown(KeyCode.Mouse0))
            StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        if (uiRoot != null)
            uiRoot.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        byte[] imageBytes = screenImage.EncodeToPNG();
        string filename = $"TempScreenshots/Screenshot_{screenshotCount}.png";
        File.WriteAllBytes(filename, imageBytes);

        if (uiRoot != null)
            uiRoot.SetActive(true);

        np.ShowNotification($"Zrzut ekranu zapisany: Screenshot_{screenshotCount}.png");

        screenshotCount++;
        StartCoroutine(WaitForScreenshotAndClassify(filename, $"Screenshot_{screenshotCount}"));
    }

    void HandleZoom()
    {
        if (cam != null)
        {
            if (Input.GetKey(KeyCode.Space))
                cam.fieldOfView -= zoomSpeed * Time.deltaTime;
            
            if (Input.GetKey(KeyCode.LeftShift))
                cam.fieldOfView += zoomSpeed * Time.deltaTime;

            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    private IEnumerator WaitForScreenshotAndClassify(string path, string screenshotName)
    {
        yield return new WaitForSeconds(1f);
       
        if (!File.Exists(pythonScriptPath))
        {
            UnityEngine.Debug.LogError($"Nie znaleziono skryptu Pythona: {pythonScriptPath}");
            yield break;
        }

        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError($"Nie znaleziono zrzutu ekranu: {path}");
            yield break;
        }

        Task<string> classificationTask = Task.Run(() => RunPythonClassifier(path));

        while (!classificationTask.IsCompleted)
            yield return null;

        if (classificationTask.Exception != null)
        {
            UnityEngine.Debug.LogError($"Błąd klasyfikacji: {classificationTask.Exception}");
        }
        else
        {
            string result = classificationTask.Result;
            string[] splittedResults = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string label = splittedResults[^1];
            np.ShowNotification($"{screenshotName}: {label}");
            if(targetSpotted.ContainsKey(label))
                if (!targetSpotted[label])
                {
                    targetSpotted[label] = true;
                    for (int i = 0; i < 4; i++)
                    {
                        TextMeshProUGUI tmp = gm.checklist.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                        if (tmp.text == label)
                        {
                            tmp.text = $"<s>{tmp.text}</s>";
                            tmp.color = new Color(0.6f, 0.6f, 0.6f);
                        }
                    }
                    if (targetSpotted.Values.Sum(v => v ? 1 : 0) == 4) gm.Endgame(true);
                }
        }
    }
    private string RunPythonClassifier(string imagePath)
    {
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"\"{pythonScriptPath}\" \"{imagePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                return reader.ReadToEnd().Trim();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (Directory.Exists(tempPath))
        {
            string[] files = Directory.GetFiles(tempPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            np.ShowNotification("Folder TempScreenshots wyczyszczony.");
        }
    }
}
