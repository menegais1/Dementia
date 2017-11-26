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
    private bool isEquiped;
    private bool unequipable;

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

    public bool IsEquiped
    {
        get { return isEquiped; }
        set { isEquiped = value; }
    }

    public bool Unequipable
    {
        get { return unequipable; }
        set { unequipable = value; }
    }


    private void Start()
    {
        Type = ItemType.Nothing;
        nameText.text = "";
        quantityText.text = "";
        name = "";
        description = "";
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
        description = "";
        quantity = 0;
        itemInstance = null;
        Toggle.isOn = false;
        isEquiped = false;
        gameObject.SetActive(false);
    }

    public void RenderItem()
    {
        gameObject.SetActive(true);
        nameText.text = name;
        quantityText.text = quantity.ToString();
    }

    public void SetActive(bool setActive)
    {
        gameObject.SetActive(setActive);
    }

    public void FillItem(CollectibleItem item)
    {
        name = item.Name;
        Description = item.Description;
        quantity = item.Quantity;
        type = item.ItemType;
        Unequipable = item.Unequipable;
        itemInstance = item.ItemInstance;
        RenderItem();
    }

    public void Equip(Color color, bool equip)
    {
        if (Unequipable) return;

        nameText.color = color;
        quantityText.color = color;
        isEquiped = equip;
    }

    public void UseItem()
    {
        --quantity;
        if (quantity <= 0)
        {
            Reset();
        }
    }
}