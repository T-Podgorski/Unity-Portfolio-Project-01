using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;


    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad( gameObject );

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        if ( !IsOwner )
            return;

        playerMovementManager.HandleMovement();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        // unity suggests handling camera on LateUpdate (?)
        PlayerCamera.instance.HandleCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if( IsOwner )
        {
            PlayerCamera.instance.playerManager = this;
            PlayerInputManager.instance.playerManager = this;
        }
    }
}