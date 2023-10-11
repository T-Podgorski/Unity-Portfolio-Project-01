using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance { get; private set; }


    [Header( "NETWORK JOIN" )]
    [SerializeField] private bool startGameAsClient;


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

        DontDestroyOnLoad( gameObject );
    }

    private void Update()
    {
        // DEBUG
        if( startGameAsClient )
        {
            startGameAsClient = false;
            // Game starts as a Host, so we need to first shut that down, then start as Client
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.StartClient();
        }
    }
}
