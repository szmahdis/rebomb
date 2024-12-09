using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;

    public Sprite coinSprite;
    public Sprite hourGlassSprite;
    public Sprite fireBombSprite;
    public Sprite bootSprite;
}
