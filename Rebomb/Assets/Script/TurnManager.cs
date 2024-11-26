using UnityEngine;
using System.Collections.Generic;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private static int REWIND_TURNS = 3;
    public int CurrentTurn { get; private set; }
    private Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot>();
    private bool TimeTravelTriggered { get; set; }

    private Dictionary<int, bool> playerReady = new Dictionary<int, bool>();
    List<int> PreviousSurvivalPlayers = new List<int>();

    // event
    public event System.Action<int> OnTurnChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Initialize()
    {
        CurrentTurn = 1;
        TimeTravelTriggered = false;
        playerReady.Clear();
        snapshots.Clear();

        // TODO: Initialize map and player's start positions.
        
        // Initialize player states(alive/ready), resources, positions.
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            playerReady.Add(i, false);
            Debug.Log($"Player {i + 1}, ready: {playerReady[i]}.");
            GameManager.Instance.Players[i].Alive = true;
            GameManager.Instance.Players[i].ResourceManager.OnRoundStart();
            
            // TODO: load candidate initial position from map.
            // Vector3 position = new Vector3(0, 0, 0);
            // GameManager.Instance.Players[i].SetInitialPosition(position);
        }
    }

    public void StartTurn()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            if (GameManager.Instance.Players[i].Alive == false) continue;

            // states updated
            playerReady[i] = false;

            // resource updated
            GameManager.Instance.Players[i].ResourceManager.OnTurnStart();
        }
        Debug.Log($"Turn {CurrentTurn} Started.");
    }

    public void MarkPlayerReady(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= GameManager.Instance.Players.Count)
        {
            Debug.LogError($"Invalid player {playerIndex} marked ready, ignored.");
            return;
        }
        if (playerReady[playerIndex] == true) return;

        Debug.Log($"Player {playerIndex + 1} is ready now.");
        playerReady[playerIndex] = true;
        if (CheckAllPlayersReady()) {
            EndTurn();
        }
    }

    private bool CheckAllPlayersReady()
    {
        foreach (bool isReady in playerReady.Values)
        {
            if (!isReady) return false;
            // note: for players who are not alive, they are always ready.
            // refer to StartTurn() for details.
        }
        return true;
    }

    private void EndTurn()
    {
        if (TimeTravelTriggered && CurrentTurn > 1 && REWIND_TURNS > 0) {
            TimeTravelTriggered = false;
            // Time travel here.
            // Rewind(min(REWIND_TURNS, CurrentTurn - 1));
        }

        CalculateExplosions();
        CheckRoundEnd();
        UpdateSnapshots();
        CurrentTurn++;
        OnTurnChanged?.Invoke(CurrentTurn);
        StartTurn();
    }

    private void CalculateExplosions()
    {
        // TODO: for each bomb, update state of this turn.
        // TODO: find the final explosion range.
        // TODO: for each player, check if they are in explosion range and update Alive.
        // TODO: for each player, check if they get new hourglass and update resources.
        return;
    }

    private void CheckRoundEnd()
    {
        int survivalPlayerNum = 0;
        List<int> CurrentSurvivalPlayers = new List<int>();
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            if (GameManager.Instance.Players[i].Alive)
            {
                survivalPlayerNum++;
                CurrentSurvivalPlayers.Add(i);
            }
        }

        if (survivalPlayerNum == 0)
        {
            RoundManager.Instance.EndRound(PreviousSurvivalPlayers);
        }
        else if (survivalPlayerNum == 1) {
            RoundManager.Instance.EndRound(CurrentSurvivalPlayers);
        } else
        {
            // survivalPlayerNum > 1, start next turn.
            PreviousSurvivalPlayers = CurrentSurvivalPlayers;
        }
    }

    private void UpdateSnapshots()
    {
        Snapshot snapshot = new Snapshot(CurrentTurn, PreviousSurvivalPlayers);
        snapshots.Add(CurrentTurn, snapshot);
    }
}