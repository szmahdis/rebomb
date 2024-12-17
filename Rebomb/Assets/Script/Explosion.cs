using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    static float TRIGGER_DELAY = 0.1f;
    public int explosion_range;
    public bool power_up = false;
    public float explosion_time = float.MaxValue;
    public HashSet<Vector3> tiles = new HashSet<Vector3>(new Vector3EqualityComparer());
    private BombType bomb_type;
    public GameObject VFXExplosionPrefab;
    private static List<Vector3> explosion_directions = new List<Vector3> {
        Vector3.forward,  // Up
        Vector3.back,     // Down
        Vector3.left,     // Left
        Vector3.right     // Right
    };

    public void configure_from_type(BombType type)
    {
        bomb_type = type;
        BombConfig cfg = BombConfigurator.Instance.GetConfig(type);
        explosion_range = cfg.explosion_range;
        // TODO(Yaxuan): configure explosion effect by bomb type
    }

    public void play()
    {
        foreach (Vector3 tile in tiles)
        {
            PlayVFX(VFXExplosionPrefab, tile);
        }
        // Destroy the bomb itself after exploding
        Destroy(gameObject);
    }

    public void calculate()
    {
        tiles.Clear();
        // current position.
        tiles.Add(transform.position);

        float rayDuration = 2f;   // Debug ray duration
        float power_up_range = power_up ? 1.0f : 0.0f;
        foreach (Vector3 direction in explosion_directions)
        {
            float explosionDrawDistance = 0f;
            // Cast a ray from the bomb's position in the specified direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, explosion_range + power_up_range))
            {
                Debug.DrawLine(transform.position, transform.position + direction * hit.distance, Color.red, rayDuration); // Debug ray (optional)

                // Check if the object is a destructible one
                if (hit.collider.CompareTag("Player"))
                {
                    Player player = hit.collider.GetComponent<Player>();
                    explosionDrawDistance = hit.distance + 0.25f; // add half of player collider width;
                    // Safe bombs don't hurt players
                    if (bomb_type != BombType.SafeBomb)
                    {
                        player.OnKilled();
                    }
                }
                else if (hit.collider.CompareTag("Bomb"))
                {
                    Bomb bomb = hit.collider.GetComponent<Bomb>();
                    explosionDrawDistance = hit.distance;
                    bomb.trigger(explosion_time + TRIGGER_DELAY, power_up = (bomb_type == BombType.ChainBomb));
                }
                else if (hit.collider.CompareTag("BreakableWall"))
                {
                    // TODO(Yaxuan): change the explosion effect on this tile.
                    // add half of wall collider width, which is half of tile width;
                    explosionDrawDistance = hit.distance + 0.5f;

                }
                else // hit a non-destructible object
                {
                    // add half of wall collider width, which is half of tile width;
                    explosionDrawDistance = hit.distance + 0.5f;
                }
            }
            else
            {
                // Draw debug ray showing no hits
                Debug.DrawLine(transform.position, transform.position + direction * explosion_range, Color.blue, rayDuration); // Debug ray (optional)
                explosionDrawDistance = explosion_range;
            }

            // add tiles on this direction.
            // Debug.Log($"Explosion distance: {hit.distance}");
            for (int i = 1; i <= explosionDrawDistance; i++)
            {
                Vector3 tile = transform.position + direction * i;
                tiles.Add(tile);
            }
        }
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
