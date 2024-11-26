using TMPro;
using UnityEngine;

public class TurnDisplay : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    private void OnEnable()
    {
        if (TurnManager.Instance != null)
        {
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
        turnText.text = $"Turn {TurnNumber}";
    }
}
