using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : MonoBehaviour
{
    private List<Note> takenNotes;
    private List<Note> archivedNotes;
    private OptionsMenu optionsMenu;

    private InventoryContainer inventoryContainer;

    // Use this for initialization
    void Start()
    {
        inventoryContainer = GetComponentInChildren<InventoryContainer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddItem(CollectibleItem item)
    {
        if (inventoryContainer != null)
        {
            inventoryContainer.AddItem(item);
        }
    }

    public void selectTab()
    {
    }

    public void selectNote()
    {
    }

    public void nextPage()
    {
    }

    public void previousPage()
    {
    }

    public void backMenu()
    {
    }
}