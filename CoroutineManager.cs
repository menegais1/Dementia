using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineManager
{
    private static List<CoroutineContainer> runningCoroutines = new List<CoroutineContainer>();
    private static MonoBehaviour monoBehaviourInstance;


    public static void setMonoBehaviourInstance(MonoBehaviour instance)
    {
        monoBehaviourInstance = instance;
    }

    public static CoroutineContainer insertNewCoroutine(IEnumerator coroutine, string name)
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

    public static bool deleteCoroutine(string name)
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


    public static void deleteAllCoroutines()
    {
        monoBehaviourInstance.StopAllCoroutines();
        runningCoroutines = new List<CoroutineContainer>();
    }

    public static CoroutineContainer findCoroutine(string name)
    {
        return runningCoroutines.Find(coroutine => coroutine.getName() == name);
    }

    public static bool existsCoroutine(string name)
    {
        return runningCoroutines.Exists(coroutine => coroutine.getName() == name);
    }

    public static bool coroutineIsRunning(string name)
    {
        if (existsCoroutine(name))
        {
            return runningCoroutines.Find(coroutine => coroutine.getName() == name).getIsRunning();
        }

        return false;
    }

    public static List<CoroutineContainer> findAllCoroutines()
    {
        return runningCoroutines;
    }

    public static void coroutineStatus(string name)
    {
        if (existsCoroutine(name))
        {
            Debug.Log("A Coroutine Existe e Está: " + (coroutineIsRunning(name) ? "Executando" : "Terminada"));
        }
        else
        {
            Debug.Log("A Coroutine Não Existe");
        }
    }
}