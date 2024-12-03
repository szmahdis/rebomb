using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Search;

public class Snapshot {
    // turn state
    public int TurnIndex = 0;
    public List<int> PreviousSurvivalPlayers = new List<int>();

    // player states
    public List<PlayerData> players = new List<PlayerData>();

    // Breakable walls in list of (x, z) format coordinates 
    public List<Vector2> breakableWalls;
    // Unbreakable walls in list of (x, z) format coordinates 
    public List<Vector2> unbreakableWalls;
    public List<BombData> bombs;
    public List<GameObject> items;

    // TODO: map state
    public Snapshot(int turnIndex, List<int> previousSurvivalPlayers) {
        TurnIndex = turnIndex;
        PreviousSurvivalPlayers = previousSurvivalPlayers;
        // save a copy of player states, not changed by reference
        players = new List<PlayerData>();
        bombs = new List<BombData>();
        foreach (var player in GameManager.Instance.GetPlayers()) {
            players.Add(new PlayerData(player)); // Using the copy constructor
        }
        // players = GameManager.Instance.GetPlayers();
        // TODO: store map state (discussion: shall we decouple a map manager?)
        // map = TurnManager.Instance.GetMap();
        breakableWalls = MapManager.Instance.GetBreakableWalls();
        unbreakableWalls = MapManager.Instance.GetUnbreakableWalls();
        items = MapManager.Instance.GetItems();

        // save a copy of bomb states, not changed by reference
        foreach (var bomb in MapManager.Instance.GetBombs()) {
            if (bomb.bombExploded == false)
                bombs.Add(new BombData(bomb)); // Using the copy constructor
        }
    }

}