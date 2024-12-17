using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip audioClip;
    public void PlayGame()
    {
        Debug.Log("Starting the game!");
        SceneManager.LoadSceneAsync("SampleScene");
        AudioManager.Instance.PlayBackgroundMusic(audioClip);
        Destroy(AudioManager.Instance);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game!");
        Application.Quit();
    }
}
