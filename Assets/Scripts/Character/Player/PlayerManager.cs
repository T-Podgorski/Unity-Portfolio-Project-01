using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerMovementManager playerMovementManager;


    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad( gameObject );

        playerMovementManager = GetComponent<PlayerMovementManager>();
    }

    protected override void Update()
    {
        base.Update();

        playerMovementManager.HandleMovement();
    }
}