using UnityEngine;

public class PlayerMovementManager : CharacterMovementManager
{
    private PlayerManager playerManager;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float movementMode;

    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [Header( "Dodge" )]
    private Vector3 rollDirection;

    protected override void Awake()
    {
        base.Awake();

        playerManager = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if ( playerManager.IsOwner )
        {
            playerManager.characterNetworkManager.animatorHorizontalMovement.Value = horizontalMovement;
            playerManager.characterNetworkManager.animatorVerticalMovement.Value = verticalMovement;
            playerManager.characterNetworkManager.animatorMovementMode.Value = movementMode;
        }
        else
        {
            horizontalMovement = playerManager.characterNetworkManager.animatorHorizontalMovement.Value;
            verticalMovement = playerManager.characterNetworkManager.animatorVerticalMovement.Value;
            movementMode = playerManager.characterNetworkManager.animatorMovementMode.Value;

            // IF NOT LOCKED ON, DONT STRAFE
            playerManager.playerAnimatorManager.UpdateAnimatorMovementParameters( 0, movementMode );

            // TODO: IF LOCKED ON, STRAFE
        }
    }

    public void HandleMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        // AERIAL MOVEMENT
    }

    private void GetMovementValuesFromInput()
    {
        verticalMovement = PlayerInputManager.instance.movementInputY;
        horizontalMovement = PlayerInputManager.instance.movementInputX;
        movementMode = PlayerInputManager.instance.movementMode;
        // CLAMP THE MOVEMENTS
    }

    private void HandleGroundedMovement()
    {
        if ( !playerManager.canMove )
            return;

        GetMovementValuesFromInput();

        // MOVE DIRECTION IS BASED ON THE CAMERA FACING PERSPECTIVE && PLAYER INPUTS
        moveDirection = PlayerCamera.instance.GetForwardDir() * verticalMovement;
        moveDirection += PlayerCamera.instance.GetRightSideDir() * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0f; // we can't move vertically

        if( PlayerInputManager.instance.movementMode > 0.5f )
        {
            // RUN
            playerManager.CharacterController().Move( moveDirection * runningSpeed * Time.deltaTime );
        }
        else if ( PlayerInputManager.instance.movementMode > 0f )
        {
            // WALK
            playerManager.CharacterController().Move( moveDirection * walkingSpeed * Time.deltaTime );
        }
    }

    private void HandleRotation()
    {
        if ( !playerManager.canRotate )
            return;

        targetRotationDirection = PlayerCamera.instance.mainCamera.transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.mainCamera.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0f;

        if( targetRotationDirection == Vector3.zero )
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation( targetRotationDirection );
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime );
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge()
    {
        if ( playerManager.isPerformingAction )
            return;

        // PLAYER MOVING - ROLL
        if ( PlayerInputManager.instance.movementMode > 0f )
        {
            // DETERMINE ROLL DIRECTION BASED ON MOVEMENT INPUTS
            rollDirection = PlayerCamera.instance.mainCamera.transform.forward * PlayerInputManager.instance.movementInputY;
            rollDirection += PlayerCamera.instance.mainCamera.transform.right * PlayerInputManager.instance.movementInputX;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation( rollDirection );
            playerManager.transform.rotation = playerRotation;

            // PLAY ROLL ANIMATION
            playerManager.playerAnimatorManager.PlayTargetActionAnimation( "Roll_Forward_01", true, true );
        }
        // PLAYER STATIONARY - BACKSTEP
        else
        {
            playerManager.playerAnimatorManager.PlayTargetActionAnimation( "Backstep_01", true, true );
        }
    }
}
