using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text quantityText;

    private int quantity;
    private string name;
    private string description;
    private GameObject itemInstance;
    private ItemType type;
    private Toggle toggle;

    public Text NameText
    {
        get { return nameText; }
        set { nameText = value; }
    }

    public Text QuantityText
    {
        get { return quantityText; }
        set { quantityText = value; }
    }

    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public GameObject ItemInstance
    {
        get { return itemInstance; }
        set { itemInstance = value; }
    }

    public ItemType Type
    {
        get { return type; }
        set { type = value; }
    }

    public Toggle Toggle
    {
        get { return toggle; }
        set { toggle = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }


    private void Start()
    {
        Type = ItemType.Nothing;
        nameText.text = "";
        quantityText.text = "";
        name = "";
        quantity = 0;
        Toggle = GetComponentInChildren<Toggle>();
        Toggle.isOn = false;
    }

    public void Reset()
    {
        Type = ItemType.Nothing;
        nameText.text = "";
        quantityText.text = "";
        name = "";
        quantity = 0;
        itemInstance = null;
        Toggle.isOn = false;
    }

    public void RenderItem()
    {
        nameText.text = name;
        quantityText.text = quantity.ToString();
    }

    public void FillItem(CollectibleItem item)
    {
        name = item.Name;
        Description = item.Description;
        quantity = item.Quantity;
        type = item.ItemType;
        itemInstance = item.ItemInstance;
        RenderItem();
    }
}