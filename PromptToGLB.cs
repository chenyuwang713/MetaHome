using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PromptToGLB : MonoBehaviour
{
    public string flaskURL = "http://10.96.50.180:5000/generate_glb";  // Replace with your Flask IP
    public GLBLoader glbLoader;  // 🔁 Use the working GLB loader you had before

    public void GenerateFromPrompt(string prompt)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            StartCoroutine(SendPrompt(prompt));
        }
        else
        {
            Debug.LogWarning("⚠️ Prompt is empty.");
        }
    }

    IEnumerator SendPrompt(string prompt)
    {
        string json = "{\"prompt\":\"" + prompt + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = new UnityWebRequest(flaskURL, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            string glbUrl = ExtractGLBURL(jsonResponse);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(glbUrl);

            Debug.Log("✅ GLB URL received: " + glbUrl);
            StartCoroutine(glbLoader.DownloadGLB(glbUrl, assetName));  // ✅ Use GLB loader
        }
        else
        {
            Debug.LogError("❌ Flask error: " + www.error);
        }
    }

    string ExtractGLBURL(string json)
    {
        int start = json.IndexOf("http");
        int end = json.IndexOf(".glb") + 4;
        if (start >= 0 && end > start)
        {
            return json.Substring(start, end - start);
        }

        Debug.LogError("❌ Failed to extract .glb URL.");
        return "";
    }
}
