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
        public bool isEquiped;

        public Weapon(int id, int ammo, int magazine, bool isEquiped)
        {
            this.id = id;
            this.ammo = ammo;
            this.magazine = magazine;
            this.isEquiped = isEquiped;
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
        public bool isEquiped;

        public Item(int id, int quantity, bool isEquiped)
        {
            this.id = id;
            this.quantity = quantity;
            this.isEquiped = isEquiped;
        }
    }

    [Serializable]
    private class Enemy
    {
        public int id;
        public float xPosition;
        public float yPosition;

        public Enemy(int id, float xPosition, float yPosition)
        {
            this.id = id;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }
    }

    private float currentLife;

    private List<Weapon> inventoryWeapons;
    private List<Item> inventoryItens;
    private List<Note> inventoryNotes;
    private List<Enemy> currentEnemiesInWorld;

    private List<int> currentItensIdInWorld;
    private List<int> currentWeaponsIdInWorld;
    private List<int> currentNotesIdInWorld;

    private float xPosition;
    private float yPosition;


    public void Save(GameObject gameDataHolder)
    {
        if (gameDataHolder == null) return;
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
                var itemSlot = inGameMenuController.MenuControllerInventory.TakeItem(item);
                var color = inventoryItem.isEquiped ? new Color(0.42f, 0.16f, 0.11f) : new Color(0.2f, 0.2f, 0.2f);
                itemSlot.Equip(color, inventoryItem.isEquiped);
                inGameMenuController.MenuControllerInventory.CheckForCurrentItem();

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
                var weaponSlot = inGameMenuController.MenuControllerInventory.TakeWeapon(weapon);
                var color = inventoryWeapon.isEquiped ? new Color(0.42f, 0.16f, 0.11f) : new Color(0.2f, 0.2f, 0.2f);
                weaponSlot.Equip(color, inventoryWeapon.isEquiped);
                inGameMenuController.MenuControllerInventory.CheckForCurrentWeapon();
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

            var currentEnemy = currentEnemiesInWorld.Find(lamdaExpression => lamdaExpression.id == enemy.Id);
            if (currentEnemy == null)
            {
                enemy.Die();
            }
            else
            {
                enemy.transform.position = new Vector3(currentEnemy.xPosition, currentEnemy.yPosition,
                    enemy.transform.position.z);
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
            inventoryWeapons.Add(new Weapon(weaponSlot.Id, weaponSlot.Ammo, weaponSlot.Magazine, weaponSlot.IsEquiped));
        }

        foreach (var itemSlot in inGameMenuController.MenuControllerInventory.ItensSlots)
        {
            inventoryItens.Add(new Item(itemSlot.Id, itemSlot.Quantity, itemSlot.IsEquiped));
        }

        foreach (var noteSlot in inGameMenuController.MenuControllerDiary.NotesSlots)
        {
            inventoryNotes.Add(new Note(noteSlot.Id));
        }
    }

    public void InitializeCurrentWorldData(GameObject gameDataHolder)
    {
        if (gameDataHolder == null) return;

        currentEnemiesInWorld = new List<Enemy>();

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
            currentEnemiesInWorld.Add(new Enemy(enemy.Id, enemy.transform.position.x, enemy.transform.position.y));
        }
    }
}