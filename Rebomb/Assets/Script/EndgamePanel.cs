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
    public AudioClip audioClipLevel1;
    public AudioClip audioClipLevel2;
    private GameObject previewPanel;

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
        GameObject canvas = GameObject.Find("Canvas");
        previewPanel = canvas.transform.Find("TimeTravelPreviewPanel").gameObject;
        gameObject.SetActive(true);
        foreach (GameObject element in otherPanels) {
            element.SetActive(false);
        }
        AudioManager.Instance.PlayBackgroundMusic(endPanelClip);

    }

    private void hide_this_panel()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlayBackgroundMusic(audioClipLevel1);
    }

    public void OnNextRoundButton()
    {
        Debug.Log("Load the next round!");
        previewPanel.SetActive(true);
        RoundManager.Instance.StartRound();
        gameObject.SetActive(false);
        AudioManager.Instance.PlayBackgroundMusic(audioClipLevel2);
    }

    public void OnExitButton()
    {
        Debug.Log("Exit the game!");
        GameManager.Instance.Quit();
    }

}
