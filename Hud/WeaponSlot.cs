using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text ammo;
    [SerializeField] private Text separator;
    private WeaponType type;

    public Text Name
    {
        get { return name; }
        set { name = value; }
    }

    public Text Ammo
    {
        get { return ammo; }
        set { ammo = value; }
    }

    public WeaponType Type
    {
        get { return type; }
        set { type = value; }
    }

    public Text Separator
    {
        get { return separator; }
        set { separator = value; }
    }

    private void Start()
    {
        Type = WeaponType.Nothing;
        Name.text = "";
        Ammo.text = "";
        Separator.text = "";
    }
}