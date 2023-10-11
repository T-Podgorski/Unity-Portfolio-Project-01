using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button pressStartButton;


    private void Awake()
    {
        pressStartButton.onClick.AddListener( () =>
        {
            StartNetworkAsHost();
        } );
    }


    public void StartNetworkAsHost()
    {
        Debug.Log( "starthost called!" );
        NetworkManager.Singleton.StartHost();
    }
}
