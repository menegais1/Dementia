using UnityEngine;

public class Item : MonoBehaviour
{
    protected bool worldStatus;
    protected string id;
    protected int quantity;
    [SerializeField] private ItemType type;

    public ItemType Type
    {
        get { return type; }
        set { type = value; }
    }

    public virtual void Effect()
    {
    }
}