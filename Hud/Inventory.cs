using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Váriaveis Gerais

    private Weapon currentWeapon;
    private Item currentItem;
    private List<Weapon> weapons;
    private List<CollectibleItem> itens;
    private List<Item> selectionItens;

    #endregion


    #region Métodos Unity

    // Use this for initialization
    void Start()
    {
        itens = new List<CollectibleItem>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    #endregion

    #region Métodos Gerais

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
        itens.Add(item);
    }

    public void removeItemSelection()
    {
    }

    #endregion
}