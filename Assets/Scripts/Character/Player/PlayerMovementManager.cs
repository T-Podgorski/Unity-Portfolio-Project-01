using UnityEngine;

public class PlayerMovementManager : CharacterMovementManager
{
    private PlayerManager player;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float movementMode;

    [Header( "Movement Settings" )]
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float sprintingSpeed = 7f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float sprintingStaminaCost = 2f;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [Header( "Dodge" )]
    [SerializeField] private float dodgeStaminaCost = 25f;
    private Vector3 rollDirection;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if ( player.IsOwner )
        {
            player.characterNetworkManager.animatorHorizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.animatorVerticalMovement.Value = verticalMovement;
            player.characterNetworkManager.animatorMovementMode.Value = movementMode;
        }
        else
        {
            horizontalMovement = player.characterNetworkManager.animatorHorizontalMovement.Value;
            verticalMovement = player.characterNetworkManager.animatorVerticalMovement.Value;
            movementMode = player.characterNetworkManager.animatorMovementMode.Value;

            // IF NOT LOCKED ON, DONT STRAFE
            player.playerAnimatorManager.UpdateAnimatorMovementParameters( 0, movementMode, player.playerNetworkManager.isSprinting.Value );

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
        if ( !player.canMove )
            return;

        GetMovementValuesFromInput();

        // MOVE DIRECTION IS BASED ON THE CAMERA FACING PERSPECTIVE && PLAYER INPUTS
        moveDirection = PlayerCamera.instance.GetForwardDir() * verticalMovement;
        moveDirection += PlayerCamera.instance.GetRightSideDir() * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0f; // we can't move vertically


        if ( player.playerNetworkManager.isSprinting.Value )
        {
            // SPRINT
            player.characterController.Move( moveDirection * sprintingSpeed * Time.deltaTime );
        }
        else
        {
            if ( PlayerInputManager.instance.movementMode > 0.5f )
            {
                // RUN
                player.characterController.Move( moveDirection * runningSpeed * Time.deltaTime );
            }
            else if ( PlayerInputManager.instance.movementMode > 0f )
            {
                // WALK
                player.characterController.Move( moveDirection * walkingSpeed * Time.deltaTime );
            }
        }
    }

    private void HandleRotation()
    {
        if ( !player.canRotate )
            return;

        targetRotationDirection = PlayerCamera.instance.mainCamera.transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.mainCamera.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0f;

        if ( targetRotationDirection == Vector3.zero )
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation( targetRotationDirection );
        Quaternion targetRotation = Quaternion.Slerp( transform.rotation, newRotation, rotationSpeed * Time.deltaTime );
        transform.rotation = targetRotation;
    }

    public void HandleSprinting()
    {
        // IF PERFORMING ACTION, SPRINT = FALSE
        if ( player.isPerformingAction )
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        // IF OUT OF STAMINA, SPRINT = FALSE
        if( player.playerNetworkManager.currentStamina.Value <= 0f )
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        // IF RUNNING, SPRINT = TRUE
        if( movementMode > 0.5f )
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }    
        // IF STATIONARY/WALKING, SPRINT = FALSE
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        // IF SPRINTING, BURN STAMINA
        if(player.playerNetworkManager.isSprinting.Value )
        {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void AttemptToPerformDodge()
    {
        if ( player.isPerformingAction )
            return;

        if ( player.playerNetworkManager.currentStamina.Value <= 0f )
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
            player.transform.rotation = playerRotation;

            // PLAY ROLL ANIMATION
            player.playerAnimatorManager.PlayTargetActionAnimation( "Roll_Forward_01", true, true );
        }
        // PLAYER STATIONARY - BACKSTEP
        else
        {
            // PLAY BACKSTEP ANIMATION
            player.playerAnimatorManager.PlayTargetActionAnimation( "Backstep_01", true, true );
        }

        // CONSUME STAMINA
        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }
}