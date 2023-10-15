using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance { get; private set; }

    [Header( "Title Screen" )]
    [SerializeField] private Button pressStartButton;

    [Header( "Main Menu" )]
    [SerializeField] private GameObject mainMenuGameObject;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button loadGameButton;

    [Header( "Load Game Menu" )]
    [SerializeField] private LoadGameMenuUI loadGameMenuUI;

    [Header( "Confirmation Message" )]
    [SerializeField] private GameObject confirmMessageGameObject;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeMessageButton;


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
        }

        // TEMP
        pressStartButton.gameObject.SetActive( false );
        mainMenuGameObject.SetActive( true );
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

        loadGameButton.onClick.AddListener( () =>
        {
            // CLOSE MAIN MENU AND OPEN LOAD GAME MENU
            mainMenuGameObject.SetActive( false );
            loadGameMenuUI.Show();
        } );

        closeMessageButton.onClick.AddListener( () =>
        {
            confirmMessageGameObject.SetActive( false );
        } );
    }

    private void Start()
    {
        confirmMessageGameObject.SetActive( false );
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
        GlobalSaveGameManager.instance.CreateNewGame();
    }

    public void DisplayConfirmMessage( string message )
    {
        confirmMessageGameObject.SetActive( true );
        messageText.text = message;
    }
}