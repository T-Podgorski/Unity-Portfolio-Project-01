using UnityEngine;

public class PlayerMovementManager : CharacterMovementManager
{
    private PlayerManager playerManager;

    public float verticalMovement;
    public float horizontalMovement;
    public float movementMode;

    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;

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
        targetRotationDirection = PlayerCamera.instance.mainCameraObject.transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.mainCameraObject.transform.right * horizontalMovement;
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
}
