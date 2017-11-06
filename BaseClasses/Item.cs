using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{

    protected bool worldStatus;
    protected string id;
    protected int quantity;
    
    public abstract void effect();


}
