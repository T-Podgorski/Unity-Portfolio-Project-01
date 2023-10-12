using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button pressStartButton;
    [SerializeField] private GameObject mainMenuGameobject;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;


    private void Awake()
    {
        // TEMP
        pressStartButton.gameObject.SetActive( false );
        mainMenuGameobject.SetActive( true );
        hostButton.Select();

        //pressStartButton.onClick.AddListener( () =>
        //{
        //    StartNetworkAsHost();
        //    pressStartButton.gameObject.SetActive( false );
        //    mainMenuGameobject.SetActive( true );
        //    hostButton.Select();
        //} );

        hostButton.onClick.AddListener( () =>
        {
            StartNetworkAsHost();
            StartNewGame();
        } );

        clientButton.onClick.AddListener( () =>
        {
            StartNetworkAsClient();
            StartNewGame();
        } );
    }

    private void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void StartNetworkAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void StartNewGame()
    {
        StartCoroutine( SaveGameManager.instance.LoadNewGame() );
    }
}