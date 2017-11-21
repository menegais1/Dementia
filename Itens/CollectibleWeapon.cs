using UnityEngine;

public class CollectibleWeapon : MonoBehaviour
{
    private PlayerStatusVariables playerStatusVariables;

    private int weaponTypeId;
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private int ammo;
    [SerializeField] private int magazine;
    [SerializeField] private ItemType bulletType;
    [SerializeField] private GameObject weaponInstance;
    [SerializeField] private WeaponType weaponType;


    public GameObject WeaponInstance
    {
        get { return weaponInstance; }
    }

    public ItemType BulletType
    {
        get { return bulletType; }
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

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public WeaponType WeaponType
    {
        get { return weaponType; }
        set { weaponType = value; }
    }

    public int WeaponTypeId
    {
        get { return weaponTypeId; }
    }


    private void Start()
    {
        if (weaponInstance != null)
        {
            var tempWeapon = Instantiate(weaponInstance);
            weaponTypeId = tempWeapon.GetComponent<Weapon>().WeaponTypeId;
            Destroy(tempWeapon);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables = other.GetComponent<PlayerManager>().PlayerStatusVariables;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables.canTakeWeapon = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables.canTakeWeapon = false;
    }

    public void DestroyWeapon()
    {
        Destroy(this.gameObject);
    }
}