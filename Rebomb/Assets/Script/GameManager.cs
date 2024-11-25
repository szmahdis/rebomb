using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject PlayerParent; 
    [SerializeField] private GameObject Map; 
    public static GameManager Instance { get; private set; }
    public List<Player> Players;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
            GameObject playerObject = Instantiate(PlayerPrefab);
            Player player = playerObject.GetComponent<Player>();
            playerObject.transform.SetParent(PlayerParent.transform, false);
            player.Initialize(i, playerObject);
            playerObject.name = player.Name;
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
            MeshRenderer renderer = playerObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = true;
            }
            Collider collider = playerObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            foreach (MonoBehaviour script in playerObject.GetComponents<MonoBehaviour>())
            {
                script.enabled = true;
            }
            playerObject.SetActive(true);
            Players.Add(player);

        }
    }
}
