using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    public GameObject configPanel;
    public GameObject[] otherGameObjects;

    public void OpenConfigPanel()
    {
        Debug.Log("[Button] Open config panel.");
        configPanel.SetActive(true);
        foreach (GameObject element in otherGameObjects) {
            element.SetActive(false);
        }
    }

    public void CloseConfigPanel()
    {
        Debug.Log("[Button] Open config panel.");
        configPanel.SetActive(false);
        foreach (GameObject element in otherGameObjects) {
            element.SetActive(true);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
