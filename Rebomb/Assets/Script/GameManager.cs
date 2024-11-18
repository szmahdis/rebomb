using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    // [SerializeField] private GameObject PlayersParent; 
    public static GameManager Instance { get; private set; }
    public List<Player> Players;
    public List<GameObject> PlayerObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // PlayersParent = GameObject.Find("Players");
        // PlayersParent.SetActive(true);
        // DontDestroyOnLoad(PlayersParent);
    }

    private void Start()
    {
        Players = new List<Player>();

        // TODO: decide playerCount in GUI config.
        StartGame(2);
        RoundManager.Instance.StartRound();

    }

    public void StartGame(int playerCount)
    {
        Debug.Log("Game Started!");

        InitializePlayers(playerCount);
    }

    public void EndGame(List<int> winners)
    {
        Debug.Log("Game Over!");
        Debug.Log("Winners: ");
        foreach (int winner in winners)
        {
            Debug.Log($"\t{Players[winner].Name}");
        }
        // TODO: show checkout scene.
    }

    public List<Player> GetPlayers()
    {
        List<Player> playerList = new List<Player>();
        foreach (Player player in Players)
        {
            playerList.Add(player);
        }
        return playerList;
    }
    private void InitializePlayers(int playerCount)
    {
        Debug.Log($"Initializing {playerCount} Players...");
        Players.Clear();
        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerObject = GameObject.Find($"Player {i + 1}");
            if (playerObject == null)
            {
                Debug.Log($"We have no Player {i + 1}.");
            }   
            // TODO: bugfix
            // Now we could only instantiate the first player's GameObject
            // but have all players' Player script.
            Player player = playerObject.GetComponent<Player>();
            
            player.Initialize(i, playerObject);

            Vector3 position;
            switch (i) {
                case 0:
                    position = new Vector3(1.0f, 0.5f, 1.0f);
                    break;
                case 1:
                    position = new Vector3(6.0f, 0.5f, 6.0f);
                    break;
                default:
                    position = new Vector3(0.0f, 0.5f, 0.0f);
                    break;
            }
            player.SetInitialPosition(position);
            Players.Add(player);
            PlayerObjects.Add(playerObject);
            print("Now we have " + Players.Count + " players.");
            Debug.Log($"Player {i + 1}, place: {playerObject.transform.position}");

            // Following commented code is for creating new player objects from prefab.
            // Which get the same problem on instantiation right now.
            // GameObject playerObject = Instantiate(PlayerPrefab);
            // Player player = playerObject.GetComponent<Player>();
            // player.Initialize(i, playerObject);
            // playerObject.transform.SetParent(PlayersParentObject.transform, false);
            // playerObject.name = player.Name;
            // player.SetInitialPosition(new Vector3(0, 0, 0));
            // MeshRenderer renderer = playerObject.GetComponent<MeshRenderer>();
            // if (renderer != null)
            // {
            //     renderer.enabled = true;
            // }
            // Collider collider = playerObject.GetComponent<Collider>();
            // if (collider != null)
            // {
            //     collider.enabled = true;
            // }
            // foreach (MonoBehaviour script in playerObject.GetComponents<MonoBehaviour>())
            // {
            //     script.enabled = true;
            // }
            // playerObject.SetActive(true);
        }
    }
}
