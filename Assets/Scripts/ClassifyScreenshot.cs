using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class ClassifyScreenshot : MonoBehaviour
{
    public string pythonScriptPath = "Python/classify_image.py";

    public void CaptureAndClassify(string screenshotPath)
    {
        StartCoroutine(WaitForScreenshotAndClassify(screenshotPath));
    }

    private IEnumerator WaitForScreenshotAndClassify(string path)
    {
        yield return new WaitForSeconds(1f);

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = $"\"{pythonScriptPath}\" \"{path}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                UnityEngine.Debug.Log($"Samolot: {result}");
            }
        }
    }
}
