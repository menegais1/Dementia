using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private string mainSceneName;
    private string mainMenuSceneName;

    private SaveData saveData;
    private GameObject gameDataHolder;
    private List<CollectibleItem> initializeItens;
    private List<CollectibleWeapon> initializeWeapons;
    private List<CollectibleNote> initializeNotes;
    private List<Enemy> initializeEnemies;
    private string currentSceneName;

    public static GameManager instance;

    public List<CollectibleItem> InitializeItens
    {
        get { return initializeItens; }
        set { initializeItens = value; }
    }

    public List<CollectibleWeapon> InitializeWeapons
    {
        get { return initializeWeapons; }
        set { initializeWeapons = value; }
    }

    public List<CollectibleNote> InitializeNotes
    {
        get { return initializeNotes; }
        set { initializeNotes = value; }
    }

    public List<Enemy> InitializeEnemies
    {
        get { return initializeEnemies; }
        set { initializeEnemies = value; }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        CoroutineManager.SetMonoBehaviourInstance(this);


        currentSceneName = SceneManager.GetActiveScene().name;


        mainMenuSceneName = "Main Menu";
        mainSceneName = "Prototype";
    }


    public void InitializeGame()
    {
        if (gameDataHolder == null) return;

        InitializeItens = new List<CollectibleItem>();
        InitializeEnemies = new List<Enemy>();
        InitializeNotes = new List<CollectibleNote>();
        InitializeWeapons = new List<CollectibleWeapon>();

        InitializeItens.AddRange(gameDataHolder.GetComponentsInChildren<CollectibleItem>());
        InitializeWeapons.AddRange(gameDataHolder.GetComponentsInChildren<CollectibleWeapon>());
        InitializeNotes.AddRange(gameDataHolder.GetComponentsInChildren<CollectibleNote>());
        InitializeEnemies.AddRange(gameDataHolder.GetComponentsInChildren<Enemy>());

        for (var i = 0; i < InitializeItens.Count; i++)
        {
            InitializeItens[i].Id = GenerateId(i);
        }

        for (var i = 0; i < InitializeWeapons.Count; i++)
        {
            InitializeWeapons[i].Id = GenerateId(i);
        }

        for (var i = 0; i < InitializeNotes.Count; i++)
        {
            InitializeNotes[i].Id = GenerateId(i);
        }

        for (var i = 0; i < InitializeEnemies.Count; i++)
        {
            InitializeEnemies[i].Id = GenerateId(i);
        }
    }


    public void NewGame()
    {
        CoroutineManager.AddCoroutine(LoadSceneAsyncCoroutine(mainSceneName, true), "LoadSceneAsyncCoroutine");
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, bool newGame)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        gameDataHolder = null;

        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (sceneName == mainSceneName)
        {
            gameDataHolder = GameObject.FindGameObjectWithTag("Game Data Holder");
            InitializeGame();

            if (newGame)
                DeleteSaveData();
            else
                LoadData(false);
        }

        currentSceneName = sceneName;

        CoroutineManager.DeleteCoroutine("LoadSceneAsyncCoroutine");
    }

    private IEnumerator ResetCurrentSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        gameDataHolder = null;

        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (sceneName == mainSceneName)
        {
            gameDataHolder = GameObject.FindGameObjectWithTag("Game Data Holder");
            InitializeGame();
            LoadData(false);
        }

        currentSceneName = sceneName;

        CoroutineManager.DeleteCoroutine("ResetCurrentSceneAsyncCoroutine");
    }

    public void ContinueGame()
    {
        CoroutineManager.AddCoroutine(LoadSceneAsyncCoroutine(mainSceneName, false), "LoadSceneAsyncCoroutine");
    }

    public void QuitGame()
    {
        CoroutineManager.AddCoroutine(LoadSceneAsyncCoroutine(mainMenuSceneName, false), "LoadSceneAsyncCoroutine");
    }

    public bool ExistsSave()
    {
        if (Directory.Exists("Saves"))
        {
            return true;
        }
        return false;
    }

    private bool DeleteSaveData()
    {
        saveData = null;
        if (ExistsSave())
        {
            try
            {
                Directory.Delete("Saves", true);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return false;
            }
            return true;
        }
        return true;
    }

    private int GenerateId(int seed)
    {
        return seed;
    }

    public void SaveData()
    {
        if (!Directory.Exists("Saves"))
            Directory.CreateDirectory("Saves");

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream saveFile = File.Create("Saves/save.binary");

        saveData = new SaveData();

        saveData.Save(gameDataHolder);

        formatter.Serialize(saveFile, saveData);

        saveFile.Close();
    }

    public void LoadData(bool resetScene)
    {
        if (ExistsSave())
        {
            if (resetScene)
            {
                CoroutineManager.AddCoroutine(ResetCurrentSceneAsyncCoroutine(currentSceneName),
                    "ResetCurrentSceneAsyncCoroutine");
                return;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream saveFile = File.Open("Saves/save.binary", FileMode.Open);

            saveData = (SaveData) formatter.Deserialize(saveFile);
            saveData.Load(gameDataHolder);

            saveFile.Close();
        }
    }
}