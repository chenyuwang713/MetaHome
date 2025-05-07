using UnityEngine;
using TMPro;

public class FloatingInfoPanelSpawner : MonoBehaviour
{
    [Header("Floating UI Setup")]
    public GameObject floatingInfoPanelPrefab;
    public Camera mainCamera;

    /// <summary>
    /// Spawns a floating info panel above the target object and attaches it.
    /// </summary>
    public void SpawnFloatingPanel(GameObject targetObject, string description)
    {
        if (floatingInfoPanelPrefab == null || targetObject == null)
        {
            Debug.LogWarning("⚠️ Floating panel or target object not assigned.");
            return;
        }

        // Instantiate the floating panel
        GameObject panel = Instantiate(floatingInfoPanelPrefab);

        // Attach to target object
        panel.transform.SetParent(targetObject.transform, false);
        panel.transform.localPosition = new Vector3(0, 1.2f, 0); // Offset above object
        panel.transform.localRotation = Quaternion.identity;
        panel.transform.localScale = Vector3.one * 0.005f; // World scale for VR

        // Always face the camera
        if (!panel.GetComponent<Billboard>())
        {
            panel.AddComponent<Billboard>();
        }

        // Set description text
        TextMeshProUGUI descText = panel.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        if (descText != null)
        {
            descText.text = description;
        }
        else
        {
            Debug.LogWarning("⚠️ DescriptionText not found in panel.");
        }
    }
}
