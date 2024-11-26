using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int Coins;
    [SerializeField] private int Steps;

    public event Action<string, int> OnResourceUpdated;
    // string: "coin"/"step";
    // int: updated resource value

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
        Coins = 1;
        OnResourceUpdated?.Invoke("step", Steps);
        OnResourceUpdated?.Invoke("coin", Coins);
    }

    public bool OnBombPlaced(BombType type){
        // TODO: integrate with CharacterPlaceBomb.cs
        if (type == BombType.Active || type == BombType.Passive)
        {
            if (Coins > 0)
            {
                Coins--;
                OnResourceUpdated?.Invoke("coin", Coins);
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    public bool OnStepTaken(){
        // TODO: integrate with CharacterMovement.cs
        if (Steps > 0)
        {
            Steps--;
            OnResourceUpdated?.Invoke("step", Steps);
            return true;
        } else {
            return false;
        }
    }
}
