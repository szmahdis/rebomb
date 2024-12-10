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
    [SerializeField] private GameObject Items;

    public Transform floorParent;
    public Transform breakableWallsParent;
    public Transform unbreakableWallsParent;
    public Transform playersParent;
    public Transform bombsParent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
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

    public void SetBombs(List<BombData> bomb_data_list)
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
        foreach (BombData bomb_data in bomb_data_list)
        {
            GameObject bomb_prefab = BombConfigurator.Instance.GetConfig(bomb_data.bombType).bomb_prefab;
            GameObject bomb_object = Instantiate(bomb_prefab, bomb_data.position, Quaternion.identity);
            bomb_object.transform.parent = Bombs.transform;
            Bomb bomb = bomb_object.GetComponent<Bomb>();
            bomb.configure_from_data(bomb_data);
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
            GameObject wall = Instantiate(BreakableWallPrefab, new Vector3(position.x, 1.0f, position.y), Quaternion.identity);
            wall.transform.parent = Map.transform.Find("BreakableWall");
        }
        foreach (Vector2 position in unbreakableWalls)
        {
            GameObject wall = Instantiate(UnbreakableWallPrefab, new Vector3(position.x, 1.0f, position.y), Quaternion.identity);
            wall.transform.parent = Map.transform.Find("UnbreakableWall");
        }
    }
    public void CalculateExplosions()
    {
        // for each bomb, update state of this turn.
        foreach (Bomb bomb in Bombs.GetComponentsInChildren<Bomb>())
        {
            if (bomb.bombType != BombType.Passive)
            {
                bomb.BombCountdown();
            }
        }
        // find the final explosion range of each bomb, play the explosion effect and destroy bombs.
        HashSet<Vector3> exploded_tiles = new HashSet<Vector3>(new Vector3EqualityComparer());
        foreach (Bomb bomb in Bombs.GetComponentsInChildren<Bomb>())
        {
            if (bomb.is_triggered)
            {
                exploded_tiles.UnionWith(bomb.explosion.tiles);
                bomb.explosion.play();
            }
        }
        // Destroy breakable walls
        foreach (Transform child in breakableWallsParent.transform)
        {
            if (exploded_tiles.Contains(child.position))
            {
                // TODO(Yaxuan): destroy breakable wall animation
                Destroy(child.gameObject);
            }
        }    
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

    public List<GameObject> GetItems()
    {
        // Items are a child of Items
        List<GameObject> items = new List<GameObject>();
        foreach (Transform child in Items.transform)
        {
            Vector2 position = new Vector2(child.position.x, child.position.z);
            Debug.Log($"Item at {position}.");
            if (child.gameObject.activeSelf)
                items.Add(child.gameObject);
        }
        return items;
    }

    public void SetItems(List<GameObject> items)
    {
        foreach (GameObject item in items)
        {

            // if have "HourGlass" in name
            if (item.name.Contains("HourGlass"))
            {
                continue;
            }
            else item.SetActive(true);
        }
    }

    public void ClearItems()
    {
        foreach (Transform child in Items.transform)
        {
            // Destroy(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }


    public bool IsValidPosition(Vector3 position)
    {

        // Check if the position is within the floor grid
        if (!IsObstacleAtPosition(position - Vector3.down, floorParent)) return false;

        // Check if there is an obstacle at the current position by iterating through child objects
        if (IsObstacleAtPosition(position, breakableWallsParent) || IsObstacleAtPosition(position, unbreakableWallsParent) || IsObstacleAtPosition(position, bombsParent)
            || IsObstacleAtPosition(position, playersParent))
        {
            return false;
        }
        return true;
    }

    public bool IsObstacleAtPosition(Vector3 position, Transform parent)
    {

        foreach (Transform child in parent)
        {
            if (child != this && Mathf.Approximately(child.position.x, position.x) && Mathf.Approximately(child.position.z, position.z))
            {
                return true;
            }
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
