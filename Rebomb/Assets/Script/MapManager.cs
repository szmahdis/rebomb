using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; set; }
    [SerializeField] private GameObject Map;

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
            Debug.Log($"Breakable wall at {position}.");
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
            Debug.Log($"Unbreakable wall at {position}.");
            unbreakableWalls.Add(position);
        }
        return unbreakableWalls;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
