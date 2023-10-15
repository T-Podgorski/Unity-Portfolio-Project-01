using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if( IsOwner )
        {
            Debug.Log( "player ON SPAWN" );
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            GlobalSaveGameManager.instance.SetPlayer( this );
            Debug.Log( "player set" );
           // LoadPlayerDataFrom( GlobalSaveGameManager.instance.currentCharacterSaveData );

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerHudUIManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            // TODO: THIS WILL BE MOVED WHEN SAVE/LOAD IS ADDED
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalcMaxStaminaBasedOnEnduranceLevel( playerNetworkManager.endurance.Value );
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalcMaxStaminaBasedOnEnduranceLevel( playerNetworkManager.endurance.Value );
            PlayerUIManager.instance.playerHudUIManager.SetMaxStaminaValue( playerNetworkManager.maxStamina.Value );
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerHudUIManager.SetNewStaminaValue;
        playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
    }

    protected override void Awake()
    {
        Debug.Log( "player AWAKE" );
        base.Awake();

        DontDestroyOnLoad( gameObject );

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        if ( !IsOwner )
            return;

        // HANDLE MOVEMENT
        playerMovementManager.HandleMovement();

        // REGEN STAMINA
        playerStatsManager.RegenerateStamina();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        // Unity suggests handling Camera on LateUpdate (?)
        PlayerCamera.instance.HandleCameraActions();
    }

    public void SavePlayerDataTo( CharacterSaveData characterSaveData )
    {
        characterSaveData.playerName = playerNetworkManager.playerName.Value.ToString();
        characterSaveData.posX = transform.position.x;
        characterSaveData.posY = transform.position.y;
        characterSaveData.posZ = transform.position.z;
    }

    public void LoadPlayerDataFrom( CharacterSaveData characterSaveData )
    {
        playerNetworkManager.playerName.Value = characterSaveData.playerName;
        transform.position = new Vector3( characterSaveData.posX, characterSaveData.posY, characterSaveData.posZ );
    }
}