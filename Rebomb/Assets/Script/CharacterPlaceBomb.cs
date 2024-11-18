using System.Diagnostics;
using UnityEngine;

public class CharacterPlaceBomb : MonoBehaviour
{
    public GameObject bombPrefab; // Prefab for the bomb to be placed
    public Transform bombsParent; // Parent object to hold all placed bombs

    private Vector3 currentGridPosition;
    public float gridHeight = 0.5f; // The fixed height where the bomb should be placed

    void Start()
    {
        // Initialize the current grid position
        currentGridPosition = new Vector3(Mathf.Round(transform.position.x), gridHeight, Mathf.Round(transform.position.z));
        bombsParent = GameObject.Find("Bombs").transform;
    }

    void Update()
    {
        // Update current grid position based on the character's position
        currentGridPosition = new Vector3(Mathf.Round(transform.position.x), gridHeight, Mathf.Round(transform.position.z));

        // Place a bomb when the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceBomb();
        }
    }

    void PlaceBomb()
    {
        // Check if a bomb already exists at the current grid position
        foreach (Transform bomb in bombsParent)
        {
            if (Mathf.Approximately(bomb.position.x, currentGridPosition.x) && Mathf.Approximately(bomb.position.z, currentGridPosition.z))
            {
                UnityEngine.Debug.Log("A bomb is already placed here!");
                return;
            }
        }

        // TODO: Check whether having enough resource to place bomb


        // Instantiate a bomb at the current grid position
        GameObject newBomb = Instantiate(bombPrefab, currentGridPosition, Quaternion.identity);
        newBomb.transform.parent = bombsParent; // Set the parent to maintain hierarchy

        UnityEngine.Debug.Log("Bomb placed at: " + currentGridPosition);
    }
}
