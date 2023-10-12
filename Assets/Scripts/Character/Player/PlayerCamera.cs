using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance { get; private set; } 


    [SerializeField] private Transform pivotY; // PIVOT THAT CONTROLS CAMERA'S ROTATION ON Y AXIS ( Left & Right )
    [SerializeField] private Transform pivotX; // PIVOT THAT CONTROLS CAMERA'S ROTATION ON X AXIS ( Up & Down )
    public Camera mainCameraObject; // ACTUAL CAMERA. IT ORBITS AROUND THE AXES AND THE DISTANCE FROM PLAYER DEPENDS ON ITS Z POSITION

    public PlayerManager playerManager;

    [Header( "Y Axis Settings" )]
    [SerializeField] private float leftRightRotationSpeed = 220;

    [Header( "X Axis Settings" )]
    [SerializeField] private float upDownRotationSpeed = 220;
    [SerializeField] private float minLookDownAngle = -30;
    [SerializeField] private float maxLookUpAngle = 60;

    [Header( "Collision Settings" )]
    [SerializeField] private float defaultCameraDistanceToPlayer = -3f; // Z = forwards/backwards. USED FOR CAMERA COLLISION
    private float targetCameraDistanceToPlayer; // USED FOR CAMERA COLLISION
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionLayers;

    private float leftRightLookAngle;
    private float upDownLookAngle;
    private float cameraSmoothSpeed = 1; // HIGHER VALUE == LONGER FOR CAMERA TO 'CATCH UP' TO THE PLAYER
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; // Move Camera object to this position after collision. USED FOR CAMERA COLLISION


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

    public void HandleCameraActions()
    {
        if ( playerManager != null )
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        // THIS object follows the Player
        Vector3 targetCameraPosition = Vector3.SmoothDamp( transform.position, playerManager.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime );
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        // TODO: TARGET LOCK ON LOGIC

        // REGULAR LOGIC
        // rotate Left & Right based on horizonal Mouse slide / Gamepad stick input
        leftRightLookAngle += PlayerInputManager.instance.cameraInputX * leftRightRotationSpeed * Time.deltaTime;

        // rotate Up & Down based on vertical Mouse slide / Gamepad stick input.
        upDownLookAngle -= PlayerInputManager.instance.cameraInputY * upDownRotationSpeed * Time.deltaTime; // '-=' because input UP = camera looks DOWN
        upDownLookAngle = Mathf.Clamp( upDownLookAngle, minLookDownAngle, maxLookUpAngle ); // Clamp the view angle between mininum and maximum pivots


        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        // APPLY LEFT/RIGHT ROTATION
        cameraRotation.y = leftRightLookAngle; // looking left and right, we rotate around the vertical axis, which is Y
        targetRotation = Quaternion.Euler( cameraRotation );
        pivotY.localRotation = targetRotation;

        // APPLY UP/DOWN ROTATION
        cameraRotation = Vector3.zero;
        cameraRotation.x = upDownLookAngle;
        targetRotation = Quaternion.Euler( cameraRotation );
        pivotX.localRotation = targetRotation;
    }

    private void HandleCollisions()
    {
        // IF NO COLLISION DETECTED, THIS WILL REMAIN AT DEFAULT
        targetCameraDistanceToPlayer = defaultCameraDistanceToPlayer;

        // DIRECTION FOR COLLISION CHECK
        Vector3 cameraFacingDirection = mainCameraObject.transform.position - pivotX.position;
        cameraFacingDirection.Normalize();

        // CHECK IF THERE IS AN OBJECT IN FRONT OF OUR DESIRED DIRECTION
        if ( Physics.SphereCast( pivotX.position, cameraCollisionRadius, cameraFacingDirection, out RaycastHit hit, Mathf.Abs( defaultCameraDistanceToPlayer ), collisionLayers ) )
        {
            // 
            float distanceFromHitObject = Vector3.Distance( pivotX.position, hit.point );
            targetCameraDistanceToPlayer = -(distanceFromHitObject - cameraCollisionRadius); 
        }

        //if (Mathf.Abs( targetCameraDistanceToPlayer ) < cameraCollisionRadius )
        //    targetCameraDistanceToPlayer = -cameraCollisionRadius;
        

        // APPLY THE FINAL CAMERA DISTANCE TO PLAYER
        float cameraCollisionLerpFactor = 0.2f;
        cameraObjectPosition.z = Mathf.Lerp( mainCameraObject.transform.localPosition.z, targetCameraDistanceToPlayer, cameraCollisionLerpFactor );
        mainCameraObject.transform.localPosition = cameraObjectPosition;
    }

    public Vector3 GetForwardDir()
        => pivotY.transform.forward;

    public Vector3 GetRightSideDir()
        => pivotY.transform.right;
}