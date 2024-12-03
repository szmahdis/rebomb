using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourcePanel : MonoBehaviour
{
    public ResourceManager ResourceManager;

    public int playerIndex;
    private TextMeshProUGUI playerNameText;
    private TextMeshProUGUI coinText;
    private TextMeshProUGUI stepText;
    private TextMeshProUGUI hourglassText;
    private Button readyButton;

    private void Awake()
    {
        // Assuming child objects have the TextMeshPro components for coins and steps
        coinText = transform.Find("CoinText").GetComponent<TextMeshProUGUI>();
        stepText = transform.Find("StepText").GetComponent<TextMeshProUGUI>();
        hourglassText = transform.Find("HourglassText").GetComponent<TextMeshProUGUI>();
        playerNameText = transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
        readyButton = transform.Find("ReadyButton").GetComponent<Button>();
        if (coinText == null || stepText == null || playerNameText == null || readyButton == null || hourglassText == null)
        {
            Debug.LogError("CoinText or StepText or PlayerNameText or ReadyButton or HourglassText not found in PlayerResourcePanel!");
        }
    }

    private void Start()
    {
        UpdateText(playerNameText, playerIndex+1, "Player ");
        readyButton.GetComponent<PlayerReadyButton>().playerIndex = playerIndex;
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
        var resourceMappings = new Dictionary<string, (TextMeshProUGUI uiText, string prefix)>
    {
        { "coin", (coinText, "Coins: ") },
        { "step", (stepText, "Steps: ") },
        { "hourglass", (hourglassText, "Hourglass: ") }
    };

        if (resourceMappings.TryGetValue(resourceType, out var mapping))
        {
            UpdateText(mapping.uiText, newValue, mapping.prefix);
        }
        else
        {
            Debug.LogWarning($"Unhandled resource type: {resourceType}");
        }
    }

    private void UpdateText(TextMeshProUGUI textElement, int newValue, string prefix)
    {
        if (textElement != null)
        {
            textElement.text = $"{prefix}{newValue}";
        }
    }
}
