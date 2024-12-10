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
    private Button hourglassButton;
    private Button readyButton;
    private Button safeBombButton;

    private void Awake()
    {
        // Assuming child objects have the TextMeshPro components for coins and steps
        coinText = transform.Find("CoinText").GetComponent<TextMeshProUGUI>();
        stepText = transform.Find("StepText").GetComponent<TextMeshProUGUI>();
        hourglassText = transform.Find("HourglassText").GetComponent<TextMeshProUGUI>();
        playerNameText = transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
        readyButton = transform.Find("ReadyButton").GetComponent<Button>();
        hourglassButton = transform.Find("HourglassButton").GetComponent<Button>();
        safeBombButton = transform.Find("SafeBombButton").GetComponent<Button>();
        if (coinText == null || stepText == null || playerNameText == null || readyButton == null || hourglassText == null || hourglassButton == null)
        {
            Debug.LogError("CoinText or StepText or PlayerNameText or ReadyButton or HourglassText not found in PlayerResourcePanel!");
        }
    }

    private void Start()
    {
        UpdateText(playerNameText, playerIndex+1, "Player ");
        readyButton.GetComponent<PlayerReadyButton>().playerIndex = playerIndex;

        //Ensure hourglass button is disabled initially
        if(hourglassButton != null)
        {
            hourglassButton.interactable = false;

            // Assign the OnHourglassButtonClicked method to the onClick event
            hourglassButton.onClick.RemoveAllListeners();
            hourglassButton.onClick.AddListener(OnHourglassButtonClicked);
        }

        safeBombButton.onClick.AddListener(OnSafeBombButtonClicked);

    }

    // Function called when hourglassButton is clicked
    public void OnHourglassButtonClicked()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.TimeTravelTriggered = true;
            Debug.Log("Time travel triggered through TurnManager!");
            HandleResourceUpdated("hourglass", 0);
            ResourceManager.GetInventoryItemList();

        }
        else
        {
            Debug.LogWarning("TurnManager reference is not assigned!");
        }
    }

    public void OnSafeBombButtonClicked()
    {
        // find player object using index
        GameObject playerObject = GameObject.Find("Players").transform.Find(playerNameText.text).gameObject;
        playerObject.GetComponent<Player>().PlaceBomb(BombType.SafeBomb);
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

            // Check for hourglass and activate the button if condition is met
            if (resourceType == "hourglass" && newValue > 0)
            {
                ActivateHourglassButton();
                Debug.Log("Hourglass button activated.");
            }
            if (resourceType == "hourglass" && newValue == 0)
            {
                DeactivateHourglassButton();
                Debug.Log("Hourglass button deactivated.");
            }

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

    // Function to enable the hourglassButton
    private void ActivateHourglassButton()
    {
        if (hourglassButton != null) // Ensure the button is assigned
        {
            hourglassButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("Hourglass button is not assigned!");
        }
    }

    private void DeactivateHourglassButton()
    {
        if (hourglassButton != null) // Ensure the button is assigned
        {
            hourglassButton.interactable = false;
        }
        else
        {
            Debug.LogWarning("Hourglass button is not assigned!");
        }
    }
}
