using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSaveGameManager : MonoBehaviour
{
    public static GlobalSaveGameManager instance { get; private set; }


    private PlayerManager player;
    private int worldSceneIndex = 1; // TODO implement Loader and enums instead

    [Header( "SAVE/LOAD" )]
    [SerializeField] private bool saveGame;
    [SerializeField] private bool loadGame;

    [Header( "Currently Referenced Save Data" )]
    public string currentSaveFilePath;
    public CharacterSaveData currentCharacterSaveData;

    [Header( "All Save Files Metadata" )]
    private List<string> saveFilePathList;


    private void Awake()
    {
        if ( instance != null )
        {
            Destroy( gameObject );
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }

        LoadSaveFilePathList();
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged( Scene previousScene, Scene newScene )
    {
        // LOAD PLAYER DATA ONLY AFTER THE PLAYER SPAWNS
        if ( newScene == SceneManager.GetSceneByBuildIndex( worldSceneIndex ) )
        {
            Debug.Log( "WORLD SCENE LOADED" );
            player.LoadPlayerDataFrom( currentCharacterSaveData );
        }
    }

    private void Update()
    {
        if ( saveGame )
        {
            saveGame = false;
            SaveGame();
        }

        if ( loadGame )
        {
            loadGame = false;
            LoadGame( currentSaveFilePath );
        }
    }

    private void LoadSaveFilePathList()
    {
        SaveFileDataWriter saveFileDataWriter = new SaveFileDataWriter();
        saveFilePathList = saveFileDataWriter.GetValidSaveFilePathList();
    }

    public void CreateNewGame()
    {
        SaveFileDataWriter saveFileDataWriter = new SaveFileDataWriter();

        // CREATE NEW DATA
        string newPlayerName = "Default Name";
        CharacterSaveData newSaveData = new CharacterSaveData( newPlayerName );

        // CREATE NEW SAVE FILE
        string newSaveFilePath = saveFileDataWriter.CreateNewCharacterSaveFile( newSaveData );
        saveFilePathList.Add( newSaveFilePath );
        saveFileDataWriter.SortFilePathListByLastWriteTimeDescending( saveFilePathList );

        // SET CURRENT REFERENCES
        currentCharacterSaveData = newSaveData;
        currentSaveFilePath = newSaveFilePath;

        // START THE GAME
        StartCoroutine( LoadWorldScene() );
    }

    public void LoadGame( string saveFilePath )
    {
        SaveFileDataWriter saveFileDataWriter = new SaveFileDataWriter();

        // SET CURRENT REFERENCES
        currentSaveFilePath = saveFilePath;
        currentCharacterSaveData = saveFileDataWriter.LoadDataFromFile( saveFilePath );

        // START THE GAME
        StartCoroutine( LoadWorldScene() );
    }

    public void SaveGame()
    {
        SaveFileDataWriter saveFileDataWriter = new SaveFileDataWriter();

        // UPDATE DATA THAT WILL BE STORED WITH CURRENT DATA
        player.SavePlayerDataTo( currentCharacterSaveData );

        // OVERWRITE SAVE FILE
        saveFileDataWriter.SaveDataToFile( currentSaveFilePath, currentCharacterSaveData );
    }

    // LOAD THE WORLD SCENE WITH CURRENTLY REFERENCED SAVE DATA
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync( worldSceneIndex );

        yield return null;
    }



    public void SetPlayer( PlayerManager player )
        => this.player = player;

    public int GetWorldSceneIndex()
        => worldSceneIndex;

    public List<string> GetSaveFilePathList()
        => new List<string>( saveFilePathList );
}