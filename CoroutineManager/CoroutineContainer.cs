using UnityEngine;
using System.Collections;

public class CoroutineContainer
{
    public string Name { get; set; }

    public IEnumerator Coroutine { get; set; }

    public bool IsRunning { get; set; }

    public Object InstanceCoroutineWasCalled { get; set; }

    public CoroutineContainer(string name, IEnumerator coroutine, bool isRunning, Object instanceCoroutineWasCalled)
    {
        this.Name = name;
        this.Coroutine = coroutine;
        this.IsRunning = isRunning;
        this.InstanceCoroutineWasCalled = instanceCoroutineWasCalled;
    }
}