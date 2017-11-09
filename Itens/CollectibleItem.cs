using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private PlayerStatusVariables playerStatusVariables;

    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private int quantity;
    [SerializeField] private bool unequipable;
    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject itemInstance;

    public string Name
    {
        get { return name; }
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

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public bool Unequipable
    {
        get { return unequipable; }
        set { unequipable = value; }
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