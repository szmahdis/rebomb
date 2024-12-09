using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int Coins;
    [SerializeField] private int Steps;
    [SerializeField] private int Hourglass;

    private List<Item> itemList;

    public event Action<string, int> OnResourceUpdated;
    // string: "coin"/"step";
    // int: updated resource value


    public ResourceManager()
    {
        // Initialize the list in the constructor
        itemList = new List<Item>();
        AddInventoryItem(new Item { itemType = Item.ItemType.Coin, amount = 5 });
        Debug.Log("Inventory initialized with " + itemList.Count + " item(s)");
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

        RefreshInventory();
        Debug.Log("Inventory updated with " + itemList.Count + " item(s)");
    }

    public List<Item> GetInventoryItemList()
    {
        return itemList;
    }

    public List<Item> SetInventoryItemList(List<Item> items)
    {
        itemList = items;
        return itemList;
    }

    public void SetCoins(int coins)
    {
        Coins = coins;
    }

    public int GetCoins()
    {
        return Coins;
    }

    private int GetTotalCoins()
    {
        // Calculate the total number of coins in the inventory
        return itemList
            .Where(item => item.itemType == Item.ItemType.Coin)
            .Sum(item => item.amount);
    }

    public int GetTotalBoots()
    {
        // Calculate the total number of coins in the inventory
        return itemList
            .Where(item => item.itemType == Item.ItemType.Boot)
            .Sum(item => item.amount);
    }

    private bool ContainsHourGlass(List<Item> itemList)
    {
        return itemList.Any(item => item.itemType == Item.ItemType.Hourglass);
    }

    private void RefreshInventory()
    {
        
        Coins = GetTotalCoins();
        
        OnResourceUpdated?.Invoke("coin", Coins);
        OnResourceUpdated?.Invoke("step", Steps);

        Hourglass = ContainsHourGlass(itemList) ? 1 : 0;
        Debug.Log("Has hourglass? " + ContainsHourGlass(itemList));
        OnResourceUpdated?.Invoke("hourglass", Hourglass);

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
        Steps = 20;
        Steps += GetTotalBoots();
        OnResourceUpdated?.Invoke("step", Steps);
        RefreshInventory();

    }

    public bool OnBombPlaced(BombLevel bombLevel)
    {
        if (bombLevel == BombLevel.NormalBomb)
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
        else if (bombLevel == BombLevel.ChainBomb)
        {
            if (Coins > 1)
            {
                Coins -= 2;
                OnResourceUpdated?.Invoke("coin", Coins);
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (bombLevel == BombLevel.SafeBomb)
        {
            if (Coins > 1)
            {
                Coins -= 2;
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

public class ResourceInfo
{
    public List<Item> Inventory;
    public ResourceInfo(ResourceManager resourceManager)
    {
        Inventory = new List<Item>();
        Inventory.Add(new Item { itemType = Item.ItemType.Coin, amount = resourceManager.GetCoins() });
        Inventory.Add(new Item { itemType = Item.ItemType.Boot, amount = resourceManager.GetTotalBoots() });
    }
}