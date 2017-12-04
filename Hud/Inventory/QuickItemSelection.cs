using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemSelection : MonoBehaviour
{
    [SerializeField] private Button quickItemSelectionPopUpSlot1;
    [SerializeField] private Text quickItemSelectionPopUpSlot1Text;

    [SerializeField] private Button quickItemSelectionPopUpSlot2;
    [SerializeField] private Text quickItemSelectionPopUpSlot2Text;

    [SerializeField] private Button quickItemSelectionPopUpBackButton;

    private ItemSlot currentItem;
    private List<ItemSlot> quickItemSelectionList;

    public ItemSlot CurrentItem
    {
        get { return currentItem; }
        set { currentItem = value; }
    }

    public List<ItemSlot> QuickItemSelectionList
    {
        get { return quickItemSelectionList; }
        set { quickItemSelectionList = value; }
    }

    private void Start()
    {
        quickItemSelectionPopUpSlot1.onClick.AddListener(SelectSlot1);
        quickItemSelectionPopUpSlot2.onClick.AddListener(SelectSlot2);
        quickItemSelectionPopUpBackButton.onClick.AddListener(OnBackButton);
    }

    private void OnEnable()
    {
        if (quickItemSelectionList == null)
        {
            quickItemSelectionList = new List<ItemSlot> {null, null};
        }
    }

    private void Update()
    {
        if (quickItemSelectionList[0] != null && quickItemSelectionList[0].Type == ItemType.Nothing)
        {
            quickItemSelectionPopUpSlot1Text.text = "";
        }

        if (quickItemSelectionList[1] != null && quickItemSelectionList[1].Type == ItemType.Nothing)
        {
            quickItemSelectionPopUpSlot2Text.text = "";
        }
    }

    private void SelectSlot1()
    {
        if (currentItem == null) return;
        if (quickItemSelectionList[0] != null && quickItemSelectionList[0].Type != ItemType.Nothing)
        {
            quickItemSelectionList[0].QuickSelectionSlot = ItemQuickSelectionSlot.None;
        }

        quickItemSelectionList[0] = currentItem;
        quickItemSelectionPopUpSlot1Text.text = currentItem.Name;
        quickItemSelectionList[0].QuickSelectionSlot = ItemQuickSelectionSlot.First;
    }

    private void SelectSlot2()
    {
        if (quickItemSelectionList[1] != null && quickItemSelectionList[1].Type != ItemType.Nothing)
        {
            quickItemSelectionList[1].QuickSelectionSlot = ItemQuickSelectionSlot.None;
        }

        quickItemSelectionList[1] = currentItem;
        quickItemSelectionPopUpSlot2Text.text = currentItem.Name;
        quickItemSelectionList[1].QuickSelectionSlot = ItemQuickSelectionSlot.Second;
    }

    private void OnBackButton()
    {
        currentItem = null;
        this.gameObject.SetActive(false);
    }
}