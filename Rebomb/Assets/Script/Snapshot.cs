using UnityEngine;
using System.Collections.Generic;

public class Snapshot {
    // turn state
    public int TurnIndex = 0;
    public List<int> PreviousSurvivalPlayers = new List<int>();

    // player states
    List<Player> players = new List<Player>();

    // Breakable walls in list of (x, z) format coordinates 
    public List<Vector2> breakableWalls;
    // Unbreakable walls in list of (x, z) format coordinates 
    public List<Vector2> unbreakableWalls;

    // TODO: map state

    public Snapshot(int turnIndex, List<int> previousSurvivalPlayers) {
        TurnIndex = turnIndex;
        PreviousSurvivalPlayers = previousSurvivalPlayers;
        players = GameManager.Instance.GetPlayers();
        // TODO: store map state (discussion: shall we decouple a map manager?)
        // map = TurnManager.Instance.GetMap();
        breakableWalls = MapManager.Instance.GetBreakableWalls();
        unbreakableWalls = MapManager.Instance.GetUnbreakableWalls();
    }

}