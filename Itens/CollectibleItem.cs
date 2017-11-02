using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private PlayerStatusVariables playerStatusVariables;

    [SerializeField] private string itemName;
    [SerializeField] private int quantity;
    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject itemInstance;

    public string ItemName
    {
        get { return itemName; }
    }

    public int Quantity
    {
        get { return quantity; }
    }

    public GameObject ItemInstance
    {
        get { return itemInstance; }
    }

    public ItemType ItemType
    {
        get { return itemType; }
    }


    //    private bool worldStatus;
    //    protected string id;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables = other.GetComponent<PlayerMovement>().PlayerStatusVariables;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables.canTakeItem = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables.canTakeItem = false;
    }

    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }
}