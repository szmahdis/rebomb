using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    public int Index { get; private set; }
    public string Name { get; private set; }
    public bool Alive { get; set; }
    public bool Ready { get; set; }
    public ResourceManager ResourceManager { get; private set; }

    public void Initialize(int i, GameObject thisPlayerObject)
    {
        Index = i;
        Name = $"Player {i + 1}";
        playerObject = thisPlayerObject;
        ResourceManager = gameObject.AddComponent<ResourceManager>();
        Debug.Log($"{Name} initialized with resource manager.");
        return;
    }

    public void SetInitialPosition(Vector3 initialPosition) {
        Debug.Log($"{Name}'s initial position is {playerObject.transform.position}.");
        // TODO: integrate with CharacterMovement.cs
        // maybe calling CharacterGridMovement.MoveToPosition(targetPosition)?
        playerObject.transform.position = initialPosition;
    }

    public void OnPlayerReady() {
        if (Alive == false) return;
        if (Ready == true) return;
        
        // state update
        Debug.Log($"{Name} is ready now.");
        Ready = true;

        // disable movement and bomb placement.
        playerObject.GetComponent<CharacterGridMovement>().enabled = false;
        playerObject.GetComponent<CharacterPlaceBomb>().enabled = false;
    }

    public void OnTurnStart() {
        if (Alive == false) return;

        // state & resource update
        Ready = false;
        ResourceManager.OnTurnStart();

        // enable player movement
        playerObject.GetComponent<CharacterGridMovement>().enabled = true;
        playerObject.GetComponent<CharacterPlaceBomb>().enabled = true; 
    }

    void Update() {
        if (Alive == false || (Alive == true && Ready == true)) return;
        // Only response to player input when alive and not ready.

        // following operation showcase how to trigger resource update.
        // TODO: Replace them by calling OnBombPlaced and OnStepTaken in CharacterPlaceBomb and CharacterGridMovement.
        // maybe: WASD for movement and C for place bomb and V for ready of player 1
        // maybe: IJKL for movement and N for place bomb and M for ready of player 2
        if (Input.GetKeyDown(KeyCode.C) && Index == 0)
        {
            ResourceManager.OnBombPlaced(BombType.Active);
        }
        if (Input.GetKeyDown(KeyCode.V) && Index == 0)
        {
            ResourceManager.OnStepTaken();
        }

        if (Input.GetKeyDown(KeyCode.N) && Index == 1)
        {
            ResourceManager.OnBombPlaced(BombType.Active);
        }
        if (Input.GetKeyDown(KeyCode.M) && Index == 1)
        {
            ResourceManager.OnStepTaken();
        }

    }
}