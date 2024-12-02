using UnityEngine;

public enum BombType
{
    Active, Passive
}

public class Bomb : MonoBehaviour
{
    public BombType bombType;
    private int turnsToExplosion = 3; // Time until explosion in turns
    float maxExplosionDistance = 2f;  // Explosion range
    bool bombExploded = false;        // Flag to avoid infinite loops
    private Vector3[] explosionDirections = new Vector3[]
    {
        Vector3.forward,  // Up
        Vector3.back,     // Down
        Vector3.left,     // Left
        Vector3.right     // Right
    };

    void Start()
    {
    }


    public void BombCountdown()
    {
        Debug.Log("Trying Bomb countdown.");
        if (bombType == BombType.Active)
        {
            Debug.Log("Bomb countdown.");
            turnsToExplosion = turnsToExplosion - 1;
            if (turnsToExplosion <= 0)
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        if (bombExploded) return; // Avoid infinite loops
        bombExploded = true;
        float rayDuration = 20f; // Debug ray duration
        Debug.Log("Bomb exploded!");
        // Play explosion animation
        // Destroy the bomb itself after exploding
        Destroy(gameObject);
        // Check explosion in all directions
        foreach (Vector3 direction in explosionDirections)
        {
            // Cast a ray from the bomb's position in the specified direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxExplosionDistance))
            {
                Debug.DrawLine(transform.position, transform.position + direction * hit.distance, Color.red, rayDuration); // Debug ray (optional)

                // Check if the object is a destructible one
                if (hit.collider.CompareTag("Player")) {
                    Player player = hit.collider.GetComponent<Player>();
                    Debug.Log("Player hit by bomb.");
                    player.Die();
                    continue;   // Stop after hitting the first object
                }
                else if (hit.collider.CompareTag("Bomb"))
                {
                    Bomb bomb = hit.collider.GetComponent<Bomb>();
                    Debug.Log("Bomb hit by bomb.");
                    bomb.Explode();
                    continue;   // Stop after hitting the first object
                }
                else if (hit.collider.CompareTag("BreakableWall"))
                {
                    Destroy(hit.collider.gameObject);
                    continue;   // Stop after hitting the first object
                }
            }
            else
            {
                // Draw debug ray showing no hits
                Debug.DrawLine(transform.position, transform.position + direction * maxExplosionDistance, Color.blue, rayDuration); // Debug ray (optional)
            }
        }
    }
}