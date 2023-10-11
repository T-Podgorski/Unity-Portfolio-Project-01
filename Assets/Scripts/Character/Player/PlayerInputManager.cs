using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }


    [SerializeField] Vector2 movementInput;


    PlayerInputActions playerInputActions;


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

            playerInputActions.PlayerMovement.Steering.performed += PlayerMovement_Steering_performed;
            playerInputActions.PlayerMovement.Steering.canceled += PlayerMovement_Steering_canceled;
        }
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

        playerInputActions.PlayerMovement.Steering.performed -= PlayerMovement_Steering_performed;
        playerInputActions.PlayerMovement.Steering.canceled -= PlayerMovement_Steering_canceled;

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



    private void PlayerMovement_Steering_canceled( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        movementInput = Vector2.zero;
    }

    private void PlayerMovement_Steering_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        // this value is normalized in PlayerInputActions asset
        movementInput = obj.ReadValue<Vector2>();
    }

    public Vector2 GetMovementInputNormalized()
    {
        Vector2 movementInput = playerInputActions.PlayerMovement.Steering.ReadValue<Vector2>();
        movementInput = movementInput.normalized;

        this.movementInput = movementInput;
        return movementInput;
    }
}