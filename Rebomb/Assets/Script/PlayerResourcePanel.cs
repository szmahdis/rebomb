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
    private Button readyButton;

    private void Awake()
    {
        // Assuming child objects have the TextMeshPro components for coins and steps
        coinText = transform.Find("CoinText").GetComponent<TextMeshProUGUI>();
        // TODO get itemGetSprite based on item, change text to image
        stepText = transform.Find("StepText").GetComponent<TextMeshProUGUI>();
        playerNameText = transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
        readyButton = transform.Find("ReadyButton").GetComponent<Button>();
        if (coinText == null || stepText == null || playerNameText == null || readyButton == null)
        {
            Debug.LogError("CoinText or StepText or PlayerNameText or ReadyButton not found in PlayerResourcePanel!");
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
        if (resourceType == "coin")
        {
            Debug.Log("new value for coin " + newValue);

            UpdateText(coinText, newValue, "Coins: ");
        }
        else if (resourceType == "step")
        {
            UpdateText(stepText, newValue, "Steps: ");
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
