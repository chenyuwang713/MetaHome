using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.IO;

public class Llama : MonoBehaviour
{
    [Header("LLaMA RAG Server URL")]
    public string scriptUrl = "http://10.96.50.180:5001/query";

    [Header("UI Prefabs")]
    public GameObject floatingInfoPanelPrefab;
    public GameObject actionButtonPrefab;

    [Header("Track Last Object")]
    public string lastBaseAsset = "";
    public GameObject lastSpawnedObject = null;

    [System.Serializable]
    public class LlamaReply
    {
        public string response;
    }

    public async void RequestRAGResponse(string userInput, string optionalContext = "")
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogWarning("⚠️ User input is empty.");
            return;
        }

        string context = string.IsNullOrEmpty(optionalContext)
            ? Application.productName + " Smart Home Asset"
            : optionalContext;

        var payload = new Dictionary<string, string>
        {
            { "query", userInput },
            { "context", context }
        };

        string jsonBody = JsonConvert.SerializeObject(payload);
        Debug.Log("📤 Sending RAG query: " + jsonBody);

        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(scriptUrl, content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                LlamaReply reply = JsonConvert.DeserializeObject<LlamaReply>(result);

                Debug.Log("🧠 RAG Response: " + reply.response);

                if (lastSpawnedObject != null && floatingInfoPanelPrefab != null)
                {
                    SpawnFloatingPanel(lastSpawnedObject, reply.response);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("❌ RAG HTTP Request Failed: " + e.Message);
            }
        }
    }

    public void StylizeLastSpawned(string stylePrompt)
    {
        if (string.IsNullOrEmpty(lastBaseAsset) || lastSpawnedObject == null)
        {
            Debug.LogWarning("⚠️ No object to stylize.");
            return;
        }

        string fullPrompt = $"Stylize the model '{lastBaseAsset}' as: {stylePrompt}";
        RequestRAGResponse(fullPrompt);
    }

    public void RegisterSpawnedObject(GameObject obj, string assetName)
    {
        lastBaseAsset = assetName;
        lastSpawnedObject = obj;
    }

    public void RequestLLaMAAction(GameObject selectedObject, string extraContext = "")
    {
        if (selectedObject == null) return;

        Renderer rend = selectedObject.GetComponentInChildren<Renderer>();
        string color = rend ? ColorUtility.ToHtmlStringRGB(rend.material.color) : "unknown";
        Vector3 scale = selectedObject.transform.localScale;
        string name = selectedObject.name;

        string prompt =
            $"A 3D object was just placed in the scene.\n" +
            $"- Name: {name}\n" +
            $"- Color: #{color}\n" +
            $"- Size: {scale}\n" +
            extraContext +
            "\nSuggest 3 interactive actions relevant for this object.";

        RequestRAGResponse(prompt);
    }

    private void SpawnFloatingPanel(GameObject targetObject, string description)
    {
        Transform existing = targetObject.transform.Find("FloatingInfoPanel(Clone)");
        if (existing != null) Destroy(existing.gameObject);

        GameObject panel = Instantiate(floatingInfoPanelPrefab);
        panel.name = "FloatingInfoPanel(Clone)";
        panel.transform.SetParent(targetObject.transform, false);
        panel.transform.localPosition = new Vector3(0, 1.2f, 0);
        panel.transform.localRotation = Quaternion.identity;
        panel.transform.localScale = Vector3.zero;

        if (!panel.GetComponent<Billboard>())
            panel.AddComponent<Billboard>();

        TextMeshProUGUI descText = panel.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        if (descText != null)
        {
            descText.text = description.Length > 300 ? description.Substring(0, 300) + "..." : description;
        }
        else
        {
            Debug.LogWarning("⚠️ DescriptionText not found in panel prefab!");
        }

        StartCoroutine(AnimatePanelSpawn(panel));
        Destroy(panel, 10f);
    }

    private IEnumerator AnimatePanelSpawn(GameObject panel)
    {
        Vector3 targetScale = Vector3.one * 0.005f;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            panel.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        panel.transform.localScale = targetScale;
    }

    private IEnumerator FadeOutAndDestroy(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            float fadeDuration = 1f;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }
        }

        Destroy(panel);
    }
}
