using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsMenuButton;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private GameObject menuGroup;

    void Start()
    {
        optionsMenuButton.onClick.AddListener(OnOptionsMenuButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
    }

    private void OnEnable()
    {
        optionsMenu.gameObject.SetActive(false);
        menuGroup.SetActive(true);
    }


    private void OnOptionsMenuButtonClick()
    {
        menuGroup.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    private void OnMainMenuButtonClick()
    {
       GameManager.instance.QuitGame();
    }
}