using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static int INIT_COIN_LIMIT = 5;
    public static int INIT_STEP_LIMIT = 5;
    [SerializeField] private int Coins;
    [SerializeField] private int Steps;
    [SerializeField] private int Hourglass;
    [SerializeField] private int CoinLimit = INIT_COIN_LIMIT;
    [SerializeField] private int StepLimit = INIT_STEP_LIMIT;
    public event Action<string, int> OnResourceUpdated;
    // string: "coin"/"step"/"hourglass";
    // int: updated resource value

    public void AddInventoryItem(Item item)
    {
        Debug.Log("Pickup up item type is: " + item.itemType + " and amount is: " + item.amount);
        switch (item.itemType)
        {
            case Item.ItemType.Coin:
                {
                    Coins += item.amount;
                    OnResourceUpdated?.Invoke("coin", Coins);
                    break;
                }
            case Item.ItemType.Boot:
                {
                    StepLimit += 1;
                    Steps = StepLimit;
                    OnResourceUpdated?.Invoke("step", Steps);
                    break;
                }
            case Item.ItemType.Hourglass:
                {
                    Hourglass += item.amount;
                    OnResourceUpdated?.Invoke("hourglass", Hourglass);
                    break;
                }
        }
    }

    public void SetCoins(int coins)
    {
        Coins = coins;
        OnResourceUpdated?.Invoke("coin", Coins);
    }

    public void SetSteps(int steps)
    {
        Steps = steps;
        OnResourceUpdated?.Invoke("step", Steps);
    }

    public int GetCoins()
    {
        return Coins;
    }

    public int GetSteps()
    {
        return Steps;
    }

    public void OnRoundStart()
    {
        CoinLimit = INIT_COIN_LIMIT;
        StepLimit = INIT_STEP_LIMIT;
        Steps = 0;
        Coins = 0;
        Hourglass = 0;
        OnResourceUpdated?.Invoke("step", Steps);
        OnResourceUpdated?.Invoke("coin", Coins);
        OnResourceUpdated?.Invoke("hourglass", Hourglass);
    }

    public void OnTurnStart()
    {
        Coins = CoinLimit;
        Steps = StepLimit;
        OnResourceUpdated?.Invoke("step", Steps);
        OnResourceUpdated?.Invoke("coin", Coins);
    }

    public bool OnBombPlaced(BombType type)
    {
        int price = BombConfigurator.Instance.GetPrice(type);
        if (Coins >= price)
        {
            Coins -= price;
            OnResourceUpdated?.Invoke("coin", Coins);
            return true;
        }
        else
        {
            return false;
        }
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

    public void consumeHourglass()
    {
        if (Hourglass > 0)
        {
            Debug.Log("Time travel triggered through TurnManager!");
            TurnManager.Instance.TimeTravelTriggered = true;
            Hourglass--;
            OnResourceUpdated?.Invoke("hourglass", Hourglass);
        }
    }
}

public class ResourceInfo
{
    public int coins;
    public int steps;
    public ResourceInfo(ResourceManager resourceManager)
    {
        coins = resourceManager.GetCoins();
        steps = resourceManager.GetSteps();
    }
}