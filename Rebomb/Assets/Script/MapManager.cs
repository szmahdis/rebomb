using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; set; }
    [SerializeField] private GameObject Map;
    [SerializeField] private GameObject BreakableWallPrefab;
    [SerializeField] private GameObject UnbreakableWallPrefab;
    [SerializeField] private GameObject ActiveBombPrefab;
    [SerializeField] private GameObject PassiveBombPrefab;
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

    public List<Bomb> GetBombs()
    {
        // Bombs are a child of Bombs
        Transform ActiveBomb = Bombs.transform;
        List<Bomb> bombs = new List<Bomb>();
        foreach (Transform child in ActiveBomb.transform)
        {
            Vector2 position = new Vector2(child.position.x, child.position.z);
            Debug.Log($"Active bomb at {position}.");
            bombs.Add(child.GetComponent<Bomb>());
        }
        return bombs;
    }

    public void SetBombs(List<BombData> bombs)
    {
        // Add Lastbombs of each player to the list
        // foreach (Player player in GameManager.Instance.Players)
        // {
        //     if (player.LastBomb != null)
        //     {
        //         BombData lastBomb = new BombData(player.LastBomb);
        //         bombs.Add(lastBomb);
        //     }
        // }
        foreach (BombData bomb in bombs)
        {
            switch (bomb.bombType)
            {
                case BombType.Active:
                    GameObject activeBomb = Instantiate(ActiveBombPrefab, bomb.position, Quaternion.identity);
                    activeBomb.transform.parent = Bombs.transform;
                    activeBomb.GetComponent<Bomb>().turnsToExplosion = bomb.turnsToExplosion;
                    activeBomb.GetComponent<Bomb>().bombType = bomb.bombType;
                    activeBomb.GetComponent<Bomb>().bombExploded = bomb.bombExploded;
                    activeBomb.GetComponent<Bomb>().explosionDirections = bomb.explosionDirections;
                    activeBomb.GetComponent<Bomb>().maxExplosionDistance = bomb.maxExplosionDistance;


                    break;
                case BombType.Passive:
                    GameObject passiveBomb = Instantiate(PassiveBombPrefab, bomb.position, Quaternion.identity);
                    passiveBomb.transform.parent = Bombs.transform;
                    passiveBomb.GetComponent<Bomb>().turnsToExplosion = bomb.turnsToExplosion;
                    passiveBomb.GetComponent<Bomb>().bombType = bomb.bombType;
                    passiveBomb.GetComponent<Bomb>().bombExploded = bomb.bombExploded;
                    passiveBomb.GetComponent<Bomb>().explosionDirections = bomb.explosionDirections;
                    passiveBomb.GetComponent<Bomb>().maxExplosionDistance = bomb.maxExplosionDistance;
                    break;
            }
        }
    }

    public void ClearWalls()
    {
        Transform BreakableWall = Map.transform.Find("BreakableWall");
        foreach (Transform child in BreakableWall.transform)
        {
            Destroy(child.gameObject);
        }
        Transform UnbreakableWall = Map.transform.Find("UnbreakableWall");
        foreach (Transform child in UnbreakableWall.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetWalls(List<Vector2> breakableWalls, List<Vector2> unbreakableWalls)
    {
        foreach (Vector2 position in breakableWalls)
        {
            GameObject wall = Instantiate(BreakableWallPrefab, new Vector3(position.x, 0.5f, position.y), Quaternion.identity);
            wall.transform.parent = Map.transform.Find("BreakableWall");
        }
        foreach (Vector2 position in unbreakableWalls)
        {
            GameObject wall = Instantiate(UnbreakableWallPrefab, new Vector3(position.x, 0.5f, position.y), Quaternion.identity);
            wall.transform.parent = Map.transform.Find("UnbreakableWall");
        }
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

    public void ClearBombs()
    {
        Transform ActiveBomb = Bombs.transform;
        foreach (Transform child in ActiveBomb.transform)
        {
            bool skip = false;
            // Don't remove last bombs of each player
            foreach (Player player in GameManager.Instance.Players)
            {
                if (player.LastBomb && child == player.LastBomb.gameObject.transform)
                {
                    skip = true;
                }
            }
            if (!skip)
            {
                Destroy(child.gameObject);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
