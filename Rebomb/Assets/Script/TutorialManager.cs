using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
            KeyCode.F, KeyCode.J, KeyCode.H, 
            KeyCode.Joystick1Button0, KeyCode.Joystick2Button0, // A button on Xbox controller
            KeyCode.Joystick1Button1, KeyCode.Joystick2Button1, // B button on Xbox controller
            KeyCode.Joystick1Button2, KeyCode.Joystick2Button2,// X button on Xbox controller
            KeyCode.Joystick1Button3, KeyCode.Joystick2Button3,// Y button on Xbox controller
            KeyCode.Joystick1Button4, KeyCode.Joystick2Button4,// LB button on Xbox controller
            KeyCode.Joystick1Button5, KeyCode.Joystick2Button5,// RB button on Xbox controller
            KeyCode.Joystick1Button6, KeyCode.Joystick2Button6, // View button on Xbox controller
            KeyCode.Joystick1Button7, KeyCode.Joystick2Button7,// Menu button on Xbox controller
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
        // Debug.Log(Input.GetAxisRaw("XboxDPadHorizontal"));
        bool Moved = false;
        if (Gamepad.current.dpad.ReadValue() != Vector2.zero)
        {
            Moved = true;
        }
        foreach (var key in keyStates.Keys.ToList())
        {
            if (Input.GetKeyDown(key))
            {
                keyStates[key] = true;
            }
        }
        // Xbox controller
        if (GameManager.Instance.xBoxUI)
        {
            switch (popUpIndex)
            {
                case 1: // Movement Popup
                    if (Moved)
                    {
                        NextPopup("Good job! Player learned movement keys.");
                    }
                    break;
                case 2: // Active Bomb Popup
                    if (CheckKeys(KeyCode.Joystick1Button0, KeyCode.Joystick2Button0)) NextPopup();
                    // if (CheckKeys(KeyCode.Joystick1Button0)) NextPopup();
                    break;
                case 3: // Press Ready Popup
                    if (CheckKeys(KeyCode.Joystick1Button7, KeyCode.Joystick2Button7)) NextPopup();
                    // if (CheckKeys(KeyCode.Joystick1Button7)) NextPopup();
                    break;
                case 4:
                    StartCoroutine(WaitAndShowNextPopup(5));
                    break;
                case 5: // Passive Bomb Popup
                    if (CheckKeys(KeyCode.Joystick1Button1, KeyCode.Joystick2Button1)) NextPopup();
                    // if (CheckKeys(KeyCode.Joystick1Button1)) NextPopup();
                    break;
                case 6: // Chain Bomb Popup
                    if (CheckKeys(KeyCode.Joystick1Button2, KeyCode.Joystick2Button2)) NextPopup();
                    break;
                case 7: // Safe Bomb Popup
                    if (CheckKeys(KeyCode.Joystick1Button3, KeyCode.Joystick2Button3)) NextPopup();
                    break;
                case 8: // Destroy Wall Popup
                    if (hourglass != null && hourglass.activeSelf == false) NextPopup();
                    break;
                case 9: // Use Hourglass Popup
                    StartCoroutine(WaitAndShowNextPopup(10));
                    break;
                case 10: // Help Panel Popup
                    if (keyStates[KeyCode.Joystick1Button4] || keyStates[KeyCode.Joystick2Button4]) DisableAllPopups();
                    break;
            }
        }
        // Keyboard
        else 
        {
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
                    if (hourglass != null && hourglass.activeSelf == false) NextPopup();
                    break;
                case 9: // Use Hourglass Popup
                    StartCoroutine(WaitAndShowNextPopup(10));
                    break;
                case 10: // Help Panel Popup
                    if (keyStates[KeyCode.H]) DisableAllPopups();
                    break;
            }
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