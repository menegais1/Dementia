using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Weapon currentWeapon;
    private Item currentItem;
    private List<Item> quickSelectionItens;

    private InGameMenuController inGameMenuController;

    private List<ItemSlot> itensSlots;
    private List<WeaponSlot> weaponsSlots;

    private Description description;


    void Start()
    {
        inGameMenuController = GetComponentInParent<InGameMenuController>();
        description = GetComponentInChildren<Description>();
        itensSlots = new List<ItemSlot>();
        weaponsSlots = new List<WeaponSlot>();
        itensSlots.AddRange(GetComponentsInChildren<ItemSlot>());
        weaponsSlots.AddRange(GetComponentsInChildren<WeaponSlot>());
    }

    void Update()
    {
        var itemSelected = itensSlots.Find(lambdaExpression => lambdaExpression.Toggle.isOn);
        if (itemSelected != null)
        {
            description.RenderDescription(itemSelected);
        }
        else
        {
            description.RenderDescription(null);
        }
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

        WeaponSlot weapon = weaponsSlots.Find(lambdaExpression =>
            lambdaExpression.BulletType == item.ItemType);

        if (weapon != null)
        {
            AddAmmo(weapon, item.Quantity);
            return;
        }


        if (inGameMenuController != null)
        {
            AddItem(item);
        }
    }

    public void TakeWeapon(CollectibleWeapon weapon)
    {
        if (weapon == null) return;
        AddWeapon(weapon);
        //currentWeapon = currentWeapon == null ? weapon.WeaponInstance.GetComponent<Weapon>() : currentWeapon;
    }

    public void RemoveItemSelection()
    {
    }

    private void AddItem(CollectibleItem item)
    {
        for (int i = 0; i < itensSlots.Count; i++)
        {
            if (itensSlots[i].Type == ItemType.Nothing)
            {
                itensSlots[i].FillItem(item);
                break;
            }

            if (itensSlots[i].Type == item.ItemType)
            {
                itensSlots[i].Quantity += item.Quantity;
                itensSlots[i].RenderItem();
                break;
            }
        }
    }

    private void AddAmmo(WeaponSlot weapon, int quantity)
    {
        weapon.Ammo += quantity;
        weapon.WeaponInstance.GetComponent<Weapon>().AddAmmo(quantity);
        weapon.RenderWeapon();
    }

    private void AddWeapon(CollectibleWeapon weapon)
    {
        for (int i = 0; i < weaponsSlots.Count; i++)
        {
            if (weaponsSlots[i].Type == WeaponType.Nothing || weaponsSlots[i].Type == weapon.WeaponType)
            {
                weaponsSlots[i].FillWeapon(weapon);
                ItemSlot item = itensSlots.Find(lambdaExpression =>
                    lambdaExpression.Type == weaponsSlots[i].BulletType);

                if (item != null)
                {
                    AddAmmo(weaponsSlots[i], item.Quantity);
                    item.Reset();
                }
                break;
            }
        }
    }
}