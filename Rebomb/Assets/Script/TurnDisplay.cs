using TMPro;
using UnityEngine;

public class TurnDisplay : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    private void OnEnable()
    {
        // Subscribe to the TurnManager's turn changed event
        Debug.Log("Subscribing to TurnManager's OnTurnChanged event [0].");
        if (TurnManager.Instance != null)
        {
            Debug.Log("Subscribing to TurnManager's OnTurnChanged event [1].");
            TurnManager.Instance.OnTurnChanged += UpdateTurnText;
        }
    }

    private void OnDisable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged -= UpdateTurnText;
        }
    }

    private void UpdateTurnText(int TurnNumber)
    {
        Debug.Log($"TurnDisplay received event: Turn is now {TurnNumber}");
        turnText.text = $"Turn {TurnNumber}";
    }
}
