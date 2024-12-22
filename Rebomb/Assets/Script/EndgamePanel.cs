using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndgamePanel : MonoBehaviour
{
    public GameObject[] otherPanels;
    public GameObject nextRoundBtn;
    public TextMeshProUGUI winnerText;
    public AudioClip endPanelClip;
    public AudioClip audioClip;

    public void ShowResult(List<int> winners)
    {
        string winner_text = "Winner:";
        foreach (int i in winners)
        {
            winner_text += " " + GameManager.Instance.Players[i].Name;
        }
        winnerText.text = winner_text;
        nextRoundBtn.SetActive(
            RoundManager.Instance.CurrentRound < RoundManager.MAX_ROUNDS
        );
        show_this_panel();
    }

    private void show_this_panel()
    {
        gameObject.SetActive(true);
        foreach (GameObject element in otherPanels) {
            element.SetActive(false);
        }
        AudioManager.Instance.PlayBackgroundMusic(endPanelClip);

    }

    private void hide_this_panel()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlayBackgroundMusic(audioClip);
    }

    public void OnNextRoundButton()
    {
        Debug.Log("Load the next round!");
        RoundManager.Instance.StartRound();
        gameObject.SetActive(false);
    }

    public void OnExitButton()
    {
        Debug.Log("Exit the game!");
        GameManager.Instance.Quit();
    }

}
