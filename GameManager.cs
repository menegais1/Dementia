using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Váriaveis Gerais

    private SaveData saveData;
    private List<Item> initializeItens;
    private List<Weapon> initializeWeapons;
    private List<Note> initializeNotes;
    private List<Enemy> initializeEnemies;
    private CoroutineManager coroutineManager;
    #endregion

    #region Váriaveis Adicionadas


    #endregion



    #region Métodos Unity


    // Use this for initialization
    void Start()
    {
        coroutineManager = CoroutineManager.getInstance();
        coroutineManager.setMonoBehaviourInstance(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region Métodos Gerais

    public void save()
    {

    }

    public void load()
    {

    }

    public void initializeGame()
    {

    }

    public void idGenerator()
    {

    }


    #endregion
}
