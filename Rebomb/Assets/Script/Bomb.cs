using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject VFXExplosionPrefab;
    public BombType bombType;
    public Explosion explosion;
    public int turnsToExplosion = 3;
    public float maxExplosionDistance = 2f;
    public bool bombExploded = false;
    public bool is_triggered = false; // triggered in the current turn
    
    public void configure_from_type(BombType type)
    {
        bombType = type;
        BombConfig config = BombConfigurator.Instance.GetConfig(type);
        turnsToExplosion = config.explosion_turns;
        bombExploded = false;
        maxExplosionDistance = config.explosion_range;
        explosion = gameObject.GetComponent<Explosion>();
        explosion.configure_from_type(type);
    }

    public void configure_from_data(BombData bomb_data){
        bombType = bomb_data.bombType;
        turnsToExplosion = bomb_data.turnsToExplosion;
        bombExploded = bomb_data.bombExploded;
        maxExplosionDistance = bomb_data.maxExplosionDistance;
        explosion = gameObject.GetComponent<Explosion>();
        explosion.configure_from_type(bombType);
    }

    public void BombCountdown()
    {
        if (bombType == BombType.Passive) return;
        turnsToExplosion--;
        if (turnsToExplosion <= 0) {
            set_triggered();
        }
    }

    public void set_triggered()
    {
        if (is_triggered) {
            return;
        }
        is_triggered = true;
        explosion.calculate();
    }
}

[System.Serializable]
public class BombData
{
    public BombType bombType;
    public int turnsToExplosion;
    public float maxExplosionDistance = 2f;
    public bool bombExploded = false;

    // Position
    public Vector3 position;

    // Constructor to copy data from a Bomb MonoBehaviour
    public BombData(Bomb bomb)
    {
        bombType = bomb.bombType;
        turnsToExplosion = bomb.turnsToExplosion;
        maxExplosionDistance = bomb.maxExplosionDistance;
        position = bomb.transform.position;
        bombExploded = bomb.bombExploded;
    }
}