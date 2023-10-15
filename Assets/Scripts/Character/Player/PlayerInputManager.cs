using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }


    public PlayerManager player;
    private PlayerInputActions playerInputActions;

    [Header( "Player Movement Input" )]
    [SerializeField] private Vector2 movementInput;
    public float movementInputX;
    public float movementInputY;
    public float movementMode;

    [Header( "Camera Rotation Input" )]
    // CAMERA INPUT IS FROM MOUSE SLIDING
    [SerializeField] private Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;
    [SerializeField, Range( 0.1f, 1f )] private float cameraSensitivity = 0.3f;

    [Header( "Player Actions Input" )]
    [SerializeField] private bool dodgeInput = false;
    [SerializeField] private bool sprintInput = false;


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
    }
    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        // This instance is created in MainMenu Scene, we don't want it enabled there.
        //instance.enabled = false;
        gameObject.SetActive( false );
    }

    private void OnApplicationFocus( bool focus )
    {
        if ( enabled )
        {
            if ( focus )
                playerInputActions.Enable();
            else
                playerInputActions.Disable();
        }
    }

    private void OnEnable()
    {
        if ( playerInputActions != null )
            return;

        playerInputActions = new PlayerInputActions();
        // seems like Enable persists through gameobject toggles
        playerInputActions.Enable();

        // PLAYER MOVEMENT
        playerInputActions.PlayerMovement.Gamepad.performed += PlayerMovement_performed;
        playerInputActions.PlayerMovement.Gamepad.canceled += PlayerMovement_canceled;

        playerInputActions.PlayerMovement.Keyboard.performed += PlayerMovement_performed;
        playerInputActions.PlayerMovement.Keyboard.canceled += PlayerMovement_canceled;

        // CAMERA ROTATION
        playerInputActions.PlayerCamera.Gamepad.performed += PlayerCamera_performed;
        playerInputActions.PlayerCamera.Mouse.performed += PlayerCamera_performed;

        // ACTIONS
        playerInputActions.PlayerActions.Dodge.performed += PlayerAction_Dodge_performed;

        playerInputActions.PlayerActions.Sprint.performed += PlayerAction_Sprint_performed;
        playerInputActions.PlayerActions.Sprint.canceled += PlayerAction_Sprint_canceled;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

        // PLAYER MOVEMENT
        playerInputActions.PlayerMovement.Gamepad.performed -= PlayerMovement_performed;
        playerInputActions.PlayerMovement.Gamepad.canceled -= PlayerMovement_canceled;

        playerInputActions.PlayerMovement.Keyboard.performed -= PlayerMovement_performed;
        playerInputActions.PlayerMovement.Keyboard.canceled -= PlayerMovement_canceled;

        // CAMERA ROTATION
        playerInputActions.PlayerCamera.Gamepad.performed -= PlayerCamera_performed;
        playerInputActions.PlayerCamera.Mouse.performed -= PlayerCamera_performed;

        // ACTIONS
        playerInputActions.PlayerActions.Dodge.performed -= PlayerAction_Dodge_performed;

        playerInputActions.PlayerActions.Sprint.performed -= PlayerAction_Sprint_performed;
        playerInputActions.PlayerActions.Sprint.canceled -= PlayerAction_Sprint_canceled;

        playerInputActions.Dispose();
    }

    private void PlayerAction_Sprint_canceled( InputAction.CallbackContext context )
    {
        sprintInput = false;
    }

    private void PlayerAction_Sprint_performed( InputAction.CallbackContext context )
    {
        sprintInput = true;
    }

    private void PlayerAction_Dodge_performed( InputAction.CallbackContext context )
    {
        dodgeInput = true;
    }

    private void PlayerCamera_performed( InputAction.CallbackContext context )
    {
        cameraInput = context.ReadValue<Vector2>() * cameraSensitivity;
    }

    private void PlayerMovement_canceled( InputAction.CallbackContext context )
    {
        // for some reason keyboard doesn't reset to zero if all keys are released
        movementInput = Vector2.zero;
    }

    private void PlayerMovement_performed( InputAction.CallbackContext context )
    {
        // this value is normalized in PlayerInputActions asset
        movementInput = context.ReadValue<Vector2>();
    }

    private void SceneManager_activeSceneChanged( Scene oldScene, Scene newScene )
    {
        // TODO implement Loader with enum for scenes
        // BUG this might not be working as intended due to the nature of events?
        //      (based on observation, neither disabled gameobject, nor instance don't stop the value of movementInput from being updated)
        // ENABLE PLAYER CONTROLS ONLY IN CERTAIN SCENES
        if ( newScene.buildIndex == GlobalSaveGameManager.instance.GetWorldSceneIndex() )
            //instance.enabled = true; // this script itself is enabled, not the entire gameobject
            gameObject.SetActive( true );
        else
            //instance.enabled = false;
            gameObject.SetActive( false );
    }



    private void Update()
    {
        // HACK: THIS CHECK IS BECAUSE THIS UPDATE STARTS RUNNING EARLIER THAN PLAYER AWAKES/SPAWNS
        if ( player == null )
            return;

        HandlePlayerMovementInput();
        HandleCameraRotationInput();
        HandleSprintInput();
        HandleDodgeInput();
    }

    // MOVEMENT

    // TODO for keyboard, ex. hold shift to run. unlike gamepad sticks, keys don't have analog 'input magnitude'
    private void HandlePlayerMovementInput()
    {
        movementInputX = movementInput.x;
        movementInputY = movementInput.y;

        // DETERMINE MOVEMENT MODE (STAND, WALK, RUN) BASED ON STICK INPUT STRENGTH. Clamp is for diagonals.
        movementMode = Mathf.Clamp01( Mathf.Abs( movementInputX ) + Mathf.Abs( movementInputY ) );

        if ( movementMode > 0f && movementMode <= 0.5f )
            movementMode = 0.5f;
        else if ( movementMode > 0.5f && movementMode <= 1f )
            movementMode = 1f;

        if ( player == null )
            return;

        // IF THE CAMERA IS NOT LOCKED ON, PERFORM ONLY NON-STRAFING MOVEMENT ( NO HORIZONTAL VALUES )
        player.playerAnimatorManager.UpdateAnimatorMovementParameters( 0f, movementMode, player.playerNetworkManager.isSprinting.Value );

        // TODO: IF THE CAMERA IS LOCKED ON, ALSO PERFORM STRAFING MOVEMENT ( HORIZONTAL INCLUDED )
    }

    private void HandleCameraRotationInput()
    {
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
    }

    // ACTIONS

    private void HandleSprintInput()
    {
        if ( sprintInput )
        {
            player.playerMovementManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    private void HandleDodgeInput()
    {
        if ( dodgeInput )
        {
            dodgeInput = false;

            // TODO: DO NOTHING IF MENU OR UI WINDOW IS OPEN

            player.playerMovementManager.AttemptToPerformDodge();
        }
    }
}