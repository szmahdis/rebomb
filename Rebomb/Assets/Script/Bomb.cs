using UnityEngine;

public enum BombType
{
    Active, Passive
}

public enum BombLevel
{
    // 1 coin, 3 turns(Active), normal active or passive bomb
    NormalBomb,
    // 2 coins, 3 turns, bombs triggered by this bomb will have explosion range +1
    ChainBomb,
    // 2 coins, 3 turns, a safe bomb that doesn't hurt players but can trigger other bombs or destroy walls
    SafeBomb
}

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject VFXExplosionPrefab;
    public BombType bombType;
    public BombLevel bombLevel;
    public int turnsToExplosion = 3; // Time until explosion in turns
    public float maxExplosionDistance = 2f;  // Explosion range
    public bool bombExploded = false;        // Flag to avoid infinite loops

    public Vector3[] explosionDirections = new Vector3[]
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
        float rayDuration = 2f;   // Debug ray duration
        Debug.Log("Bomb exploded!");
        // Play explosion animation at the bomb tile
        PlayVFX(VFXExplosionPrefab, transform.position);
        // Check explosion in all directions
        foreach (Vector3 direction in explosionDirections)
        {
            float explosionDrawDistance = 0f;
            // Cast a ray from the bomb's position in the specified direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxExplosionDistance))
            {
                Debug.DrawLine(transform.position, transform.position + direction * hit.distance, Color.red, rayDuration); // Debug ray (optional)

                // Check if the object is a destructible one
                if (hit.collider.CompareTag("Player")) {
                    Player player = hit.collider.GetComponent<Player>();
                    Debug.Log("Player hit by bomb.");
                    explosionDrawDistance = hit.distance;
                    // Safe bombs don't hurt players
                    if (this.bombLevel != BombLevel.SafeBomb)
                    {
                        player.OnKilled();
                    }
                    
                }
                else if (hit.collider.CompareTag("Bomb"))
                {
                    Bomb bomb = hit.collider.GetComponent<Bomb>();
                    Debug.Log("Bomb hit by bomb.");
                    explosionDrawDistance = hit.distance;
                    if (this.bombLevel == BombLevel.ChainBomb)
                    {
                        bomb.maxExplosionDistance += 1;
                    }
                    bomb.Explode();
                }
                else if (hit.collider.CompareTag("BreakableWall"))
                {
                    Destroy(hit.collider.gameObject);
                    explosionDrawDistance = hit.distance;
                } else {
                    // Ray hit a non-destructible object
                    explosionDrawDistance = hit.distance - 1.0f;
                }
            }
            else
            {
                // Draw debug ray showing no hits
                Debug.DrawLine(transform.position, transform.position + direction * maxExplosionDistance, Color.blue, rayDuration); // Debug ray (optional)
                explosionDrawDistance = maxExplosionDistance;
            }

            // Play bomb animation at every free tile along the ray
            Debug.Log($"Explosion distance: {hit.distance}");
            for (int i = 1; i <= explosionDrawDistance; i++)
            {
                Vector3 tilePosition = transform.position + direction * i;
                PlayVFX(VFXExplosionPrefab, tilePosition);
            }
        }
        // Destroy the bomb itself after exploding
        Destroy(gameObject);
    }

    private void PlayVFX(GameObject vfxPrefab, Vector3 position)
    {
        GameObject instantiatedVFX = Instantiate(vfxPrefab, position, Quaternion.identity);
        float timeToDestroy = 5f;
        foreach (Transform particleEffect in instantiatedVFX.transform)
        {
            // Debug.Log("Playing VFX.");
            particleEffect.GetComponent<ParticleSystem>().Play();
            timeToDestroy = Mathf.Max(timeToDestroy, particleEffect.GetComponent<ParticleSystem>().main.duration);
        }
        // Delete the object after x time to keep Scene clean
        Destroy(instantiatedVFX, timeToDestroy + 1f);
    }
}

[System.Serializable]
public class BombData
{
    public BombType bombType;
    public BombLevel bombLevel;
    public int turnsToExplosion;
    public bool bombExploded;
    public Vector3[] explosionDirections;
    public float maxExplosionDistance = 2f;

    // Position
    public Vector3 position;

    // Constructor to copy data from a Bomb MonoBehaviour
    public BombData(Bomb bomb)
    {
        bombType = bomb.bombType;
        bombLevel = bomb.bombLevel;
        turnsToExplosion = bomb.turnsToExplosion;
        bombExploded = bomb.bombExploded;
        explosionDirections = bomb.explosionDirections;
        maxExplosionDistance = bomb.maxExplosionDistance;
        position = bomb.transform.position;
    }
}