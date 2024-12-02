using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerReadyButton : MonoBehaviour
{
    private Button readyButton;
    private TextMeshProUGUI buttonText;

    public int playerIndex;
    private bool isReady = false;

    private void Awake()
    {
        readyButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (readyButton != null)
        {
            readyButton.onClick.AddListener(OnReadyButtonClicked);
        }
    }

    private void Start()
    {
        SetButtonState(false);
    }

    private void OnDestroy()
    {
        if (readyButton != null)
        {
            readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        }
    }


    private void OnEnable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged += ResetButton;
        }
    }

    private void OnDisable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged -= ResetButton;
        }
    }

    public void OnReadyButtonClicked()
    {
        if (isReady) return;

        isReady = true;
        SetButtonState(true);
        GameManager.Instance.Players[playerIndex].OnPlayerReady();
    }

    private void SetButtonState(bool ready)
    {
        if (buttonText != null)
        {
            buttonText.text = ready ? "Wait" : "Ready";
        }

        if (readyButton != null)
        {
            readyButton.interactable = !ready;
        }
    }

    public void ResetButton(int CurrentTurn)
    {
        isReady = false;
        SetButtonState(false);
    }
}