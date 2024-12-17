using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgamePanel : MonoBehaviour
{
    public GameObject endgamePanel;
    private TextMeshProUGUI winnerText;
    public AudioClip audioClip;

    public void ShowEndGameResult(string winner)
    {
        endgamePanel.SetActive(true);
        winnerText = endgamePanel.transform.Find("WinnerText").GetComponent<TextMeshProUGUI>();
        winnerText.text = $"Winner: {winner}";
        AudioManager.Instance.PlayBackgroundMusic(audioClip);

    }

    public void LoadMainMenu()
    {
        Debug.Log("Going back to Main Menu!");
        SceneManager.LoadSceneAsync("MainMenu");
    }


}
