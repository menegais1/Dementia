using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text separatorText;

    private int ammo;
    private int weaponTypeId;
    private string name;
    private int magazine;
    private ItemType bulletType;
    private GameObject weaponInstance;
    private WeaponType type;
    private string description;
    private Toggle toggle;
    private bool isEquiped;

    public bool IsEquiped
    {
        get { return isEquiped; }
        set { isEquiped = value; }
    }

    public Text NameText
    {
        get { return nameText; }
        set { nameText = value; }
    }

    public Text AmmoText
    {
        get { return ammoText; }
        set { ammoText = value; }
    }

    public Text SeparatorText
    {
        get { return separatorText; }
        set { separatorText = value; }
    }

    public int Ammo
    {
        get { return ammo; }
        set { ammo = value; }
    }

    public int Magazine
    {
        get { return magazine; }
        set { magazine = value; }
    }

    public ItemType BulletType
    {
        get { return bulletType; }
        set { bulletType = value; }
    }

    public GameObject WeaponInstance
    {
        get { return weaponInstance; }
        set { weaponInstance = value; }
    }

    public WeaponType Type
    {
        get { return type; }
        set { type = value; }
    }

    public Toggle Toggle
    {
        get { return toggle; }
        set { toggle = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int WeaponTypeId
    {
        get { return weaponTypeId; }
    }

    private void Start()
    {
        Type = WeaponType.Nothing;
        nameText.text = "";
        ammoText.text = "";
        separatorText.text = "";
        name = "";
        description = "";
        ammo = 0;
        Toggle = GetComponentInChildren<Toggle>();
        magazine = 0;
        bulletType = ItemType.Nothing;
    }

    public void Reset()
    {
        Type = WeaponType.Nothing;
        nameText.text = "";
        ammoText.text = "";
        separatorText.text = "";
        name = "";
        description = "";
        ammo = 0;
        magazine = 0;
        bulletType = ItemType.Nothing;
        Toggle.isOn = false;
        isEquiped = false;
        weaponInstance = null;
        gameObject.SetActive(false);
    }

    public void RenderWeapon()
    {
        SetActive(true);
        nameText.text = name;
        ammoText.text = magazine + "/" + ammo;
        separatorText.text = "||";
    }

    public void SetActive(bool setActive)
    {
        gameObject.SetActive(setActive);
    }

    public void FillWeapon(CollectibleWeapon weapon)
    {
        name = weapon.Name;
        description = weapon.Description;
        ammo = weapon.Ammo;
        magazine = weapon.Magazine;
        type = weapon.WeaponType;
        bulletType = weapon.BulletType;
        weaponTypeId = weapon.WeaponTypeId;
        weaponInstance = weapon.WeaponInstance;
        RenderWeapon();
    }

    public void Equip(Color color, bool equip)
    {
        nameText.color = color;
        ammoText.color = color;
        separatorText.color = color;
        isEquiped = equip;
    }
}