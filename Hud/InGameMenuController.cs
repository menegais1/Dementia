using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuController : MonoBehaviour
{
    private List<Note> takenNotes;
    private List<Note> archivedNotes;
    private OptionsMenu optionsMenu;

    private Inventory inventory;
    private Diary diary;
    private Menu menu;

    // Use this for initialization


    private void Awake()
    {
        inventory = GetComponentInChildren<Inventory>();
        diary = GetComponentInChildren<Diary>();
        menu = GetComponentInChildren<Menu>();
    }

    private void OnEnable()
    {
        inventory.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
        diary.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SelectTab(string tab)
    {
        switch (tab)
        {
            case "Inventory":
                inventory.gameObject.SetActive(true);
                menu.gameObject.SetActive(false);
                diary.gameObject.SetActive(false);
                break;
            case "Diary":
                inventory.gameObject.SetActive(false);
                Debug.Log(menu);
                menu.gameObject.SetActive(false);
                diary.gameObject.SetActive(true);
                break;
            case "Menu":
                inventory.gameObject.SetActive(false);
                menu.gameObject.SetActive(true);
                diary.gameObject.SetActive(false);
                break;
        }
    }


    public void OpenCloseInGameMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


    public void BackMenu()
    {
    }
}