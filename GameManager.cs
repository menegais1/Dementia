using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SaveData saveData;
    private List<Item> initializeItens;
    private List<Weapon> initializeWeapons;
    private List<Note> initializeNotes;
    private List<Enemy> initializeEnemies;

    public static GameManager instance;

    void Start()
    {
        CoroutineManager.SetMonoBehaviourInstance(this);
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Save()
    {
    }

    public void Load()
    {
    }

    public void InitializeGame()
    {
    }

    public void IdGenerator()
    {
    }
}