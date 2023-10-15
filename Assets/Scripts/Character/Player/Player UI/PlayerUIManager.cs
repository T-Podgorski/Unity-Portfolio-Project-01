using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance { get; private set; }

    [HideInInspector] public PlayerHudUIManager playerHudUIManager;


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

        playerHudUIManager = GetComponentInChildren<PlayerHudUIManager>();
    }
}