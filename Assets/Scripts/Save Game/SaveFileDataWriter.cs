using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SaveFileDataWriter
{
    private string saveDataDirectoryPath;
    private string saveFileExtension = ".json";


    public SaveFileDataWriter()
    {
        // UNITY RECOMMENDED, UNIVERSAL, PLATFORM-BASED SAVE FILE LOCATION
        saveDataDirectoryPath = Application.persistentDataPath;
    }

    public List<string> GetValidSaveFilePathList()
    {
        List<string> saveFilePathList = new List<string>();

        // FIND ALL FILES IN SAVES DIRECTORY THAT END WITH SAVE FILE EXTENSION
        string fileNameSearchPattern = "*" + saveFileExtension;

        foreach ( string filePath in Directory.GetFiles( saveDataDirectoryPath, fileNameSearchPattern ) )
        {
            try
            {
                string serializedFile = File.ReadAllText( filePath );

                // THIS LINE WILL THROW AN EXCEPTION IF THE FILE IS NOT VALID ( cannot be deserialized into CharacterSaveData )
                CharacterSaveData saveData = JsonUtility.FromJson<CharacterSaveData>( serializedFile );

                saveFilePathList.Add( filePath );
            }
            catch ( Exception ex )
            {
                Debug.LogWarning( $"Error loading file: {filePath}. Exception: {ex}" );
            }
        }

        SortFilePathListByLastWriteTimeDescending( saveFilePathList );
        //Debug.Log( "sorted list:" );
        //for ( int i = 0; i < saveFilePathList.Count; i++ )
        //    Debug.Log( saveFilePathList[ i ] );

        return saveFilePathList;
    }

    public void SortFilePathListByLastWriteTimeDescending( List<string> filePathList )
    {
        filePathList.Sort(( filePath1, filePath2 ) => File.GetLastWriteTime(filePath2).CompareTo( File.GetLastWriteTime(filePath1) ) );
    }

    // LOADS SAVE FILE METADATA
    //public List<SaveFileMetadata> LoadMetadataFromLocalStorage()
    //{
    //    List<SaveFileMetadata> metadataList = new List<SaveFileMetadata>();

    //    // LOAD SERIALIZED METADATA
    //    string serializedSingleMetadata = PlayerPrefs.GetString( PLAYER_PREFS_SERIALIZED_SAVE_FILE_METADATA_LIST_KEY, string.Empty );
    //    Debug.Log( serializedSingleMetadata );

    //    // IF METADATA KEY HAS NOT BEEN CREATED YET
    //    if ( serializedSingleMetadata == string.Empty )
    //    {
    //        Debug.LogWarning( "Loaded Save File Metadata List is Empty." );
    //        return metadataList;
    //    }

    //    // DESERIALIZE INTO A LIST OF METADATA OF EACH SAVE FILE
    //    SaveFileMetadata singleMetadata = JsonUtility.FromJson<SaveFileMetadata>( serializedSingleMetadata );
    //    metadataList.Add( singleMetadata );
    //    Debug.Log( singleMetadata.fileName );
    //    //metadataList = wrappedMetadataList.saveFileMetadataList;

    //    //foreach ( var metadata in metadataList )
    //    //{
    //    //    Debug.Log( "loaded: " + metadata.fileName );
    //    //}

    //    return metadataList;
    //}

    // SAVES SAVE FILE METADATA
    //public void SaveMetadataToLocalStorage( List<SaveFileMetadata> metadataList )
    //{
    //    SaveFileMetadata[] metadataArray = metadataList.ToArray();
    //    Debug.Log( "array: " + metadataArray[ 0 ].fileName );
    //    string serializedSingleMetadata = JsonUtility.ToJson( metadataArray, true );
    //    Debug.Log( metadataArray[ 0 ].fileName );

    //    if ( metadataList == null || metadataList.Count == 0 )
    //    {
    //        Debug.LogWarning( "Saved Save File Metadata List is Empty." );
    //        serializedSingleMetadata = string.Empty;
    //    }

    //    //foreach ( var metadata in saveFileMetadataList )
    //    //{
    //    //    Debug.Log( "saved: " + metadata.fileName );
    //    //}
    //    Debug.Log( serializedSingleMetadata );

    //    PlayerPrefs.SetString( PLAYER_PREFS_SERIALIZED_SAVE_FILE_METADATA_LIST_KEY, serializedSingleMetadata );
    //}

    // OVERWRITE THE FILE IF IT ALREADY EXISTS, OTHERWISE CREATE NEW FILE
    public bool DoesFileExist( string saveFilePath )
    {
        if ( File.Exists( saveFilePath ) )
            return true;
        else
            return false;
    }

    // USED TO DELETE CHARACTER SAVE FILES
    public void DeleteSaveFile( string saveFilePath )
    {
        File.Delete( saveFilePath );
    }

    // USED TO CREATE A NEW GAME SAVE FILE
    public string CreateNewCharacterSaveFile( CharacterSaveData saveData )
    {
        string newSaveFilePath;

        // CREATE NEW SAVE FILE
        string newFileName = GenerateNewSaveFileName();
        newSaveFilePath = Path.Combine( saveDataDirectoryPath, newFileName );

        // SAVE DATA
        SaveDataToFile( newSaveFilePath, saveData );

        return newSaveFilePath;
    }

    // USED TO SAVE DATA TO FILE
    public void SaveDataToFile( string saveFilePath, CharacterSaveData saveData )
    {
        try
        {
            // CREATE OR OVERWRITE THE DIRECTORY THE FILE WILL BE WRITTEN TO
            Directory.CreateDirectory( Path.GetDirectoryName( saveFilePath ) );
            Debug.Log( "CREATING SAVE FILE, AT: " + saveFilePath );

            // SERIALIZE THE SAVEDATA CLASS INTO JSON
            string dataToStore = JsonUtility.ToJson( saveData, true );

            // WRITE THE FILE TO OUR SAVEPATH
            using ( FileStream stream = new FileStream( saveFilePath, FileMode.Create ) )
            {
                using ( StreamWriter fileWriter = new StreamWriter( stream ) )
                {
                    fileWriter.Write( dataToStore );
                }
            }
        }
        catch ( Exception ex )
        {
            Debug.LogError( $"ERROR TRYING TO SAVE CHARACTER DATA AT: {saveFilePath}. EXCEPTION: {ex}" );
        }
    }

    // USED TO LOAD DATA FROM FILE
    public CharacterSaveData LoadDataFromFile( string saveFilePath )
    {
        CharacterSaveData characterData = null;

        if ( File.Exists( saveFilePath ) )
        {
            try
            {
                string serializedCharacterData;

                using ( FileStream stream = new FileStream( saveFilePath, FileMode.Open ) )
                {
                    using ( StreamReader reader = new StreamReader( stream ) )
                    {
                        serializedCharacterData = reader.ReadToEnd();
                    }
                }

                // DESERIALIZE CHARACTER DATA
                characterData = JsonUtility.FromJson<CharacterSaveData>( serializedCharacterData );
            }
            catch ( Exception ex )
            {
                Debug.LogError( $"ERROR LOADING CHARACTER DATA FROM: {saveFilePath}. WRONG METADATA? EXCEPTION THROWN: {ex}" );
            }
        }

        return characterData;
    }

    private string GenerateNewSaveFileName()
    {
        string newFilePath;


        bool duplicateIDFound = false;

        do
        { 
            int newSaveID = UnityEngine.Random.Range( 100000, 1000000 );
            string newFileName = "SaveData_" + newSaveID.ToString() + saveFileExtension;
            newFilePath = Path.Combine( saveDataDirectoryPath, newFileName );

            // TODO: find duplicate names
            //foreach ( string filePath in validSaveFilePathList )
            //{
            //    if ( newFilePath = filePath )
            //    {
            //        duplicateIDFound = true;
            //        break;
            //    }
            //}
        }
        while ( duplicateIDFound );

        return newFilePath;
    }
}


//public struct SaveFileMetadata : IEquatable<SaveFileMetadata>
//{
//    public string fileName;
//    public string playerName;
//    public int secondsPlayed;


//    public SaveFileMetadata( string fileName, string playerName, int secondsPlayed )
//    {
//        this.fileName = fileName;
//        this.playerName = playerName;
//        this.secondsPlayed = secondsPlayed;
//    }

//    public bool Equals( SaveFileMetadata other )
//    {
//        return this.fileName == other.fileName;
//    }
//}