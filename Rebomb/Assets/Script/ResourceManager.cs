using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int Coins;
    [SerializeField] private int Steps;

    private List<Item> itemList;

    public event Action<string, int> OnResourceUpdated;
    // string: "coin"/"step";
    // int: updated resource value


    public ResourceManager()
    {
        // Initialize the list in the constructor
        itemList = new List<Item>();
        AddInventoryItem(new Item { itemType = Item.ItemType.Coin, amount = 5});
        Debug.Log("Inventory initialized with " + itemList.Count + " item(s)");
    }

    // Copy constructor
    public ResourceManager(ResourceManager other)
    {
        Coins = other.Coins;
        Steps = other.Steps;
        itemList = new List<Item>(other.itemList);
    }

    public void AddInventoryItem(Item item)
    {
        if (item.IsStackable()) {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList) {
                Debug.Log("Item type in inventory is: " + inventoryItem.itemType);
                Debug.Log("Pickup up in item type is: " + item.itemType);
                if (inventoryItem.itemType == item.itemType) {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory) {
                itemList.Add(item);
            }
        } else {
            itemList.Add(item);
        }
        OnResourceUpdated?.Invoke("coin", GetTotalCoins());
        Debug.Log("Inventory updated with " + itemList.Count + " item(s)");
    }

    public List<Item> GetInventoryItemList()
    {
        return itemList;
    }

    private int GetTotalCoins()
    {
        // Calculate the total number of coins in the inventory
        return itemList
            .Where(item => item.itemType == Item.ItemType.Coin)
            .Sum(item => item.amount);
    }

    public void OnRoundStart()
    {
        // Steps = 0;
        // Coins = 0;
        // OnResourceUpdated?.Invoke("step", Steps);
        // OnResourceUpdated?.Invoke("coin", Coins);
    }

    public void OnTurnStart()
    {
        Steps = 3;
        Coins = GetTotalCoins();
        OnResourceUpdated?.Invoke("step", Steps);
        OnResourceUpdated?.Invoke("coin", Coins);
    }

    public bool OnBombPlaced(BombType type)
    {
        if (type == BombType.Active || type == BombType.Passive)
        {
            if (Coins > 0)
            {
                Coins--;
                OnResourceUpdated?.Invoke("coin", Coins);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public bool OnStepTaken(int step_num = 1)
    {
        if (Steps > 0)
        {
            Steps--;
            OnResourceUpdated?.Invoke("step", Steps);
            return true;
        }
        else
        {
            return false;
        }
    }
}
