using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private static int REWIND_TURNS = 3;
    public int CurrentTurn { get; private set; }
    public Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot>();
    public bool TimeTravelTriggered { get; set; }
    public GameObject turnPanel; // Assign the TurnPanel in the Inspector
    public TextMeshProUGUI turnText; // Assign the TurnText in the Inspector
    List<int> PreviousSurvivalPlayers = new List<int>();
    public AudioClip rewindClip;

    // event
    public event System.Action<int> OnTurnChanged;

    bool RoundEnd = false;
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
        snapshots.Clear();
        CurrentTurn = 0;
        UpdateSnapshots();
        OnTurnChanged?.Invoke(CurrentTurn);
        RoundEnd = false;
        CurrentTurn = 1;
        TimeTravelTriggered = false;
        OnTurnChanged?.Invoke(CurrentTurn);
    }

    public void StartTurn()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.OnTurnStart();
        }
        Debug.Log($"Turn {CurrentTurn} Started.");
        StartCoroutine(ShowTurnPanel());
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
        if (TimeTravelTriggered && CurrentTurn > 1)
        {
            TimeTravelTriggered = false;
            int rewind_turn_number = Mathf.Min(REWIND_TURNS, CurrentTurn);
            // Time travel here.
            Rewind(CurrentTurn - rewind_turn_number);
        }
        else
        {
            MapManager.Instance.CalculateExplosions();
            CheckRoundEnd();
            UpdateSnapshots();
            // next turn here
            CurrentTurn++;
        }
        OnTurnChanged?.Invoke(CurrentTurn);
        StartTurn();
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
            RoundEnd = true;
        }
        else if (survivalPlayerNum == 1)
        {
            RoundManager.Instance.EndRound(CurrentSurvivalPlayers);
            RoundEnd = true;
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

    public void Rewind(int turnIndex)
    {
        int turn_num = CurrentTurn;
        if (snapshots.ContainsKey(turnIndex))
        {
            Snapshot snapshot = snapshots[turnIndex];
            CurrentTurn = snapshot.TurnIndex+1;
            PreviousSurvivalPlayers = snapshot.PreviousSurvivalPlayers;
            List<Vector2> breakableWalls = snapshot.breakableWalls;
            List<Vector2> unbreakableWalls = snapshot.unbreakableWalls;
            MapManager.Instance.ClearWalls();
            MapManager.Instance.SetWalls(breakableWalls, unbreakableWalls);
            MapManager.Instance.ClearBombs();
            MapManager.Instance.SetBombs(snapshot.bombs);
            MapManager.Instance.ClearItems();
            MapManager.Instance.SetItems(snapshot.items);
            AudioManager.Instance.PlaySoundEffect(rewindClip);
            Debug.Log($"Rewind to turn {turnIndex}.");
            foreach (Player player in GameManager.Instance.Players)
            {
                foreach (PlayerData snapshotPlayer in snapshot.players)
                {
                    if (player.Index == snapshotPlayer.Index)
                    {
                        player.currentPosition = snapshotPlayer.currentPosition;
                        player.Alive = snapshotPlayer.Alive;
                        player.ResourceManager.SetCoins(snapshotPlayer.ResourceInfo.coins);
                        player.ResourceManager.SetSteps(snapshotPlayer.ResourceInfo.steps);
                        player.gameObject.transform.position = player.currentPosition;
                        player.LastBomb = snapshotPlayer.LastBomb;
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

    public Texture2D GetSnapshotImage()
    {
        int rewind_turn_number = Mathf.Min(REWIND_TURNS, CurrentTurn);
        if (snapshots.ContainsKey(CurrentTurn - rewind_turn_number))
        {
            // deep copy snapshot image
            return snapshots[CurrentTurn - rewind_turn_number].snapshotImage;
        }
        else
        {
            Debug.LogError($"No snapshot found for turn {CurrentTurn}, cannot get snapshot image.");
            return null;
        }
    }

    private System.Collections.IEnumerator ShowTurnPanel()
    {
        if (RoundEnd) yield break;
        turnText.text = $"Turn {CurrentTurn}..."; // Update the text
        // change text color
        turnText.color = Color.yellow;
        turnPanel.SetActive(true); // Show the panel
        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds
        turnText.text = $"Go!";
        turnText.color = Color.red;
        Invoke(nameof(HideTurnPanel), 0.5f); // Hide the panel after 1.5 seconds
    }

    private void HideTurnPanel()
    {
        turnPanel.SetActive(false); // Hide the panel
    }
}