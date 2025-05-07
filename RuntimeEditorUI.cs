using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RuntimeEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject editorPanel;
    public TextMeshProUGUI objectNameLabel;
    public Button colorButton;
    public Button scaleUpButton;
    public Button scaleDownButton;

    [Header("Settings")]
    public bool showPanelOnStart = true;

    private SelectableObject currentObject;

    void Start()
    {
        if (editorPanel != null)
        {
            editorPanel.SetActive(showPanelOnStart);
        }

        if (colorButton != null)
            colorButton.onClick.AddListener(ChangeColor);
        else
            Debug.LogWarning("⚠️ ColorButton is not assigned!");

        if (scaleUpButton != null)
            scaleUpButton.onClick.AddListener(() => ScaleObject(1.1f));
        else
            Debug.LogWarning("⚠️ ScaleUpButton is not assigned!");

        if (scaleDownButton != null)
            scaleDownButton.onClick.AddListener(() => ScaleObject(0.9f));
        else
            Debug.LogWarning("⚠️ ScaleDownButton is not assigned!");
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("🔍 Raycast hit: " + hit.collider.name);

                SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();
                if (selectable != null)
                {
                    Debug.Log("✅ Selectable Object Found: " + selectable.objectLabel);
                    ShowEditor(selectable);
                }
                else
                {
                    Debug.LogWarning("❌ No SelectableObject found on: " + hit.collider.name);
                }
            }
        }
    }

    public void ShowEditor(SelectableObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("❌ ShowEditor called with NULL object!");
            return;
        }

        currentObject = obj;

        Debug.Log($"✅ ShowEditor: Object Found - Name: {obj.name}, Label: {obj.objectLabel}");

        if (editorPanel != null)
        {
            editorPanel.SetActive(true);
            Debug.Log("✅ Editor panel set active.");
        }
        else
        {
            Debug.LogWarning("⚠️ Editor panel reference is missing!");
        }

        if (objectNameLabel != null)
        {
            objectNameLabel.text = currentObject.objectLabel;
            Debug.Log($"✅ Label updated: {currentObject.objectLabel}");
        }
        else
        {
            Debug.LogWarning("⚠️ Object name label reference is missing!");
        }
    }

    void ChangeColor()
    {
        if (currentObject != null)
        {
            Renderer rend = currentObject.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.color = new Color(Random.value, Random.value, Random.value);
                }
                Debug.Log("🎨 Color changed for object: " + currentObject.name);
            }
            else
            {
                Debug.LogWarning("⚠️ No Renderer found on selected object.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No object selected to change color.");
        }
    }

    void ScaleObject(float scaleFactor)
    {
        if (currentObject != null)
        {
            currentObject.transform.localScale *= scaleFactor;
            Debug.Log($"📐 Object scaled by {scaleFactor}: " + currentObject.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No object selected to scale.");
        }
    }
}
