using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    public GameObject[] otherPanels;

    public void OpenConfigPanel()
    {
        gameObject.SetActive(true);
        foreach (GameObject element in otherPanels) {
            element.SetActive(false);
        }
    }

    public void CloseConfigPanel()
    {
        gameObject.SetActive(false);
        foreach (GameObject element in otherPanels) {
            element.SetActive(true);
        }
    }
}
