using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuController : MonoBehaviour
{
    private OptionsMenu optionsMenu;

    public Inventory MenuControllerInventory { get; private set; }
    public Diary MenuControllerDiary { get; private set; }
    public Menu MenuControllerMenu { get; private set; }


    private void Awake()
    {
        MenuControllerInventory = GetComponentInChildren<Inventory>();
        MenuControllerDiary = GetComponentInChildren<Diary>();
        MenuControllerMenu = GetComponentInChildren<Menu>();
    }

    private void OnEnable()
    {
        MenuControllerInventory.gameObject.SetActive(true);
        MenuControllerMenu.gameObject.SetActive(false);
        MenuControllerDiary.gameObject.SetActive(false);
    }


    public void SelectTab(string tab)
    {
        switch (tab)
        {
            case "Inventory":
                MenuControllerInventory.gameObject.SetActive(true);
                MenuControllerMenu.gameObject.SetActive(false);
                MenuControllerDiary.gameObject.SetActive(false);
                break;
            case "Diary":
                MenuControllerInventory.gameObject.SetActive(false);
                Debug.Log(MenuControllerMenu);
                MenuControllerMenu.gameObject.SetActive(false);
                MenuControllerDiary.gameObject.SetActive(true);
                break;
            case "Menu":
                MenuControllerInventory.gameObject.SetActive(false);
                MenuControllerMenu.gameObject.SetActive(true);
                MenuControllerDiary.gameObject.SetActive(false);
                break;
        }
    }


    public void OpenCloseInGameMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}