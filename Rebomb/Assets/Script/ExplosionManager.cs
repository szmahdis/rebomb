using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance { get; private set; }
    private List<Explosion> explosionList = new List<Explosion>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterExplosion(Explosion explosion)
    {
        explosionList.Add(explosion);
    }

    // Trigger all explosions with their delays
    public void Play()
    {
        StartCoroutine(PlayExplosionAtExplosionTime());
    }

    private IEnumerator PlayExplosionAtExplosionTime()
    {
        explosionList.Sort((a, b) => a.explosion_time.CompareTo(b.explosion_time));
        float current_delay = 0f;
        foreach (var explosion in explosionList)
        {
            float delay = explosion.explosion_time - current_delay;
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            explosion.play();
            current_delay = explosion.explosion_time;
        }
        explosionList.Clear();
    }

}
