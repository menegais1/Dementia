using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private WeaponSlot currentWeapon;
    private ItemSlot currentItem;
    private List<Item> quickSelectionItens;

    private InGameMenuController inGameMenuController;

    private List<ItemSlot> itensSlots;
    private List<WeaponSlot> weaponsSlots;

    private Description description;

    public WeaponSlot CurrentWeapon
    {
        get { return currentWeapon; }
        set { currentWeapon = value; }
    }

    public ItemSlot CurrentItem
    {
        get { return currentItem; }
        set { currentItem = value; }
    }

    public List<ItemSlot> ItensSlots
    {
        get { return itensSlots; }
        set { itensSlots = value; }
    }

    public List<WeaponSlot> WeaponsSlots
    {
        get { return weaponsSlots; }
        set { weaponsSlots = value; }
    }


    void Start()
    {
        inGameMenuController = GetComponentInParent<InGameMenuController>();
        description = GetComponentInChildren<Description>();

        ItensSlots.AddRange(GetComponentsInChildren<ItemSlot>());
        WeaponsSlots.AddRange(GetComponentsInChildren<WeaponSlot>());

        description.gameObject.SetActive(false);

        DisableItemSlots();
        DisableWeaponSlots();
    }

    private void OnEnable()
    {
        if (ItensSlots == null)
        {
            ItensSlots = new List<ItemSlot>();
        }
        if (WeaponsSlots == null)
        {
            WeaponsSlots = new List<WeaponSlot>();
        }

        for (var i = 0; i < ItensSlots.Count; i++)
        {
            if (ItensSlots[i].Type != ItemType.Nothing)
                ItensSlots[i].RenderItem();
        }
        for (var i = 0; i < WeaponsSlots.Count; i++)
        {
            if (WeaponsSlots[i].Type != WeaponType.Nothing)
                WeaponsSlots[i].RenderWeapon();
        }

        if (description != null)
            description.RenderDescription();
    }

    private void DisableWeaponSlots()
    {
        for (var i = 0; i < WeaponsSlots.Count; i++)
        {
            WeaponsSlots[i].SetActive(false);
        }
    }

    private void DisableItemSlots()
    {
        for (var i = 0; i < ItensSlots.Count; i++)
        {
            ItensSlots[i].SetActive(false);
        }
    }

    void Update()
    {
        CheckForSelection();


        if (description.Equip.isOn)
        {
            //Checa se não há outra arma/item equipada, se houver, desequipa-a
            CheckForDifferentEquipedWeapon();
            CheckForDifferentEquipedItem();
        }

        CheckForCurrentItem();
        CheckForCurrentWeapon();
    }

    public void CheckForCurrentItem()
    {
        CurrentItem = ItensSlots.Find(lambdaExpression =>
            lambdaExpression.IsEquiped);
    }

    public void CheckForCurrentWeapon()
    {
        CurrentWeapon = WeaponsSlots.Find(lambdaExpression =>
            lambdaExpression.IsEquiped);
    }

    private void CheckForSelection()
    {
        ItemSlot itemSelected = ItensSlots.Find(lambdaExpression => lambdaExpression.Toggle.isOn);
        WeaponSlot weaponSelected = WeaponsSlots.Find(lambdaExpression => lambdaExpression.Toggle.isOn);


        if (itemSelected != null)
        {
            if (description.ItemSlot != null && itemSelected.Type != description.ItemSlot.Type)
            {
                description.RenderDescription();
            }
            else
            {
                description.RenderDescription(itemSelected);
            }
        }
        else if (weaponSelected != null)
        {
            if (description.WeaponSlot != null && weaponSelected.Type != description.WeaponSlot.Type)
            {
                description.RenderDescription();
            }
            else
            {
                description.RenderDescription(weaponSelected);
            }
        }
        else
        {
            description.RenderDescription();
        }
    }

    private void CheckForDifferentEquipedWeapon()
    {
        if (description.WeaponSlot != null)
        {
            var equipedWeapon = WeaponsSlots.Find(lambdaExpression =>
                lambdaExpression.IsEquiped && lambdaExpression != description.WeaponSlot);

            if (equipedWeapon != null)
            {
                equipedWeapon.Equip(new Color(0.2f, 0.2f, 0.2f), false);
            }
        }
    }

    private void CheckForDifferentEquipedItem()
    {
        if (description.ItemSlot != null)
        {
            var equipedItem = ItensSlots.Find(lambdaExpression =>
                lambdaExpression.IsEquiped && lambdaExpression != description.ItemSlot);
            if (equipedItem != null)
            {
                equipedItem.Equip(new Color(0.2f, 0.2f, 0.2f), false);
            }
        }
    }

    public ItemSlot TakeItem(CollectibleItem item)
    {
        if (item == null) return null;

        WeaponSlot weapon = WeaponsSlots.Find(lambdaExpression =>
            lambdaExpression.BulletType == item.ItemType);

        if (weapon != null)
        {
            AddAmmo(weapon, item.Quantity);
            return null;
        }

        if (item.ItemType == ItemType.Nothing) return null;

        if (item.ItemInstance == null && !item.Unequipable) return null;

        return AddItem(item);
    }

    public WeaponSlot TakeWeapon(CollectibleWeapon weapon)
    {
        if (weapon == null) return null;
        if (weapon.WeaponInstance == null || weapon.WeaponType == WeaponType.Nothing) return null;
        return AddWeapon(weapon);
    }


    private ItemSlot AddItem(CollectibleItem item)
    {
        for (int i = 0; i < ItensSlots.Count; i++)
        {
            if (ItensSlots[i].Type == ItemType.Nothing)
            {
                ItensSlots[i].FillItem(item);
                return ItensSlots[i];
            }

            if (ItensSlots[i].Type == item.ItemType)
            {
                ItensSlots[i].Quantity += item.Quantity;
                ItensSlots[i].RenderItem();
                return ItensSlots[i];
            }
        }
        return null;
    }

    private void AddAmmo(WeaponSlot weapon, int quantity)
    {
        weapon.Ammo += quantity;
        var weaponEquiped = GameObject.FindGameObjectWithTag("Weapon");

        if (weaponEquiped != null)
        {
            weaponEquiped.GetComponent<Weapon>().AddAmmo(quantity);
        }
        weapon.RenderWeapon();
    }

    private WeaponSlot AddWeapon(CollectibleWeapon weapon)
    {
        for (int i = 0; i < WeaponsSlots.Count; i++)
        {
            if (WeaponsSlots[i].Type == WeaponType.Nothing || WeaponsSlots[i].Type == weapon.WeaponType)
            {
                WeaponsSlots[i].FillWeapon(weapon);
                ItemSlot item = ItensSlots.Find(lambdaExpression =>
                    lambdaExpression.Type == WeaponsSlots[i].BulletType);

                if (item != null)
                {
                    AddAmmo(WeaponsSlots[i], item.Quantity);
                    item.Reset();
                }
                return weaponsSlots[i];
            }
        }
        return null;
    }
}