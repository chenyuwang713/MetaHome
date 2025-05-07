using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InputUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputField;
    public Button submitButton;

    [Header("References")]
    public Llama llamaAI;
    public PromptToGLB promptToGLB;  // Ensure this is correctly assigned in the Unity Inspector
    public Transform spawnPoint;

    [Header("Debugging")]
    public bool enableDebugKeys = false;

    void Start()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
        else
            Debug.LogWarning("⚠️ Submit Button is not assigned!");
    }

    void OnSubmit()
    {
        if (inputField == null)
        {
            Debug.LogError("❌ Input field is missing.");
            return;
        }

        string userInput = inputField.text.Trim();

        // 🧹 Sanitize the input: skip default or empty prompts
        if (string.IsNullOrEmpty(userInput) ||
            userInput.ToLower().Contains("type an object") ||
            userInput.ToLower().Contains("enter prompt") ||
            userInput.Length < 3)
        {
            Debug.LogWarning("⚠️ Invalid or placeholder input. Ignored.");
            return;
        }

        Debug.Log("📨 User Input: " + userInput);

        if (promptToGLB != null)
        {
            promptToGLB.GenerateFromPrompt(userInput);
        }
        else
        {
            Debug.LogError("❌ PromptToGLB not assigned. Cannot generate model.");
        }

        if (llamaAI != null)
            llamaAI.RequestRAGResponse(userInput);

        inputField.text = "";
    }

    void Update()
    {
#if UNITY_EDITOR
        if (enableDebugKeys && Keyboard.current != null)
        {
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                OnSubmit();
            }
        }
#endif
    }
}
