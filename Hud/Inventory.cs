using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Weapon currentWeapon;
    private Item currentItem;
    private List<Weapon> inventoryWeapons;
    private List<Item> quickSelectionItens;
    public List<CollectibleItem> InventoryItens { get; set; }

    private InGameMenuController inGameMenuController;

    private ItemSlot[] itensSlots;
    private WeaponSlot[] weaponsSlots;


    void Start()
    {
        inGameMenuController = GetComponentInParent<InGameMenuController>();
        InventoryItens = new List<CollectibleItem>();
        inventoryWeapons = new List<Weapon>();

        itensSlots = GetComponentsInChildren<ItemSlot>();
        weaponsSlots = GetComponentsInChildren<WeaponSlot>();
    }

    void Update()
    {
    }


    public void SelectItem()
    {
    }

    public void SelectWeapon()
    {
    }

    public void AddItemSelection()
    {
    }

    public void TakeItem(CollectibleItem item)
    {
        if (item == null) return;
        Weapon weapon;
        
        switch (item.ItemType)
        {
            case ItemType.RevolverBullet:
                weapon = inventoryWeapons.Find(lambdaExpression => lambdaExpression.Type == WeaponType.Revolver);
                if (weapon != null)
                {
                    weapon.AddAmmo(item.Quantity);
                    RenderWeapon(weapon);
                }
                else
                {
                    InventoryItens.Add(item);
                }
                break;
            case ItemType.Weapon:
                weapon = item.ItemInstance.GetComponent<Weapon>();
                inventoryWeapons.Add(weapon);
                currentWeapon = currentWeapon == null ? item.ItemInstance.GetComponent<Weapon>() : currentWeapon;
                RenderWeapon(weapon);
                return;
            case ItemType.Bandages:
                InventoryItens.Add(item);
                break;
            case ItemType.Molotov:
                InventoryItens.Add(item);
                break;
            case ItemType.Analgesics:
                InventoryItens.Add(item);
                break;
            case ItemType.Nothing:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        if (inGameMenuController != null)
        {
            RenderItem(item);
        }
    }

    public void RemoveItemSelection()
    {
    }

    private void RenderItem(CollectibleItem item)
    {
        for (int i = 0; i < itensSlots.Length; i++)
        {
            if (itensSlots[i].Type == ItemType.Nothing)
            {
                itensSlots[i].Name.text = item.ItemName;
                itensSlots[i].Quantity.text = item.Quantity.ToString();
                itensSlots[i].Type = item.ItemType;
                break;
            }

            if (itensSlots[i].Type == item.ItemType)
            {
                itensSlots[i].Quantity.text = (int.Parse(itensSlots[i].Quantity.text) + item.Quantity).ToString();
                break;
            }
        }
    }

    private void RenderWeapon(Weapon weapon)
    {
        for (int i = 0; i < weaponsSlots.Length; i++)
        {
            if (weaponsSlots[i].Type == WeaponType.Nothing || weaponsSlots[i].Type == weapon.Type)
            {
                weaponsSlots[i].Name.text = weapon.Name;
                weaponsSlots[i].Separator.text = "||";
                weaponsSlots[i].Ammo.text = weapon.CurrentMagazine + "/" + weapon.CurrentAmmo;
                weaponsSlots[i].Type = weapon.Type;
                break;
            }
        }
    }
}