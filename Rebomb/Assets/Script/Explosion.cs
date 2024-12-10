using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    public int explosion_range;
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

    public void play(){
        foreach(Vector3 tile in tiles)
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
        foreach (Vector3 direction in explosion_directions)
        {
            float explosionDrawDistance = 0f;
            // Cast a ray from the bomb's position in the specified direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, explosion_range))
            {
                Debug.DrawLine(transform.position, transform.position + direction * hit.distance, Color.red, rayDuration); // Debug ray (optional)

                // Check if the object is a destructible one
                if (hit.collider.CompareTag("Player"))
                {
                    Player player = hit.collider.GetComponent<Player>();
                    // Debug.Log("Player hit by bomb.");
                    explosionDrawDistance = hit.distance;
                    // Safe bombs don't hurt players
                    if (bomb_type != BombType.SafeBomb)
                    {
                        player.OnKilled();
                    }
                }
                else if (hit.collider.CompareTag("Bomb"))
                {
                    Bomb bomb = hit.collider.GetComponent<Bomb>();
                    // Debug.Log("Bomb hit by bomb.");
                    explosionDrawDistance = hit.distance;
                    if (bomb_type == BombType.ChainBomb)
                    {
                        // TODO(Jialin): Chain bomb cannot increase explosion range of another bomb 
                        // if it was triggered earlier?
                        bomb.explosion.explosion_range++;
                    }
                    bomb.set_triggered();
                }
                else if (hit.collider.CompareTag("BreakableWall"))
                {
                    // TODO(Yaxuan): change the explosion effect on this tile.
                    explosionDrawDistance = hit.distance;
                }
                else // hit a non-destructible object
                {
                    explosionDrawDistance = hit.distance - 1.0f;
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
