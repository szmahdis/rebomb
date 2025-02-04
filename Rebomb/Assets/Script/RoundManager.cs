using UnityEngine;
using System.Collections.Generic;


public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public EndgamePanel endRoundPanel;
    public int CurrentRound = 1;
    public static int MAX_ROUNDS = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartRound()
    {
        Debug.Log($"Round {CurrentRound} started.");
        if (CurrentRound > 1) {
            // Use the default map from SampleScene in the first round.
            GameObject map = GameObject.Find("Map");
            map.GetComponent<RandomWalkerGenerator>().GenerateMapButton();
        }
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            GameManager.Instance.Players[i].OnRoundStart();
        }
        TurnManager.Instance.Initialize();
        TurnManager.Instance.StartTurn();
    }

    public void EndRound(List<int> winners)
    {
        Debug.Log($"Round {CurrentRound} ended.");
        if (CheckAllRoundsEnd())
        {
            GameManager.Instance.EndGame(winners);
        }
        else
        {
            endRoundPanel.ShowResult(winners, CurrentRound);
            CurrentRound++;
        }
    }

    private bool CheckAllRoundsEnd()
    {
        return CurrentRound >= MAX_ROUNDS;
    }
}