using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; set; }
    [SerializeField] private GameObject Map;
    [SerializeField] private GameObject Bombs;

    void Awake() {
    if (Instance == null) {
        Instance = this;
    } else {
        Destroy(gameObject);
    }
}
    public List<Vector2> GetBreakableWalls()
    {
        // Breakable is child of Map
        Transform BreakableWall = Map.transform.Find("BreakableWall");
        List<Vector2> breakableWalls = new List<Vector2>();
        foreach (Transform child in BreakableWall.transform)
        {
            Vector2 position = new Vector2(child.position.x, child.position.z);
            // Debug.Log($"Breakable wall at {position}.");
            breakableWalls.Add(position);
        }
        return breakableWalls;
    }

    public List<Vector2> GetUnbreakableWalls()
    {
        // Unbreakable is child of Map
        Transform UnbreakableWall = Map.transform.Find("UnbreakableWall");
        List<Vector2> unbreakableWalls = new List<Vector2>();
        foreach (Transform child in UnbreakableWall.transform)
        {
            Vector2 position = new Vector2(child.position.x, child.position.z);
            // Debug.Log($"Unbreakable wall at {position}.");
            unbreakableWalls.Add(position);
        }
        return unbreakableWalls;
    }

    public List<Transform> GetActiveBombs()
    {
        // Bombs are a child of Bombs
        Transform ActiveBomb = Bombs.transform;
        List<Transform> activeBombs = new List<Transform>();
        foreach (Transform child in ActiveBomb.transform)
        {
            if (child.GetComponent<Bomb>().bombType == BombType.Active)
            {
            Vector2 position = new Vector2(child.position.x, child.position.z);
            Debug.Log($"Active bomb at {position}.");
            activeBombs.Add(child);
            }
        }
        return activeBombs;
    }
    
    public List<Bomb> GetPassiveBombs()
    {
        // Breakable is child of Map
        Transform PassiveBombs = Bombs.transform;
        List<Bomb> passiveBombs = new List<Bomb>();
        foreach (Bomb child in PassiveBombs.transform)
        {
            if (child.GetComponent<Bomb>().bombType == BombType.Passive)
            {
            Vector2 position = new Vector2(child.transform.position.x, child.transform.position.z);
            Debug.Log($"Passive bomb at {position}.");
            passiveBombs.Add(child);
            }
        }
        return passiveBombs;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
