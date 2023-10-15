using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timePlayed;
    [SerializeField] private TextMeshProUGUI playerName;


    private string saveFilePath;


    private void Start()
    {
        GetComponent<Button>().onClick.AddListener( () =>
        {
            // LOAD THE GAME UNDER THIS CHARACTER SLOT
            StartNetworkAsHost();
            GlobalSaveGameManager.instance.LoadGame( saveFilePath );
        } );
    }

    public void SetSaveFilePath( string saveFilePath )
    {
        this.saveFilePath = saveFilePath;
        playerName.text = saveFilePath;

        //playerName.text = saveFileMetadata.playerName;
        //timePlayed.text = saveFileMetadata.secondsPlayed.ToString(); // TODO: format the time display
    }

    private void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}