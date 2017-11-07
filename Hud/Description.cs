using UnityEngine;
using UnityEngine.UI;

public class Description : MonoBehaviour
{
    [SerializeField] private Text descriptionText;
    [SerializeField] private Toggle equip;
    [SerializeField] private Button discard;

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

    private void Start()
    {
        descriptionText.text = "";
        equip.isOn = false;
    }

    public void Reset()
    {
        descriptionText.text = "";
        equip.isOn = false;
    }

    public void RenderDescription(ItemSlot item)
    {
        descriptionText.text = item != null ? item.Description : "";
    }
}