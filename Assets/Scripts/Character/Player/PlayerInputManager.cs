using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }


    public PlayerManager playerManager;
    private PlayerInputActions playerInputActions;

    [Header("Player Movement Input")]
    [SerializeField] private Vector2 movementInput;
    public float movementInputX;
    public float movementInputY;
    public float movementMode;

    [Header("Camera Rotation Input")]
    // CAMERA INPUT IS FROM MOUSE SLIDING
    [SerializeField] private Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;
    [SerializeField, Range( 0.1f, 1f )] private float mouseSensitivity = 0.3f;




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

    private void OnEnable()
    {
        if ( playerInputActions == null )
        {
            playerInputActions = new PlayerInputActions();
            // seems like Enable persists through gameobject toggles
            playerInputActions.Enable();

            // PLAYER MOVEMENT
            playerInputActions.PlayerMovement.Gamepad.performed += PlayerMovement_performed;
            playerInputActions.PlayerMovement.Gamepad.canceled += PlayerMovement_canceled;

            playerInputActions.PlayerMovement.Keyboard.performed += PlayerMovement_performed;
            playerInputActions.PlayerMovement.Keyboard.canceled += PlayerMovement_canceled;

            // CAMERA ROTATION
            playerInputActions.PlayerCamera.Gamepad.performed += PlayerCamera_Gamepad_performed;
            playerInputActions.PlayerCamera.Mouse.performed += PlayerCamera_Mouse_performed;
        }
    }

    private void PlayerCamera_Gamepad_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        cameraInput = obj.ReadValue<Vector2>();
    }

    private void PlayerCamera_Mouse_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        cameraInput = obj.ReadValue<Vector2>() * mouseSensitivity;
    }

    private void PlayerMovement_canceled( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        // for some reason keyboard doesn't reset to zero if all keys are released
        movementInput = Vector2.zero;
    }

    private void PlayerMovement_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        // this value is normalized in PlayerInputActions asset
        movementInput = obj.ReadValue<Vector2>();
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        // This instance is created in MainMenu Scene, we don't want it enabled there.
        instance.enabled = false;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

        playerInputActions.PlayerMovement.Gamepad.performed -= PlayerMovement_performed;
        playerInputActions.PlayerMovement.Gamepad.canceled -= PlayerMovement_canceled;

        playerInputActions.PlayerMovement.Keyboard.performed -= PlayerMovement_performed;
        playerInputActions.PlayerMovement.Keyboard.canceled -= PlayerMovement_canceled;

        playerInputActions.Dispose();
    }

    private void SceneManager_activeSceneChanged( Scene oldScene, Scene newScene )
    {
        // TODO implement Loader with enum for scenes
        // BUG this might not be working as intended due to the nature of events?
        //      (based on observation, neither disabled gameobject, nor instance don't stop the value of movementInput from being updated)
        // ENABLE PLAYER CONTROLS ONLY IN CERTAIN SCENES
        if ( newScene.buildIndex == SaveGameManager.instance.GetWorldSceneIndex() )
            instance.enabled = true; // this script itself is enabled, not the entire gameobject
        else
            instance.enabled = false;
    }


    private void OnApplicationFocus( bool focus )
    {
        if( enabled )
        {
            if ( focus )
                playerInputActions.Enable();
            else
                playerInputActions.Disable();
        }
    }

    private void Update()
    {
        HandlePlayerMovementInput();
        HandleCameraRotationInput();
    }


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

        if ( playerManager == null )
            return;
        // IF THE CAMERA IS NOT LOCKED ON, PERFORM ONLY NON-STRAFING MOVEMENT ( NO HORIZONTAL VALUES )
        playerManager.playerAnimatorManager.UpdateAnimatorMovementParameters( 0f, movementMode );

        // TODO: IF THE CAMERA IS LOCKED ON, ALSO PERFORM STRAFING MOVEMENT ( HORIZONTAL INCLUDED )
    }

    private void HandleCameraRotationInput()
    {
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
    }
}