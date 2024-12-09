using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        FireBomb,
        Hourglass,
        Coin,
        // step per turn +1
        Boot,
    }

    public ItemType itemType;
    public int amount;

    // TODO: Find and set sprites/3d models
    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.FireBomb: return ItemAssets.Instance.fireBombSprite;
            case ItemType.Hourglass: return ItemAssets.Instance.hourGlassSprite;
            case ItemType.Coin: return ItemAssets.Instance.coinSprite;
            case ItemType.Boot: return ItemAssets.Instance.bootSprite;
        }
    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Coin:
            case ItemType.FireBomb:
            case ItemType.Boot:
                return true;
            case ItemType.Hourglass:
                return false;
        }
    }

}
