using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using System.Collections;

public class GLBLoader : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform parentObject;
    public Llama llamaAI;

    public void Download(string assetNameWithHash)
    {
        string glbUrl = $"https://raw.githubusercontent.com/raghava7261/3D-Models/main/{assetNameWithHash}.glb";
        Debug.Log("📦 Downloading from: " + glbUrl);
        StartCoroutine(DownloadGLB(glbUrl, assetNameWithHash));
    }

    public IEnumerator DownloadGLB(string url, string assetName)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Failed to download GLB: " + www.error + " | URL: " + url);
            yield break;
        }

        byte[] data = www.downloadHandler.data;
        string path = $"{Application.persistentDataPath}/{assetName}.glb";
        System.IO.File.WriteAllBytes(path, data);

        Importer.ImportGLBAsync(path, new ImportSettings(), (GameObject rootObj, AnimationClip[] clips) =>
        {
            if (rootObj == null)
            {
                Debug.LogError("❌ Failed to import GLB");
                return;
            }

            // Set position and parent
            rootObj.transform.position = spawnPoint.position;
            rootObj.transform.SetParent(parentObject, true);

            // Rename the object to appear clearly in the hierarchy
            rootObj.name = assetName;
            rootObj.tag = "ImportedAsset";

            // Optional: Add mesh or material manually if needed
            if (!rootObj.GetComponent<MeshRenderer>() && rootObj.transform.childCount > 0)
            {
                foreach (Transform child in rootObj.transform)
                {
                    if (!child.GetComponent<MeshRenderer>())
                        child.gameObject.AddComponent<MeshRenderer>(); // TEMP fallback
                }
            }

            // Add interaction components
            if (!rootObj.GetComponent<Rigidbody>()) rootObj.AddComponent<Rigidbody>();
            if (!rootObj.GetComponent<BoxCollider>()) rootObj.AddComponent<BoxCollider>();
            if (!rootObj.GetComponent<ObjectMover>()) rootObj.AddComponent<ObjectMover>();
            if (!rootObj.GetComponent<SelectableObject>())
            {
                var selectable = rootObj.AddComponent<SelectableObject>();
                selectable.objectLabel = assetName;
            }

            Debug.Log("✅ GLB loaded & attached: " + assetName);

            // Register with LLaMA
            if (llamaAI != null && rootObj != null)
            {
                llamaAI.RegisterSpawnedObject(rootObj, assetName);
                llamaAI.RequestLLaMAAction(rootObj);
            }
        });
    }
}
