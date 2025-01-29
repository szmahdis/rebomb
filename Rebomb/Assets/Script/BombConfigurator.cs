using UnityEngine;
using System.Collections.Generic;

public enum BombType
{
    Active,
    Passive,
    ChainBomb,
    SafeBomb
};

public class BombConfig
{
    public BombType bombType;
    public int price_in_coins = 1;
    public int explosion_turns = 3;
    public int explosion_range = 2;
    public GameObject bomb_prefab;
    
    public BombConfig(BombType type, int price, int turns, int range, GameObject prefab)
    {
        bombType = type;
        price_in_coins = price;
        explosion_turns = turns;
        explosion_range = range;
        bomb_prefab = prefab;
    }
};

public class BombConfigurator : MonoBehaviour
{
    public static BombConfigurator Instance { get; private set; }
    [Header("Bomb Prefabs")]
    [SerializeField] public GameObject ActiveBombPrefab;
    [SerializeField] public GameObject PassiveBombPrefab;
    [SerializeField] public GameObject ChainBombPrefab;
    [SerializeField] public GameObject SafeBombPrefab;
    [SerializeField] public GameObject DefaultBombPrefab;

    [Header("Bomb Configurations")]
    public Dictionary<BombType, BombConfig> bomb_configs;
    
    public BombConfig GetConfig(BombType type)
    {
        return bomb_configs[type];
    }

    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize();
    }

    public int GetPrice(BombType type)
    {
        return bomb_configs[type].price_in_coins;
    }

    public GameObject GetPrefab(BombType type)
    {
        return bomb_configs[type].bomb_prefab;
    }

    private void Initialize()
    {
        BombConfig active_bomb = new BombConfig(
            BombType.Active, 2, 3, 2, ActiveBombPrefab
        );

        // only triggered by other bombs
        BombConfig passive_bomb = new BombConfig(
            BombType.Passive, 1, 3, 2, PassiveBombPrefab
        );
        
        // bombs triggered by this bomb will have explosion range +1
        BombConfig chain_bomb = new BombConfig(
            BombType.ChainBomb, 2, 3, 2, ChainBombPrefab
        );

        // a safe bomb that doesn't hurt players but can trigger other bombs or destroy walls
        BombConfig safe_bomb = new BombConfig(
            BombType.SafeBomb, 1, 3, 2, SafeBombPrefab
        );

        bomb_configs = new Dictionary<BombType, BombConfig>
        {
            { BombType.Active, active_bomb },
            { BombType.Passive, passive_bomb },
            { BombType.ChainBomb, chain_bomb },
            { BombType.SafeBomb, safe_bomb }
        };
    }
}
