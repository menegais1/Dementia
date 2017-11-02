using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text quantity;
    private ItemType type;

    public Text Name
    {
        get { return name; }
        set { name = value; }
    }

    public Text Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }

    public ItemType Type
    {
        get { return type; }
        set { type = value; }
    }

    private void Start()
    {
        Type = ItemType.Nothing;
        Name.text = "";
        Quantity.text = "";
    }
}