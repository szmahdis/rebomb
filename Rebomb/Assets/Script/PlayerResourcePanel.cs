using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class PlayerResourcePanel : MonoBehaviour
{
    public ResourceManager ResourceManager;

    public int playerIndex;
    private TextMeshProUGUI playerNameText;
    private TextMeshProUGUI coinText;
    private TextMeshProUGUI stepText;
    private TextMeshProUGUI hourglassText;
    private Button hourglassButton;
    private Button readyButton;

    private void Awake()
    {
        // Assuming child objects have the TextMeshPro components for coins and steps
        coinText = transform.Find("CoinText").GetComponent<TextMeshProUGUI>();
        stepText = transform.Find("StepText").GetComponent<TextMeshProUGUI>();
        playerNameText = transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
        readyButton = transform.Find("ReadyButton").GetComponent<Button>();
        hourglassButton = transform.Find("HourglassButton").GetComponent<Button>();
        if (coinText == null || stepText == null || playerNameText == null || readyButton == null || hourglassButton == null)
        {
            Debug.LogError("CoinText or StepText or PlayerNameText or ReadyButton or HourglassText not found in PlayerResourcePanel!");
        }
    }

    private void Start()
    {
        string playerName = "";
        if (playerIndex == 0) playerName = "Gold";
        else if (playerIndex == 1) playerName = "Blue";
        UpdateText(playerNameText, playerName, "Player ");
        readyButton.GetComponent<PlayerReadyButton>().playerIndex = playerIndex;

        // Ensure hourglass button is disabled initially
        if (hourglassButton != null)
        {
            hourglassButton.interactable = false;
            hourglassButton.onClick.RemoveAllListeners();
            hourglassButton.onClick.AddListener(OnHourglassButtonClicked);
        }
    }

    // Function called when hourglassButton is clicked
    public void OnHourglassButtonClicked()
    {
        if (TurnManager.Instance != null)
        {
            GameManager.Instance.Players[playerIndex].ResourceManager.consumeHourglass();
        }
        else
        {
            Debug.LogWarning("TurnManager reference is not assigned!");
        }
    }

    public void OnHourglassPreviewButtonClicked()
    {
        if (hourglassButton != null)
        {
            HoverHandler hoverHandler = hourglassButton.GetComponent<HoverHandler>();
            if (hoverHandler != null)
            {
                if (hoverHandler.IsTimeTravelPreviewVisible())
                    hoverHandler.HideTimeTravelPreview();
                else
                    hoverHandler.ShowTimeTravelPreview();
            }
        }
    }

    private void OnEnable()
    {
        if (ResourceManager != null)
        {
            ResourceManager.OnResourceUpdated += HandleResourceUpdated;
        }
    }

    private void OnDisable()
    {
        if (ResourceManager != null)
        {
            ResourceManager.OnResourceUpdated -= HandleResourceUpdated;
        }
    }

    private void HandleResourceUpdated(string resourceType, int newValue)
    {
        var resourceMappings = new Dictionary<string, (TextMeshProUGUI uiText, string prefix)>{
            { "coin", (coinText, "     x ") },
            { "step", (stepText, "     x ") },
        };

        if (resourceType == "hourglass")
        {
            if (newValue == 0)
            {
                DeactivateHourglassButton();
            }
            else
            {
                ActivateHourglassButton();
            }
        }
        else if (resourceMappings.TryGetValue(resourceType, out var mapping))
        {
            UpdateText(mapping.uiText, newValue.ToString(), mapping.prefix);
        }
        else
        {
            Debug.LogWarning($"Unhandled resource type: {resourceType}");
        }
    }

    private void UpdateText(TextMeshProUGUI textElement, string newValue, string prefix)
    {
        if (textElement != null)
        {
            textElement.text = $"{prefix}{newValue}";
        }
    }

    // Function to enable the hourglassButton
    private void ActivateHourglassButton()
    {
        if (hourglassButton != null)
        {
            hourglassButton.interactable = true;
            // Enable child objects
            foreach (Transform child in hourglassButton.transform)
            {
                child.gameObject.SetActive(true);
            }
            // Check if HoverHandler is already added to avoid duplicates
            if (hourglassButton.GetComponent<HoverHandler>() == null)
            {
                hourglassButton.gameObject.AddComponent<HoverHandler>();
            }
        }
        else
        {
            Debug.LogWarning("Hourglass button is not assigned!");
        }
    }

    private void DeactivateHourglassButton()
    {
        if (hourglassButton != null)
        {
            hourglassButton.interactable = false;
            // Disable child objects
            foreach (Transform child in hourglassButton.transform)
            {
                child.gameObject.SetActive(false);
            }
            // Optionally, remove the HoverHandler when deactivating
            var hoverHandler = hourglassButton.GetComponent<HoverHandler>();
            if (hoverHandler != null)
            {
                hoverHandler.OnPointerExit(null);
                Destroy(hoverHandler);
            }
        }
        else
        {
            Debug.LogWarning("Hourglass button is not assigned!");
        }
    }
}

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RenderTexture previewTexture;       // RenderTexture for the small window
    public GameObject previewPanel;            // UI Panel that contains the RawImage
    private RawImage previewImage;              // RawImage component to display RenderTexture
    private void Awake()
    {
        // Find the preview panel and its RawImage
        GameObject canvas = GameObject.Find("Canvas");
        previewPanel = canvas.transform.Find("TimeTravelPreviewPanel").gameObject;
        if (previewPanel == null)
        {
            Debug.LogError("TimeTravelPreviewPanel not found in the scene!");
            return;
        }
        previewImage = previewPanel.GetComponentInChildren<RawImage>();
        if (previewImage == null)
        {
            Debug.LogError("RawImage for Time Travel Preview not found!");
            return;
        }

        previewPanel.SetActive(false); // Hide initially
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("Hovered!");
        // Show a small window of preview of time travel using snapshots
        ShowTimeTravelPreview();

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("Exited!");
        // Handle exit behavior here
        HideTimeTravelPreview();
    }
    public void ShowTimeTravelPreview()
    {
        previewPanel.SetActive(true);

        Texture2D Previewscreenshot = TurnManager.Instance.GetSnapshotImage();

        // make image rgba to be 255,255,255,255
        RenderTexture LastBombTexture = new RenderTexture(1920, 1080, 32);
        Camera LastBombCamera = GameObject.Find("LastBombCamera").GetComponent<Camera>();
        Texture2D LastBombScreenshot = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
        LastBombCamera.targetTexture = LastBombTexture;
        LastBombCamera.Render();
        RenderTexture.active = LastBombTexture;
        LastBombScreenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        LastBombScreenshot.Apply();
        LastBombCamera.targetTexture = null;
        RenderTexture.active = null;
        Texture2D CombinedScreenshot = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
        // combine two textures
        for (int i = 0; i < 1920; i++)
        {
            for (int j = 0; j < 1080; j++)
            {
                if (LastBombScreenshot.GetPixel(i, j) != Color.clear)
                {
                    CombinedScreenshot.SetPixel(i, j, LastBombScreenshot.GetPixel(i, j));
                }
                else
                {
                    CombinedScreenshot.SetPixel(i, j, Previewscreenshot.GetPixel(i, j));
                }
            }
        }
        CombinedScreenshot.Apply();
        // previewImage.texture = TurnManager.Instance.GetSnapshotImage();
        previewImage.texture = CombinedScreenshot;
        previewImage.color = new Color(1, 1, 1, 0.8f);
    }

    public void HideTimeTravelPreview()
    {
        previewPanel.SetActive(false);
        previewImage.texture = null;
        previewImage.color = new Color(1, 1, 1, 0);
    }

    public bool IsTimeTravelPreviewVisible()
    {
        return previewPanel.activeSelf;
    }
}