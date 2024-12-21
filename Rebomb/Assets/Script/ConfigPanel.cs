using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    public GameObject[] otherGameObjects;

    public void OpenConfigPanel()
    {
        gameObject.SetActive(true);
        foreach (GameObject element in otherGameObjects) {
            element.SetActive(false);
        }
    }

    public void CloseConfigPanel()
    {
        gameObject.SetActive(false);
        foreach (GameObject element in otherGameObjects) {
            element.SetActive(true);
        }
    }
}
