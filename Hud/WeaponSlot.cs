using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text separatorText;
    private int ammo;
    private int magazine;
    private ItemType bulletType;
    private GameObject weaponInstance;

    private WeaponType type;

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

    private void Start()
    {
        Type = WeaponType.Nothing;
        nameText.text = "";
        ammoText.text = "";
        separatorText.text = "";
        name = "";
        ammo = 0;
        magazine = 0;
        bulletType = ItemType.Nothing;
    }

    public void RenderWeapon()
    {
        nameText.text = name;
        ammoText.text = magazine + "/" + ammo;
        separatorText.text = "||";
    }

    public void FillWeapon(CollectibleWeapon weapon)
    {
        name = weapon.Name;
        ammo = weapon.Ammo;
        magazine = weapon.Magazine;
        type = weapon.WeaponType;
        bulletType = weapon.BulletType;
        weaponInstance = weapon.WeaponInstance;
        RenderWeapon();
    }
}