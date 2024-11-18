using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    public int Index { get; private set; }
    public string Name { get; private set; }
    public bool Alive { get; set; }
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

    public void FinishTurn()
    {
        TurnManager.Instance.MarkPlayerReady(Index);
    }

    public void SetInitialPosition(Vector3 initialPosition) {
        UpdatePosition(initialPosition);
        Debug.Log($"{Name}'s initial position is {playerObject.transform.position}.");
    }

    public void UpdatePosition(Vector3 targetPosition) {
        // TODO: integrate with CharacterMovement.cs
        playerObject.transform.position = targetPosition;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F) && Index == 0)
        {
            FinishTurn();
        }
        if (Input.GetKeyDown(KeyCode.J) && Index == 1)
        {
            FinishTurn();
        }
    } 
}