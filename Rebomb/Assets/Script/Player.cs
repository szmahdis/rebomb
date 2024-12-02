using UnityEngine;
using UnityEngine.InputSystem;
using static System.Math;

public class Player : MonoBehaviour
{
    private GameObject playerObject;
    public ResourceManager ResourceManager { get; private set; }
    public GameManager GameManager;

    [Header("Player States")]
    public int Index { get; private set; }
    public string Name { get; private set; }
    public bool Alive { get; set; }
    public bool Ready { get; set; }
    public Vector3 currentPosition;

    [Header("Map Members")]
    public float gridHeight = 0.5f; // Fixed Y-axis position
    public int gridSize = 8; // Defines the grid range
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


    void Start()
    {
        floorParent = GameObject.Find("Floor").transform;
        breakableWallsParent = GameObject.Find("BreakableWall").transform;
        unbreakableWallsParent = GameObject.Find("UnbreakableWall").transform;
        playersParent = GameObject.Find("Players").transform;
        bombsParent = GameObject.Find("Bombs").transform;
    }


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
        currentPosition = new Vector3(Mathf.Round(initialPosition.x), gridHeight, Mathf.Round(initialPosition.z));
        playerObject.transform.position = currentPosition;
    }

    public void OnPlayerReady()
    {
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
        if (context.performed) { OnPlayerReady(); }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput == Vector2.zero) return;
        if (Alive == false || (Alive == true && Ready == true)) return;
        if (isMoving) return;

        Vector3 proposedPosition = currentPosition + new Vector3((int)moveInput.x, 0, (int)moveInput.y);

        // TODO: refactoring, move validation check and map member to map manager.
        // map.getComponent<MapManager>().IsValidPosition(position);
        if (IsValidPosition(proposedPosition) == false) return;

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

    private void PlaceBomb(BombType bombType)
    {
        if (Alive == false) return;
        if (Alive == true && Ready == true) return;
        if (isMoving) return;

        if (IsObstacleAtPosition(currentPosition, bombsParent))
        {
            Debug.Log($"A bomb is already at {currentPosition}!");
            return;
        }
        if (ResourceManager.OnBombPlaced(bombType) == false)
        {
            Debug.Log($"Not enough coin for {bombType} bomb.");
            return;
        }
        GameObject bombPrefab = null;
        if (bombType == BombType.Active)
        {
            bombPrefab = activeBombPrefab;
        }
        else
        {
            bombPrefab = passiveBombPrefab;
        }
        GameObject newBomb = Instantiate(bombPrefab, currentPosition, Quaternion.identity);
        newBomb.transform.parent = bombsParent;
        Debug.Log("Bomb placed at: " + currentPosition);
    }
    public void Die() {
        // Play death animation
        // Notify the game manager
        GameManager.Instance.PlayerDied(this);
        // Destroy the player object
        Destroy(playerObject);
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
    private bool IsValidPosition(Vector3 position)
    {
        // TODO: move to map manager.
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

    private bool IsObstacleAtPosition(Vector3 position, Transform parent)
    {
        // TODO: move to map manager.
        foreach (Transform child in parent)
        {
            if (child != this &&Mathf.Approximately(child.position.x, position.x) && Mathf.Approximately(child.position.z, position.z))
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            //Touching item
            ResourceManager.AddInventoryItem(itemWorld.GetItem());
            itemWorld.DestroySelf();

        }
    }

}