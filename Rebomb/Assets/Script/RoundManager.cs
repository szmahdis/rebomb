using UnityEngine;
using System.Collections.Generic;


public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private int CurrentRound = 1;
    static int MAX_ROUNDS = 1;

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
        TurnManager.Instance.Initialize();
        TurnManager.Instance.StartTurn();
    }

    public void EndRound(List<int> winners)
    {
        Debug.Log($"Round {CurrentRound} ended.");
        if (CheckAllRoundsEnd() == true)
        {
            // TODO: decide winners if there are multiple rounds.
            GameManager.Instance.EndGame(winners);
        }
        else
        {
            CurrentRound++;
            StartRound();
        }
    }

    private bool CheckAllRoundsEnd()
    {
        return CurrentRound >= MAX_ROUNDS;
    }
}