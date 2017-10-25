using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Comparar duas coroutines nunca retorna falso, necessário refazer a classe


public static class CoroutineManager
{
    private static List<CoroutineContainer> runningCoroutines = new List<CoroutineContainer>();
    private static MonoBehaviour monoBehaviourInstance;


    public static void SetMonoBehaviourInstance(MonoBehaviour instance)
    {
        monoBehaviourInstance = instance;
    }

    public static CoroutineContainer AddCoroutine(IEnumerator coroutine, string name, Object instanceCoroutineWasCalled)
    {
        if (coroutine == null) return null;

        monoBehaviourInstance.StartCoroutine(coroutine);
        var coroutineContainer =
            new CoroutineContainer(name ?? "", coroutine, true, instanceCoroutineWasCalled);
        runningCoroutines.Add(coroutineContainer);
        return coroutineContainer;
    }

    public static CoroutineContainer AddCoroutine(IEnumerator coroutine, Object instanceCoroutineWasCalled)
    {
        return AddCoroutine(coroutine, null, instanceCoroutineWasCalled);
    }

    public static CoroutineContainer AddCoroutine(IEnumerator coroutine, string name)
    {
        return AddCoroutine(coroutine, name, null);
    }

    public static CoroutineContainer AddCoroutine(IEnumerator coroutine)
    {
        return AddCoroutine(coroutine, null, null);
    }


    public static bool DeleteCoroutine(string name, Object instanceCoroutineWasCalled)
    {
        var resultCoroutine = FindCoroutine(name, instanceCoroutineWasCalled);
        if (resultCoroutine == null) return false;

        monoBehaviourInstance.StopCoroutine(resultCoroutine.Coroutine);
        runningCoroutines.Remove(resultCoroutine);
        return true;
    }

    public static bool DeleteCoroutine(IEnumerator coroutine, Object instanceCoroutineWasCalled)
    {
        var resultCoroutine = FindCoroutine(coroutine, instanceCoroutineWasCalled);
        if (resultCoroutine == null) return false;

        monoBehaviourInstance.StopCoroutine(resultCoroutine.Coroutine);
        runningCoroutines.Remove(resultCoroutine);
        return true;
    }

    public static bool DeleteCoroutine(IEnumerator coroutine)
    {
        var resultCoroutine = FindCoroutine(coroutine);
        if (resultCoroutine == null) return false;

        monoBehaviourInstance.StopCoroutine(resultCoroutine.Coroutine);
        runningCoroutines.Remove(resultCoroutine);
        return true;
    }


    public static bool DeleteCoroutine(string name)
    {
        var resultCoroutine = FindCoroutine(name);
        if (resultCoroutine == null) return false;

        monoBehaviourInstance.StopCoroutine(resultCoroutine.Coroutine);
        runningCoroutines.Remove(resultCoroutine);
        return true;
    }


    public static void DeleteAllCoroutines()
    {
        monoBehaviourInstance.StopAllCoroutines();
        runningCoroutines = new List<CoroutineContainer>();
    }

    public static CoroutineContainer FindCoroutine(string name, Object instanceCoroutineWasCalled)
    {
        return runningCoroutines.Find(lambdaCoroutine =>
            lambdaCoroutine.Name == name && lambdaCoroutine.InstanceCoroutineWasCalled == instanceCoroutineWasCalled);
    }

    public static CoroutineContainer FindCoroutine(IEnumerator coroutine, Object instanceCoroutineWasCalled)
    {
        return runningCoroutines.Find(lambdaCoroutine =>
            lambdaCoroutine.Coroutine == coroutine &&
            lambdaCoroutine.InstanceCoroutineWasCalled == instanceCoroutineWasCalled);
    }

    public static CoroutineContainer FindCoroutine(IEnumerator coroutine)
    {
        return runningCoroutines.Find(lambdaCoroutine =>
            lambdaCoroutine.Coroutine == coroutine);
    }

    public static CoroutineContainer FindCoroutine(string name)
    {
        return runningCoroutines.Find(lambdaCoroutine =>
            lambdaCoroutine.Name == name);
    }

    public static bool CheckIfCoroutineExists(string name, Object instanceCoroutineWasCalled)
    {
        return runningCoroutines.Exists(lambdaCoroutine =>
            lambdaCoroutine.Name == name && lambdaCoroutine.InstanceCoroutineWasCalled == instanceCoroutineWasCalled);
    }

    public static bool CheckIfCoroutineExists(IEnumerator coroutine, Object instanceCoroutineWasCalled)
    {
        return runningCoroutines.Exists(lambdaCoroutine =>
            lambdaCoroutine.Coroutine == coroutine &&
            lambdaCoroutine.InstanceCoroutineWasCalled == instanceCoroutineWasCalled);
    }

    public static bool CheckIfCoroutineExists(IEnumerator coroutine)
    {
        return runningCoroutines.Exists(lambdaCoroutine =>
            lambdaCoroutine.Coroutine == coroutine);
    }

    public static bool CheckIfCoroutineExists(string name)
    {
        return runningCoroutines.Exists(lambdaCoroutine =>
            lambdaCoroutine.Name == name);
    }

    public static bool CheckIfCoroutineIsRunning(string name, Object instanceCoroutineWasCalled)
    {
        if (CheckIfCoroutineExists(name, instanceCoroutineWasCalled))
        {
            return runningCoroutines.Find(lambdaCoroutine =>
                lambdaCoroutine.Name == name &&
                lambdaCoroutine.InstanceCoroutineWasCalled == instanceCoroutineWasCalled).IsRunning;
        }

        return false;
    }

    public static bool CheckIfCoroutineIsRunning(IEnumerator coroutine, Object instanceCoroutineWasCalled)
    {
        if (CheckIfCoroutineExists(coroutine, instanceCoroutineWasCalled))
        {
            return runningCoroutines.Find(lambdaCoroutine =>
                lambdaCoroutine.Coroutine == coroutine &&
                lambdaCoroutine.InstanceCoroutineWasCalled == instanceCoroutineWasCalled).IsRunning;
        }

        return false;
    }

    public static bool CheckIfCoroutineIsRunning(IEnumerator coroutine)
    {
        if (CheckIfCoroutineExists(coroutine))
        {
            return runningCoroutines.Find(lambdaCoroutine =>
                lambdaCoroutine.Coroutine == coroutine).IsRunning;
        }

        return false;
    }

    public static bool CheckIfCoroutineIsRunning(string name)
    {
        if (CheckIfCoroutineExists(name))
        {
            return runningCoroutines.Find(lambdaCoroutine =>
                lambdaCoroutine.Name == name).IsRunning;
        }

        return false;
    }


    public static List<CoroutineContainer> FindAllCoroutines()
    {
        return runningCoroutines;
    }

    public static void CoroutineStatus(IEnumerator coroutine)
    {
        if (CheckIfCoroutineExists(coroutine))
        {
            Debug.Log("A Coroutine Existe e Está: " +
                      (CheckIfCoroutineIsRunning(coroutine) ? "Executando" : "Terminada"));
        }
        else
        {
            Debug.Log("A Coroutine Não Existe");
        }
    }
}