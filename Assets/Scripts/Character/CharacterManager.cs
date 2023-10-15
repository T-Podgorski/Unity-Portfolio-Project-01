using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Common logic for game characters.
/// </summary>
public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;

    [Header( "Flags" )]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;


    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
    }

    protected virtual void Update()
    {
        // LOCAL CLIENT UPDATES NETWORK REFERENCE DATA WITH ITS LOCAL DATA
        if ( IsOwner )
        {
            characterNetworkManager.characterPosition.Value = transform.position;
            characterNetworkManager.characterRotation.Value = transform.rotation;
        }
        // OTHER CONNECTED CLIENTS USE THAT NETWORK DATA TO UPDATE THE SENDING CLIENT's DATA ON THEIR CLIENT
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                characterNetworkManager.characterPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.characterPositionSmoothTime );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                characterNetworkManager.characterRotation.Value,
                characterNetworkManager.characterRotationSmoothTime );
        }
    }

    protected virtual void LateUpdate() { }
}