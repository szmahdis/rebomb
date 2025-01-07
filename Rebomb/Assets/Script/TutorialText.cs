using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;

    void Start()
    {
        tutorialText.text =
        "<b><size=38><color=#FFD700>Game Tutorial</color></size></b>\n\n" +

        "<size=32><color=#00BFFF>Resource Reset:</color></size>\n" +
        "<size=28><color=#FFFFFF>Coins and boots reset at the start of each turn.</color></size>\n\n" +

        "<size=32><color=#00BFFF>Special Item: Hourglass</color></size>\n" +
        "<size=28><color=#FFFFFF>• A one-time use special item hidden under a breakable wall.\n" +
        "• After obtaining it, click on the <b>Hourglass Icon</b> to travel back in time by <color=#00FF00><b>2 rounds</b></color>.</color></size>\n\n" +

        "<size=32><color=#00BFFF>Bomb Types:</color></size>\n" +
        "<size=28><color=#FFFFFF>• <b>Active Bomb:</b> Costs <color=#FFD700>1 coin</color>. Explodes after <color=#00FF00><b>3 turns</b></color>.\n" +
        "• <b>Passive Bomb:</b> Costs <color=#FFD700>1 coin</color>. Explodes only when triggered by another bomb.\n" +
        "• <b>ChainBomb:</b> Costs <color=#FFD700>2 coins</color>. Explodes in <color=#00FF00><b>3 turns</b></color> and increases triggered bombs' range by <b>+1</b>.\n" +
        "• <b>SafeBomb:</b> Costs <color=#FFD700>2 coins</color>. Explodes in <color=#00FF00><b>3 turns</b></color>, does not harm the player who placed it, but can trigger other bombs or destroy walls.</color></size>\n\n" +

        "<size=32><color=#00BFFF>Game Objective:</color></size>\n" +
        "<size=28><color=#FFFFFF>Use your resources wisely and try to eliminate the other player strategically!</color></size>";
    }
}
