using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject Map;
    
    public List<Player> Players;
    [Header("Game Config")]
    // TODO: decide playerCount in MainMenu.
    [SerializeField] public int playerCount = 2;

    [Header("Prefabs and References")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject PlayerParent; 

    [SerializeField] private GameObject PlayerPanelPrefab;
    [SerializeField] private Transform PlayerPanelParent;

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
        StartGame(playerCount);
        RoundManager.Instance.StartRound();

    }

    public void StartGame(int playerCount)
    {

        InitializePlayers(playerCount);
        InitializePlayerPanels(playerCount);
        Debug.Log("Game Started!");
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
        Players = new List<Player>();
        Vector3[] playerPositions = new Vector3[4] {
            new Vector3(1.0f, 0.5f, 1.0f), new Vector3(1.0f, 0.5f, 6.0f),
            new Vector3(6.0f, 0.5f, 1.0f), new Vector3(6.0f, 0.5f, 6.0f)
        };
        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerObject = Instantiate(PlayerPrefab);
            Player player = playerObject.GetComponent<Player>();
            playerObject.transform.SetParent(PlayerParent.transform, false);
            player.Initialize(i, playerObject);
            playerObject.name = player.Name;
            player.SetInitialPosition(playerPositions[i]);
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

    private void InitializePlayerPanels(int playerCount) {
        // positioning -> corners of parent panel
        RectTransform parentRectTransform = PlayerPanelParent.GetComponent<RectTransform>();
        if (parentRectTransform == null)
        {
            Debug.LogError("Parent panel is missing RectTransform!");
            return;
        }
        Vector3[] corners = new Vector3[4];
        parentRectTransform.GetWorldCorners(corners);
        Vector3[] panelCorners = new Vector3[4]{ corners[0], corners[3], corners[1], corners[2] };
        Vector2[] panelPivots = new Vector2[4]{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };        
        
        // instantiate panels
        for (int i = 0; i < playerCount; i++)
        {
            GameObject panel = Instantiate(PlayerPanelPrefab, PlayerPanelParent);
            panel.name = $"PlayerPanel{i+1}";

            RectTransform panel_position = panel.GetComponent<RectTransform>();
            panel_position.position = panelCorners[i];
            panel_position.pivot = panelPivots[i];
            PlayerResourcePanel panel_script = panel.GetComponent<PlayerResourcePanel>();
            ResourceManager resource_manager = Players[i].ResourceManager;
            if (resource_manager != null && panel_script != null)
            {
                panel_script.ResourceManager = resource_manager;
                panel_script.playerIndex = i;
                panel_script.enabled = true;
            }
            // Note: set active explicitly here, to call OnEnable() to subscribe event. 
            panel.SetActive(true);
        }
    }
}
