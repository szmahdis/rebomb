using UnityEngine;
using UnityEngine.InputSystem;
using static System.Math;

public class Player : MonoBehaviour
{
    private GameObject playerObject;

    [Header("Game Objects")]
    public ResourceManager ResourceManager { get; set; }
    public PlayerReadyButton readyButton;

    [Header("Player States")]
    public int Index { get; private set; }
    public string Name { get; private set; }
    public bool Alive { get; set; }
    public bool Ready { get; set; }
    public Vector3 currentPosition;

    [Header("Map Members")]
    public float gridHeight = 1.5f; // Fixed Y-axis position
    //public int gridSize = 8; // Defines the grid range
    public Transform floorParent;
    public Transform breakableWallsParent;
    public Transform unbreakableWallsParent;
    public Transform playersParent;

    [Header("Movement")]
    public float speed = 5.0f; // Controls the movement speed
    public bool isMoving = false;
    public Vector2 moveInput;
    public Vector3 targetPosition; // The position to move towards


    [Header("Bomb Placement")]
    public GameObject activeBombPrefab;  // Prefab for the active bomb
    public GameObject passiveBombPrefab; // Prefab for the passive bomb
    public Transform bombsParent; // Parent object to hold all placed bombs
    public Bomb LastBomb { get; set; }


    void Start()
    {
        floorParent = GameObject.Find("Floor").transform;
        breakableWallsParent = GameObject.Find("BreakableWall").transform;
        unbreakableWallsParent = GameObject.Find("UnbreakableWall").transform;
        playersParent = GameObject.Find("Players").transform;
        bombsParent = GameObject.Find("Bombs").transform;
    }

    // Copy Constructor
    // public Player(Player player)
    // {
    //     Index = player.Index;
    //     Name = player.Name;
    //     Alive = player.Alive;
    //     Ready = player.Ready;
    //     currentPosition = player.currentPosition;
    //     ResourceManager = new ResourceManager(player.ResourceManager);
    //     LastBomb = player.LastBomb;
    //     readyButton = player.readyButton;
    // }

    public void Initialize(int i, GameObject thisPlayerObject)
    {
        Index = i;
        Name = $"Player {i + 1}";
        playerObject = thisPlayerObject;
        ResourceManager = gameObject.AddComponent<ResourceManager>();
        Debug.Log($"{Name} initialized with resource manager.");
        return;
    }

    public void SetInitialPosition(Vector3 initialPosition)
    {
        currentPosition = new Vector3(Mathf.Round(initialPosition.x), 0.5f, Mathf.Round(initialPosition.z));
        playerObject.transform.position = currentPosition;
    }

    public void OnPlayerReady()
    {
        // called only by the player ready button.
        
        if (Alive == false) return;
        if (Ready == true) return;

        // state update
        Debug.Log($"{Name} is ready now.");
        Ready = true;
        TurnManager.Instance.MarkPlayerReady(Index);
    }

    public void OnTurnStart()
    {
        if (Alive == false) return;

        // state & resource update
        Ready = false;
        ResourceManager.OnTurnStart();
    }
    public void OnReady(InputAction.CallbackContext context)
    {
        if (Alive == false) return;

        // event pass, keyboard input -> button click
        if (context.performed) {
            readyButton.OnReadyButtonClicked();
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput == Vector2.zero) return;
        if (Alive == false || (Alive == true && Ready == true)) return;
        if (isMoving) return;

        Vector3 proposedPosition = currentPosition + new Vector3((int)moveInput.x, 0, (int)moveInput.y);

        if (MapManager.Instance.IsValidPosition(proposedPosition) == false) return;

        int step_num = Abs((int)moveInput.x) + Abs((int)moveInput.y);
        if (ResourceManager.OnStepTaken(step_num) == false) return;

        targetPosition = proposedPosition;
        isMoving = true;
    }

    public void OnActiveBomb(InputAction.CallbackContext context)
    {
        if (context.performed) { PlaceBomb(BombType.Active); }
    }
    public void OnPassiveBomb(InputAction.CallbackContext context)
    {
        if (context.performed) { PlaceBomb(BombType.Passive); }
    }
    public void OnChainBomb(InputAction.CallbackContext context)
    {
        if (context.performed) { PlaceBomb(BombType.ChainBomb); }
    }

    public void PlaceBomb(BombType bombType)
    {
        if (Alive == false) return;
        if (Alive == true && Ready == true) return;
        if (isMoving) return;

        if (MapManager.Instance.IsObstacleAtPosition(currentPosition, bombsParent))
        {
            Debug.Log($"A bomb is already at {currentPosition}!");
            return;
        }
        if (ResourceManager.OnBombPlaced(bombType) == false)
        {
            Debug.Log($"Not enough coin for {bombType} bomb.");
            return;
        }
        GameObject bomb_prefab = BombConfigurator.Instance.GetPrefab(bombType);
        GameObject bomb_object = Instantiate(bomb_prefab, currentPosition, Quaternion.identity);
        bomb_object.transform.parent = bombsParent;
        Bomb bomb = bomb_object.GetComponent<Bomb>();
        bomb.configure_from_type(bombType);
        if (LastBomb != null)
            LastBomb.gameObject.layer = LayerMask.NameToLayer("Default");
        LastBomb = bomb;
        int LastBombLayer = LayerMask.NameToLayer("Last Bombs");
        if (LastBombLayer == -1)
        {
            Debug.LogError("Layer 'Last Bombs' not found!");
            return;
        }
        LastBomb.gameObject.layer = LastBombLayer;
        Debug.Log("Bomb placed at: " + currentPosition);
    }

    public void OnKilled() {
        if (Alive == false) return;
        
        // TODO: Play death animation

        playerObject.GetComponent<MeshRenderer>().enabled = false;
        playerObject.GetComponent<Collider>().enabled = false;
        playerObject.SetActive(false);
        Alive = false;
    }

    void Update()
    {
        if (Alive == false || (Alive == true && Ready == true)) return;

        if (isMoving)
        {
            playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, targetPosition, speed * Time.deltaTime);
            if (playerObject.transform.position == targetPosition)
            {
                isMoving = false;
                currentPosition = targetPosition;
            }
        }


    }
   
    private void OnTriggerEnter(Collider collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            //Touching item
            ResourceManager.AddInventoryItem(itemWorld.GetItem());
            // itemWorld.DestroySelf();
            itemWorld.DisableSelf();

        }
    }

}

[System.Serializable]
public class PlayerData
{
    public int Index;
    public string Name;
    public bool Alive;
    public Vector3 currentPosition;
    public ResourceInfo ResourceInfo;
    public Bomb LastBomb;

    
    // Add other necessary fields
    public PlayerData(Player player)
    {
        Index = player.Index;
        Name = player.Name;
        Alive = player.Alive;
        currentPosition = player.currentPosition;
        ResourceInfo = new ResourceInfo(player.ResourceManager);
        LastBomb = player.LastBomb;
    }
}
