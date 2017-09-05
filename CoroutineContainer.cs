using UnityEngine;
using System.Collections;

public class CoroutineContainer
{

    private string name;
    private IEnumerator coroutine;
    private bool isRunning;

    public CoroutineContainer(string name, IEnumerator coroutine, bool isRunning)
    {
        this.name = name;
        this.coroutine = coroutine;
        this.isRunning = isRunning;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public string getName()
    {
        return this.name;
    }

    public void setIsRunning(bool isRunning)
    {
        this.isRunning = isRunning;
    }

    public bool getIsRunning()
    {
        return this.isRunning;
    }

    public void setCoroutine(IEnumerator coroutine)
    {
        this.coroutine = coroutine;
    }

    public IEnumerator getCoroutine()
    {
        return this.coroutine;
    }
}
