using UnityEngine;
using UnityEngine.UI;

public class Description : MonoBehaviour
{
    [SerializeField] private Text descriptionText;
    [SerializeField] private Toggle equip;
    [SerializeField] private Button discard;
    [SerializeField] private GameObject discardPopUp;
    [SerializeField] private Button discardAcceptPopUp;
    [SerializeField] private Button discardDeclinePopUp;
    [SerializeField] private Button discardPopUpBackgroundButton;

    [SerializeField] private Button quickItemSelectionButton;
    [SerializeField] private QuickItemSelection quickItemSelectionPopUp;


    private Text equipLabel;

    private ItemSlot itemSlot;
    private WeaponSlot weaponSlot;


    public ItemSlot ItemSlot
    {
        get { return itemSlot; }
        set { itemSlot = value; }
    }

    public WeaponSlot WeaponSlot
    {
        get { return weaponSlot; }
        set { weaponSlot = value; }
    }

    public Text DescriptionText
    {
        get { return descriptionText; }
        set { descriptionText = value; }
    }

    public Toggle Equip
    {
        get { return equip; }
        set { equip = value; }
    }

    public Button Discard
    {
        get { return discard; }
        set { discard = value; }
    }

    public GameObject DiscardPopUp
    {
        get { return discardPopUp; }
        set { discardPopUp = value; }
    }

    public QuickItemSelection QuickItemSelectionPopUp
    {
        get { return quickItemSelectionPopUp; }
        set { quickItemSelectionPopUp = value; }
    }

    private void Start()
    {
        descriptionText.text = "";
        equip.isOn = false;
        equipLabel = equip.gameObject.GetComponentInChildren<Text>();

        equip.onValueChanged.AddListener(OnEquip);
        discard.onClick.AddListener(OpenDiscardPopUp);
        discardAcceptPopUp.onClick.AddListener(OnDiscard);
        discardDeclinePopUp.onClick.AddListener(CloseDiscardPopUp);
        discardPopUpBackgroundButton.onClick.AddListener(CloseDiscardPopUp);
        
        quickItemSelectionButton.onClick.AddListener(OpenQuickItemSelectionPopUp);

        discardPopUp.gameObject.SetActive(false);
        QuickItemSelectionPopUp.gameObject.SetActive(false);
        quickItemSelectionButton.gameObject.SetActive(false);
    }

    private void OnEquip(bool equip)
    {
        equipLabel.text = equip ? "Equipado" : "Não Equipado";
        var color = equip ? new Color(0.42f, 0.16f, 0.11f) : new Color(0.2f, 0.2f, 0.2f);
        if (weaponSlot != null)
        {
            weaponSlot.Equip(color, equip);
        }
        else if (itemSlot != null)
        {
            itemSlot.Equip(color, equip);
        }
    }

    private void OpenDiscardPopUp()
    {
        discardPopUp.gameObject.SetActive(true);
    }

    private void OpenQuickItemSelectionPopUp()
    {
        QuickItemSelectionPopUp.gameObject.SetActive(true);
        QuickItemSelectionPopUp.CurrentItem = itemSlot;
    }

    private void CloseDiscardPopUp()
    {
        discardPopUp.gameObject.SetActive(false);
    }

    private void OnDiscard()
    {
        if (itemSlot != null)
        {
            itemSlot.Reset();
        }
        else if (weaponSlot != null)
        {
            weaponSlot.Reset();
        }
        CloseDiscardPopUp();
    }

    public void RenderDescription(ItemSlot item)
    {
        if (item != null)
        {
            gameObject.SetActive(true);
            descriptionText.text = item.Description;

            weaponSlot = null;
            itemSlot = item;

            if (item.Unequipable)
            {
                equip.isOn = false;
                equip.gameObject.SetActive(false);
            }
            else
            {
                equip.gameObject.SetActive(true);
                quickItemSelectionButton.gameObject.SetActive(true);
                equip.isOn = item.IsEquiped;
            }
        }
    }

    public void RenderDescription(WeaponSlot weapon)
    {
        if (weapon != null)
        {
            gameObject.SetActive(true);
            quickItemSelectionButton.gameObject.SetActive(false);
            descriptionText.text = weapon.Description;

            weaponSlot = weapon;
            itemSlot = null;

            equip.isOn = weapon.IsEquiped;
        }
    }

    public void RenderDescription()
    {
        gameObject.SetActive(false);
        DiscardPopUp.gameObject.SetActive(false);
        equip.gameObject.SetActive(true);
        quickItemSelectionButton.gameObject.SetActive(false);
        QuickItemSelectionPopUp.gameObject.SetActive(false);

        descriptionText.text = "";

        weaponSlot = null;
        itemSlot = null;
        equip.isOn = false;
    }
}