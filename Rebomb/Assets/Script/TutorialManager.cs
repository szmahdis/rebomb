using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour

{
    public GameObject[] popUps;
    public GameObject turnPanel;
    public GameObject hourglass;
    public Button okBtn;
    private int popUpIndex;
    private readonly Dictionary<KeyCode, bool> keyStates = new();

    private void Start()
    {
        okBtn.onClick.AddListener(OnOkButton);
        InitializeKeyStates();
    }

    private void InitializeKeyStates()
    {
        KeyCode[] keys =
        {
            KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
            KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,
            KeyCode.Alpha1, KeyCode.Alpha7, KeyCode.Alpha2, KeyCode.Alpha8,
            KeyCode.Alpha3, KeyCode.Alpha9, KeyCode.Alpha4, KeyCode.Alpha0,
            KeyCode.F, KeyCode.J, KeyCode.H
        };

        foreach (var key in keys)
        {
            keyStates[key] = false;
        }
    }

    private void OnOkButton()
    {
        popUpIndex++;
    }

    private void Update()
    {
        HandlePopups();
        HandleKeyPresses();
    }

    private void HandlePopups()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            popUps[i].SetActive(i == popUpIndex && turnPanel != null && !turnPanel.activeSelf);
        }
    }

    private void HandleKeyPresses()
    {
        foreach (var key in keyStates.Keys.ToList())
        {
            if (Input.GetKeyDown(key))
            {
                keyStates[key] = true;
            }
        }

        switch (popUpIndex)
        {
            case 1: // Movement Popup
                if (CheckKeys(KeyCode.LeftArrow, KeyCode.RightArrow) && CheckKeys(KeyCode.UpArrow, KeyCode.DownArrow) &&
                    CheckKeys(KeyCode.A, KeyCode.D) && CheckKeys(KeyCode.W, KeyCode.S))
                {
                    NextPopup("Good job! Player learned movement keys.");
                }
                break;
            case 2: // Active Bomb Popup
                if (CheckKeys(KeyCode.Alpha1, KeyCode.Alpha7)) NextPopup();
                break;
            case 3: // Press Ready Popup
                if (CheckKeys(KeyCode.F, KeyCode.J)) NextPopup();
                break;
            case 4:
                StartCoroutine(WaitAndShowNextPopup(5));
                break;
            case 5: // Passive Bomb Popup
                if (CheckKeys(KeyCode.Alpha2, KeyCode.Alpha8)) NextPopup();
                break;
            case 6: // Chain Bomb Popup
                if (CheckKeys(KeyCode.Alpha3, KeyCode.Alpha9)) NextPopup();
                break;
            case 7: // Safe Bomb Popup
                if (CheckKeys(KeyCode.Alpha4, KeyCode.Alpha0)) NextPopup();
                break;
            case 8: // Destroy Wall Popup
                if (!hourglass.activeSelf) NextPopup();
                break;
            case 9: // Use Hourglass Popup
                StartCoroutine(WaitAndShowNextPopup(10));
                break;
            case 10: // Help Panel Popup
                if (keyStates[KeyCode.H]) DisableAllPopups();
                break;
        }
    }

    private bool CheckKeys(params KeyCode[] keys)
    {
        return keys.All(key => keyStates[key]);
    }

    private void NextPopup(string message = "")
    {
        if (!string.IsNullOrEmpty(message)) Debug.Log(message);
        popUpIndex++;
    }

    private IEnumerator WaitAndShowNextPopup(int nextPopUp)
    {
        yield return new WaitForSeconds(7f);
        NextPopup();
        popUpIndex = nextPopUp;
    }

    public void DisableAllPopups()
    {
        foreach (var popup in popUps)
        {
            popup.SetActive(false);
        }
    }
}