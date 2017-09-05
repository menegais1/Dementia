using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager
{

    private static List<CoroutineContainer> runningCoroutines;
    private static CoroutineManager instance;
    private static MonoBehaviour monoBehaviourInstance;

    public static CoroutineManager getInstance()
    {
        if (instance == null)
        {
            new CoroutineManager();
        }

        return instance;

    }

    private CoroutineManager()
    {
        instance = this;
        runningCoroutines = new List<CoroutineContainer>();
    }

    public void setMonoBehaviourInstance(MonoBehaviour instance)
    {
        monoBehaviourInstance = instance;
    }

    public CoroutineContainer insertNewCoroutine(IEnumerator coroutine, string name)
    {

        if (coroutine != null && name != null)
        {
            monoBehaviourInstance.StartCoroutine(coroutine);
            CoroutineContainer coroutineContainer = new CoroutineContainer(name, coroutine, true);
            runningCoroutines.Add(coroutineContainer);
            return coroutineContainer;
        }

        return null;

    }

    public bool deleteCoroutine(string name)
    {
        CoroutineContainer coroutine = findCoroutine(name);
        if (coroutine != null)
        {
            monoBehaviourInstance.StopCoroutine(coroutine.getCoroutine());
            runningCoroutines.Remove(coroutine);
            return true;
        }

        return false;
    }


    public void deleteAllCoroutines()
    {

        monoBehaviourInstance.StopAllCoroutines();
        runningCoroutines = new List<CoroutineContainer>();

    }

    public CoroutineContainer findCoroutine(string name)
    {
        return runningCoroutines.Find(coroutine => coroutine.getName() == name);
    }

    public bool existsCoroutine(string name)
    {
        return runningCoroutines.Exists(coroutine => coroutine.getName() == name);
    }

    public bool coroutineIsRunning(string name)
    {
        if (existsCoroutine(name))
        {
            return runningCoroutines.Find(coroutine => coroutine.getName() == name).getIsRunning();
        }

        return false;
    }

    public List<CoroutineContainer> findAllCoroutines()
    {
        return runningCoroutines;
    }

    public void coroutineStatus(string name)
    {
        if (existsCoroutine(name))
        {
            Debug.Log("A Coroutine Existe e Está: " + ((coroutineIsRunning("teste")) ? "Executando" : "Terminada"));
        }
        else
        {
            Debug.Log("A Coroutine Não Existe");
        }
    }

}
