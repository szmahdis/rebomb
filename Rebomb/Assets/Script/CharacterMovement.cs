using UnityEngine;

public class CharacterGridMovement : MonoBehaviour
{
    public float speed = 5.0f; // Controls the movement speed
    public float gridHeight = 0.5f; // Fixed Y-axis position
    public int gridSize = 8; // Defines the grid range
    public Transform breakableWallsParent;
    public Transform unbreakableWallsParent;
    public Transform bombsParent;

    private Vector3 targetPosition;

    void Start()
    {
        // Initialize the character's position to the nearest grid point
        targetPosition = new Vector3(Mathf.Round(transform.position.x), gridHeight, Mathf.Round(transform.position.z));
        transform.position = targetPosition;
    }

    void Update()
    {
        Vector3 proposedPosition = targetPosition;

        // Check input and calculate the target position
        if (Input.GetKeyDown(KeyCode.W))
        {
            proposedPosition += new Vector3(0, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            proposedPosition += new Vector3(0, 0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            proposedPosition += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            proposedPosition += new Vector3(1, 0, 0);
        }

        // Ensure the target position is at the correct height
        proposedPosition.y = gridHeight;

        // Check if the position is within the grid range and if there are obstacles
        if (IsValidPosition(proposedPosition))
        {
            targetPosition = proposedPosition;
            // TODO: Moving Resource -1
        }

        // Smoothly move to the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    bool IsValidPosition(Vector3 position)
    {
        // Check if the position is within the grid range
        if (position.x < 0 || position.x >= gridSize || position.z < 0 || position.z >= gridSize)
        {
            return false;
        }

        // Check if there is an obstacle at the current position by iterating through child objects
        if (IsObstacleAtPosition(position, breakableWallsParent) || IsObstacleAtPosition(position, unbreakableWallsParent) || IsObstacleAtPosition(position, bombsParent))
        {
            return false;
        }
        // TODO: Check whether Moving Resource is enough

        return true;
    }

    bool IsObstacleAtPosition(Vector3 position, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (Mathf.Approximately(child.position.x, position.x) && Mathf.Approximately(child.position.z, position.z))
            {
                return true;
            }
        }
        return false;
    }
}
