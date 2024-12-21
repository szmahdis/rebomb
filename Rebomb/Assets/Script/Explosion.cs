using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    public int explosion_range;
    public bool power_up = false;
    public float explosion_time = float.MaxValue;
    public HashSet<Vector3> tiles = new HashSet<Vector3>(new Vector3EqualityComparer());
    private BombType bomb_type;
    public GameObject VFXExplosionPrefab;
    public static float TRIGGER_DELAY = 0.2f;
    public Material particleGreen;
    private static List<Vector3> explosion_directions = new List<Vector3> {
        Vector3.forward,  // Up
        Vector3.back,     // Down
        Vector3.left,     // Left
        Vector3.right     // Right
    };
    public AudioClip explosionClip;

    public void configure_from_type(BombType type)
    {
        bomb_type = type;
        BombConfig cfg = BombConfigurator.Instance.GetConfig(type);
        explosion_range = cfg.explosion_range;
    }

    public void play()
    {
        foreach (Vector3 tile in tiles)
        {
            PlayVFX(VFXExplosionPrefab, tile);
        }
        AudioManager.Instance.PlaySoundEffect(explosionClip);
        Destroy(gameObject);
    }

    public List<Bomb> calculate()
    {
        List<Bomb> cascaded_triggered = new List<Bomb>();

        tiles.Clear();
        // current position.
        tiles.Add(transform.position);

        float rayDuration = 2f;   // Debug ray duration

        // effect of triggered by ChainBomb
        if (power_up) explosion_range += 1;

        // effect of SafeBomb
        if (bomb_type == BombType.SafeBomb)
        {
            foreach (Player player in GameManager.Instance.Players)
                if (player.Alive) player.GetComponent<Collider>().enabled = false;
        }
        foreach (Vector3 direction in explosion_directions)
        {
            float explosionDrawDistance = 0f;
            // Cast a ray from the bomb's position in the specified direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, explosion_range))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Player player = hit.collider.GetComponent<Player>();
                    player.OnKilled();
                }
                else if (hit.collider.CompareTag("Bomb"))
                {
                    Bomb bomb = hit.collider.GetComponent<Bomb>();
                    cascaded_triggered.Add(bomb);
                }
                // else if (hit.collider.CompareTag("BreakableWall"))
                // {}
                // else // hit a non-destructible object, unbreakable wall or boarder
                // {}

                explosionDrawDistance = hit.distance + 0.5f; // 0.5 is half of tile size.
            }
            else
            {
                // Draw debug ray showing no hits
                Debug.DrawLine(transform.position, transform.position + direction * explosion_range, Color.blue, rayDuration); // Debug ray (optional)
                explosionDrawDistance = explosion_range;
            }

            // add tiles on this direction.
            for (int i = 1; i <= explosionDrawDistance; i++)
            {
                Vector3 tile = transform.position + direction * i;
                tiles.Add(tile);
            }
        }

        // end of effect of SafeBomb
        if (bomb_type == BombType.SafeBomb)
        {
            foreach (Player player in GameManager.Instance.Players)
                if (player.Alive) player.GetComponent<Collider>().enabled = true;
        }

        return cascaded_triggered;
    }

    private void PlayVFX(GameObject vfxPrefab, Vector3 tile_position)
    {
        GameObject instantiatedVFX = Instantiate(vfxPrefab, tile_position, Quaternion.identity);

        foreach(ParticleSystem ps in instantiatedVFX.GetComponentsInChildren<ParticleSystem>())
        {
            // decay range based on distance from bomb
            float range_factor = 1.0f - (tile_position - transform.position).magnitude / explosion_range;
            range_factor = range_factor * 0.7f + 0.3f;
            var main = ps.main;
            main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constant * range_factor);

            // adjust color of explosion effect based on bomb type
            if (bomb_type == BombType.SafeBomb)
                ps.GetComponent<Renderer>().material = particleGreen;
            
            ps.Play();
        }
        Destroy(instantiatedVFX, 1.0f);
    }
}
