using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button pressStartButton;
    [SerializeField] private GameObject mainMenuGameobject;
    [SerializeField] private Button newGameButton;


    private void Awake()
    {
        pressStartButton.onClick.AddListener( () =>
        {
            StartNetworkAsHost();
            pressStartButton.gameObject.SetActive( false );
            mainMenuGameobject.SetActive( true );
            newGameButton.Select();
        } );

        newGameButton.onClick.AddListener( () =>
        {
            StartNewGame();
        } );
    }

    private void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void StartNewGame()
    {
        StartCoroutine( SaveGameManager.instance.LoadNewGame() );
    }
}