using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class TutorialManager : MonoBehaviour

{
    public static TutorialManager Instance { get; private set; }
    public GameObject[] popUps;
    public GameObject[] xboxPopUps;
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

    public void OnHideCurrentGuidance(InputAction.CallbackContext context){
        if (context.performed)
        {
            OnOkButton();
        }
    }

    private void OnOkButton()
    {
        popUpIndex++;
    }

    private void Update()
    {
        HandlePopups();
        // HandleKeyPresses();
    }

    private void HandlePopups()
    {
        if (GameManager.Instance.xBoxUI == true)
        {
            for (int i = 0; i < xboxPopUps.Length; i++)
            {
                xboxPopUps[i].SetActive(i == popUpIndex && turnPanel != null && !turnPanel.activeSelf);
            }
        }
        else
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                popUps[i].SetActive(i == popUpIndex && turnPanel != null && !turnPanel.activeSelf);
            }
        }
    }

    // Track completion status for both players across phases
    private Dictionary<int, bool[]> playerCompletion = new Dictionary<int, bool[]>();
    private int currentPhase = -1;

    public void HandleKeyPresses(string action, int playerIndex)
    {
        // Initialize phase tracking when entering new phase
        if (currentPhase != popUpIndex)
        {
            currentPhase = popUpIndex;
            playerCompletion[popUpIndex] = new bool[2] { false, false };
        }

        // Validate player index range
        if (playerIndex < 0 || playerIndex > 1) return;

        switch (popUpIndex)
        {
            case 1: // Movement tutorial phase
                if (action == "Move")
                {
                    MarkPlayerComplete(playerIndex);
                    if (AllPlayersComplete())
                    {
                        NextPopup("Good job! Both players learned movement keys.");
                    }
                }
                break;
            
            case 2: // Active Bomb tutorial
                if (action == "ActiveBomb") 
                {
                    MarkPlayerComplete(playerIndex);
                    if (AllPlayersComplete()) NextPopup();
                }
                break;
            
            case 3: // Ready confirmation
                if (action == "Ready")
                {
                    MarkPlayerComplete(playerIndex);
                    if (AllPlayersComplete()) NextPopup();
                }
                break;
            
            case 5: // Passive Bomb
                if (action == "PassiveBomb")
                {
                    MarkPlayerComplete(playerIndex);
                    if (AllPlayersComplete()) NextPopup();
                }
                break;
            
            case 6: // Chain Bomb
                if (action == "ChainBomb")
                {
                    MarkPlayerComplete(playerIndex);
                    if (AllPlayersComplete()) NextPopup();
                }
                break;
            
            case 7: // Safe Bomb
                if (action == "SafeBomb")
                {
                    MarkPlayerComplete(playerIndex);
                    if (AllPlayersComplete()) NextPopup();
                }
                break;
            
            // Single-player phases (no coordination needed)
            case 4:  // Timed delay
            case 8:  // Environmental interaction
            case 9:  // Timed progression
            case 10: // Help system
                HandleSinglePlayerPhase(action);

                break;
        }
    }

    /// <summary>
    /// Marks a player as having completed the current phase requirement
    /// </summary>
    /// <param name="playerIndex">0 for Player 1, 1 for Player 2</param>
    private void MarkPlayerComplete(int playerIndex)
    {
        if (playerCompletion.ContainsKey(popUpIndex))
        {
            playerCompletion[popUpIndex][playerIndex] = true;
        }
    }

    /// <summary>
    /// Checks if both players have completed the current phase requirement
    /// </summary>
    private bool AllPlayersComplete()
    {
        return playerCompletion.TryGetValue(popUpIndex, out var completion) 
            && completion[0] 
            && completion[1];
    }

    /// <summary>
    /// Handles phases that don't require dual-player coordination
    /// </summary>
    private void HandleSinglePlayerPhase(string action)
    {
        switch (popUpIndex)
        {
            case 4: // Automatic progression after delay
                StartCoroutine(WaitAndShowNextPopup(5));
                break;
            case 8: // Environmental condition check
                if (hourglass != null && !hourglass.activeSelf) NextPopup();
                break;
            case 9: // Timed progression
                StartCoroutine(WaitAndShowNextPopup(10));
                break;
            case 10: // Help system toggle
                if (action == "Help") DisableAllPopups();
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
        yield return new WaitForSeconds(2f);
        NextPopup();
        popUpIndex = nextPopUp;
    }

    public void DisableAllPopups()
    {
        NextPopup();
        if (GameManager.Instance.xBoxUI == true)
        {
            foreach (var popup in xboxPopUps)
            {
                popup.SetActive(false);
            }
        }
        else
        {
            foreach (var popup in popUps)
            {
                popup.SetActive(false);
            }
        }
    }
}