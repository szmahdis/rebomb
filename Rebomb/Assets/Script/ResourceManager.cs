using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int Steps;
    [SerializeField] private int Bombs;
    
    // TODO: record total coins with later weapon system.
    // public int Coins { get; private set; }

    public void OnRoundStart()
    {
        // Bombs = 0;
        // Steps = 0;
    }

    public void OnTurnStart()
    {
        Steps = 3;
        Bombs = 1;
    }

    public bool OnBombPlaced(BombType type){
        // TODO: integrate with CharacterPlaceBomb.cs
        if (type == BombType.Active || type == BombType.Passive)
        {
            if (Bombs > 0)
            {
                Bombs--;
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
            return true;
        } else {
            return false;
        }
    }
}
