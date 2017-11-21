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
    
    void Start()
    {
        CoroutineManager.SetMonoBehaviourInstance(this);
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