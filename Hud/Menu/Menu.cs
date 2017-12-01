using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsMenuButton;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private GameObject menuGroup;
    [SerializeField] private GameObject quitGamePopUp;
    [SerializeField] private Button quitGamePopUpConfirmButton;
    [SerializeField] private Button quitGamePopUpCancelButton;

    void Start()
    {
        optionsMenuButton.onClick.AddListener(OnOptionsMenuButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
        quitGamePopUpConfirmButton.onClick.AddListener(OnquitGamePopUpConfirmButtonClick);
        quitGamePopUpCancelButton.onClick.AddListener(OnquitGamePopUpCancelButtonClick);
    }

    private void OnEnable()
    {
        optionsMenu.gameObject.SetActive(false);
        menuGroup.SetActive(true);
        quitGamePopUp.gameObject.SetActive(false);
    }


    private void OnOptionsMenuButtonClick()
    {
        menuGroup.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    private void OnMainMenuButtonClick()
    {
        quitGamePopUp.gameObject.SetActive(true);
        menuGroup.SetActive(false);
    }

    private void OnquitGamePopUpConfirmButtonClick()
    {
        GameManager.instance.QuitGame();
    }

    private void OnquitGamePopUpCancelButtonClick()
    {
        quitGamePopUp.gameObject.SetActive(false);
        menuGroup.SetActive(true);
    }
}