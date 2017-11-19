using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject menuGroup;
    [SerializeField] private Button optionsMenuButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button creditsBackButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        optionsMenuButton.onClick.AddListener(OnOptionsMenuButtonClick);
        creditsButton.onClick.AddListener(OnCreditsButtonClick);
        creditsBackButton.onClick.AddListener(OnCreditsBackButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnEnable()
    {
        optionsMenu.gameObject.SetActive(false);
        credits.SetActive(false);
        menuGroup.SetActive(true);
    }


    private void OnOptionsMenuButtonClick()
    {
        menuGroup.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    private void OnCreditsButtonClick()
    {
        menuGroup.SetActive(false);
        credits.SetActive(true);
    }

    private void OnCreditsBackButtonClick()
    {
        menuGroup.SetActive(true);
        credits.SetActive(false);
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }
}