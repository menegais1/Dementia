using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    private class Weapon
    {
        public int id;
        public int ammo;
        public int magazine;

        public Weapon(int id, int ammo, int magazine)
        {
            this.id = id;
            this.ammo = ammo;
            this.magazine = magazine;
        }
    }

    [Serializable]
    private class Note
    {
        public int id;

        public Note(int id)
        {
            this.id = id;
        }
    }

    [Serializable]
    private class Item
    {
        public int id;
        public int quantity;

        public Item(int id, int quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }
    }

    [Serializable]
    private class Enemy
    {
        public int id;

        public Enemy(int id)
        {
            this.id = id;
        }
    }

    private float currentLife;

    private List<Weapon> inventoryWeapons;
    private List<Item> inventoryItens;
    private List<Note> inventoryNotes;

    private List<int> currentEnemiesIdInWorld;
    private List<int> currentItensIdInWorld;
    private List<int> currentWeaponsIdInWorld;
    private List<int> currentNotesIdInWorld;

    private float xPosition;
    private float yPosition;


    public void Save(GameObject gameDataHolder)
    {
        var playerManager = gameDataHolder.GetComponentInChildren<PlayerManager>(true);
        var inGameMenuController = gameDataHolder.GetComponentInChildren<InGameMenuController>(true);

        currentLife = playerManager.PlayerStatusController.CurrentLife;
        xPosition = playerManager.transform.position.x;
        yPosition = playerManager.transform.position.y;
        InitializeInventoryData(inGameMenuController);
        InitializeCurrentWorldData(gameDataHolder);

        Debug.Log("Save");
    }

    public void Load(GameObject gameDataHolder)
    {
        if (gameDataHolder == null) return;
        var playerManager = gameDataHolder.GetComponentInChildren<PlayerManager>(true);
        var inGameMenuController = gameDataHolder.GetComponentInChildren<InGameMenuController>(true);

        playerManager.PlayerStatusController.CurrentLife = currentLife;
        playerManager.transform.position = new Vector3(xPosition, yPosition, playerManager.transform.position.z);

        UpdateItensWorldData(inGameMenuController);
        UpdateWeaponsWorldData(inGameMenuController);
        UpdateNotesWorldData(inGameMenuController);
        UpdateEnemiesWorldData();

        Debug.Log("Load");
    }

    public void UpdateItensWorldData(InGameMenuController inGameMenuController)
    {
        foreach (var item in GameManager.instance.InitializeItens)
        {
            if (item == null)
            {
                continue;
            }

            var inventoryItem = inventoryItens.Find(lamdaExpression => lamdaExpression.id == item.Id);
            if (inventoryItem != null)
            {
                item.Quantity = inventoryItem.quantity;
                inGameMenuController.MenuControllerInventory.TakeItem(item);
                item.DestroyItem();
            }
            else if (!currentItensIdInWorld.Exists(lamdaExpression => lamdaExpression == item.Id))
            {
                item.DestroyItem();
            }
        }
    }

    public void UpdateWeaponsWorldData(InGameMenuController inGameMenuController)
    {
        foreach (var weapon in GameManager.instance.InitializeWeapons)
        {
            if (weapon == null)
            {
                continue;
            }
            var inventoryWeapon = inventoryWeapons.Find(lamdaExpression => lamdaExpression.id == weapon.Id);
            if (inventoryWeapon != null)
            {
                weapon.Ammo = inventoryWeapon.ammo;
                weapon.Magazine = inventoryWeapon.magazine;
                inGameMenuController.MenuControllerInventory.TakeWeapon(weapon);
                weapon.DestroyWeapon();
            }
            else if (!currentWeaponsIdInWorld.Exists(lamdaExpression => lamdaExpression == weapon.Id))
            {
                weapon.DestroyWeapon();
            }
        }
    }

    public void UpdateNotesWorldData(InGameMenuController inGameMenuController)
    {
        foreach (var note in GameManager.instance.InitializeNotes)
        {
            if (note == null)
            {
                continue;
            }
            var inventoryNote = inventoryNotes.Find(lamdaExpression => lamdaExpression.id == note.Id);
            if (inventoryNote != null)
            {
                inGameMenuController.MenuControllerDiary.TakeNote(note);
                note.DestroyNote();
            }
            else if (!currentNotesIdInWorld.Exists(lamdaExpression => lamdaExpression == note.Id))
            {
                note.DestroyNote();
            }
        }
    }

    public void UpdateEnemiesWorldData()
    {
        foreach (var enemy in GameManager.instance.InitializeEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (!currentEnemiesIdInWorld.Exists(lamdaExpression => lamdaExpression == enemy.Id))
            {
                enemy.Die();
            }
        }
    }

    public void InitializeInventoryData(InGameMenuController inGameMenuController)
    {
        inventoryItens = new List<Item>();
        inventoryWeapons = new List<Weapon>();
        inventoryNotes = new List<Note>();

        foreach (var weaponSlot in inGameMenuController.MenuControllerInventory.WeaponsSlots)
        {
            inventoryWeapons.Add(new Weapon(weaponSlot.Id, weaponSlot.Ammo, weaponSlot.Magazine));
        }

        foreach (var itemSlot in inGameMenuController.MenuControllerInventory.ItensSlots)
        {
            inventoryItens.Add(new Item(itemSlot.Id, itemSlot.Quantity));
        }

        foreach (var noteSlot in inGameMenuController.MenuControllerDiary.NotesSlots)
        {
            inventoryNotes.Add(new Note(noteSlot.Id));
        }
    }

    public void InitializeCurrentWorldData(GameObject gameDataHolder)
    {
        if (gameDataHolder == null) return;

        currentEnemiesIdInWorld = new List<int>();
        currentItensIdInWorld = new List<int>();
        currentNotesIdInWorld = new List<int>();
        currentWeaponsIdInWorld = new List<int>();

        foreach (var collectibleItem in gameDataHolder.GetComponentsInChildren<CollectibleItem>(true))
        {
            currentItensIdInWorld.Add(collectibleItem.Id);
        }

        foreach (var collectibleWeapon in gameDataHolder.GetComponentsInChildren<CollectibleWeapon>(true))
        {
            currentWeaponsIdInWorld.Add(collectibleWeapon.Id);
        }

        foreach (var collectibleNote in gameDataHolder.GetComponentsInChildren<CollectibleNote>(true))
        {
            currentNotesIdInWorld.Add(collectibleNote.Id);
        }

        foreach (var enemy in gameDataHolder.GetComponentsInChildren<global::Enemy>(true))
        {
            currentEnemiesIdInWorld.Add(enemy.Id);
        }
    }
}