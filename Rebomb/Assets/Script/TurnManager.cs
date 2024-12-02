using UnityEngine;
using System.Collections.Generic;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private static int REWIND_TURNS = 3;
    public int CurrentTurn { get; private set; }
    private Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot>();
    private bool TimeTravelTriggered { get; set; }

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
        snapshots.Clear();

        // TODO: Initialize map and player's start positions.

        // Initialize player states(alive/ready), resources, positions.
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            GameManager.Instance.Players[i].Alive = true;
            GameManager.Instance.Players[i].Ready = false;
            GameManager.Instance.Players[i].ResourceManager.OnRoundStart();

            // TODO: load candidate initial position from map.
            // Vector3 position = new Vector3(0, 0, 0);
            // GameManager.Instance.Players[i].SetInitialPosition(position);
        }
    }

    public void StartTurn()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.OnTurnStart();
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
        if (CheckAllPlayersReady()) EndTurn();
    }

    private bool CheckAllPlayersReady()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            // Debug.Log($"Player {player.Index + 1} ready: {player.Ready}, alive: {player.Alive}");
            if (player.Alive && player.Ready == false) return false;
        }
        return true;
    }

    private void EndTurn()
    {
        // test rewind
        // if (CurrentTurn == 5) TimeTravelTriggered = true;

        if (TimeTravelTriggered && CurrentTurn > 1)
        {
            TimeTravelTriggered = false;
            int rewind_turn_number = Mathf.Min(REWIND_TURNS, CurrentTurn - 1);
            // Time travel here.
            Rewind(CurrentTurn - rewind_turn_number);
        }
        else
        {
            CalculateExplosions();
            CheckRoundEnd();
            UpdateSnapshots();
            // next turn here
            CurrentTurn++;
        }
        OnTurnChanged?.Invoke(CurrentTurn);
        StartTurn();
    }

    private void CalculateExplosions()
    {
        // TODO: for each bomb, update state of this turn.
        List<Transform> activeBombs = MapManager.Instance.GetActiveBombs();
        Debug.Log("Active Bombs succesully loaded.");
        Debug.Log($"Active Bombs count: {activeBombs.Count}");
        foreach (Transform activeBomb in activeBombs)
        {
            Debug.Log("Calling method of active bomb.");
            activeBomb.GetComponent<Bomb>().BombCountdown();
        }
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
        else if (survivalPlayerNum == 1)
        {
            RoundManager.Instance.EndRound(CurrentSurvivalPlayers);
        }
        else
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

    private void Rewind(int turnIndex)
    {
        int turn_num = CurrentTurn;
        if (snapshots.ContainsKey(turnIndex))
        {
            Snapshot snapshot = snapshots[turnIndex];
            CurrentTurn = snapshot.TurnIndex;
            PreviousSurvivalPlayers = snapshot.PreviousSurvivalPlayers;
            List<Vector2> breakableWalls = snapshot.breakableWalls;
            List<Vector2> unbreakableWalls = snapshot.unbreakableWalls;
            MapManager.Instance.ClearWalls();
            MapManager.Instance.SetWalls(breakableWalls, unbreakableWalls);
            Debug.Log($"Rewind to turn {turnIndex}.");
            foreach (Player player in GameManager.Instance.Players)
            {
                foreach (Player snapshotPlayer in snapshot.players)
                {
                    if (player.Index == snapshotPlayer.Index)
                    {
                        player.currentPosition = snapshotPlayer.currentPosition;
                        player.targetPosition = snapshotPlayer.targetPosition;
                        player.Alive = snapshotPlayer.Alive;
                        player.Ready = snapshotPlayer.Ready;
                        // player.ResourceManager = snapshotPlayer.ResourceManager;
                        player.ResourceManager.GetInventoryItemList();
                        player.gameObject.transform.position = player.currentPosition;
                    }
                }
            }
            // delete snapshots after rewinding
            for (int i = turnIndex; i <= turn_num; i++)
            {
                snapshots.Remove(i);
            }
        }
        else
        {
            Debug.LogError($"No snapshot found for turn {turnIndex}, cannot rewind.");
        }
    }
}