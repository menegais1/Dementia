using UnityEngine;
using UnityEngine.UI;

public class InventoryContainer : MonoBehaviour
{
    private ItemSlot[] itens;


    private void Start()
    {
        itens = GetComponentsInChildren<ItemSlot>();
    }

    public void AddItem(CollectibleItem item)
    {
        for (int i = 0; i < itens.Length; i++)
        {
            if (itens[i].Type == ItemType.Nothing)
            {
                itens[i].Name.text = item.ItemName;
                itens[i].Quantity.text = item.Quantity.ToString();
                itens[i].Type = item.ItemType;
                break;
            }
           
            if (itens[i].Type == item.ItemType)
            {
                itens[i].Quantity.text = (int.Parse(itens[i].Quantity.text) + item.Quantity).ToString();
                break;
            }
        }
    }
}