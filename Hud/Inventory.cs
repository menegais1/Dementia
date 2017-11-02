using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Weapon currentWeapon;
    private Item currentItem;
    private List<Weapon> weapons;
    private List<Item> selectionItens;
    [SerializeField] private Diary diary;

    public List<CollectibleItem> Itens { get; set; }


    // Use this for initialization
    void Start()
    {
        Itens = new List<CollectibleItem>();
        weapons = new List<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void selectItem()
    {
    }

    public void selectWeapon()
    {
    }

    public void addItemSelection()
    {
    }

    public void TakeItem(CollectibleItem item)
    {
        if (item == null) return;
        switch (item.ItemType)
        {
            case ItemType.RevolverBullet:
                var weapon = weapons.Find(lambdaExpression => lambdaExpression.Type == WeaponType.Revolver);
                if (weapon != null)
                {
                    weapon.AddAmmo(item.Quantity);
                }
                else
                {
                    Itens.Add(item);
                }
                break;
            case ItemType.Weapon:
                weapons.Add(item.ItemInstance.GetComponent<Weapon>());
                currentWeapon = currentWeapon == null ? item.ItemInstance.GetComponent<Weapon>() : currentWeapon;
                break;
            case ItemType.Bandages:
                Itens.Add(item);
                break;
            case ItemType.Molotov:
                Itens.Add(item);
                break;
            case ItemType.Analgesics:
                Itens.Add(item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Itens.Add(item);

        if (diary != null)
        {
            diary.AddItem(item);
        }
    }

    public void removeItemSelection()
    {
    }
}