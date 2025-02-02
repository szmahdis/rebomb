using UnityEngine;
using System.Collections;

public class ItemWorld : MonoBehaviour
{

    public AudioClip pickupClip;
    public Item item;

    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }

   public void SetItem(Item item)
    {
        this.item = item;
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void DisableSelf()
    {
        AudioManager.Instance.PlaySoundEffect(pickupClip);
        gameObject.SetActive(false);

    }

    public void Update()
    {
    }
}
