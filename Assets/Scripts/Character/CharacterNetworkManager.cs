using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
    private CharacterManager characterManager;

    [Header( "Transform" )]
    public NetworkVariable<Vector3> characterPosition = new NetworkVariable<Vector3>( Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    public Vector3 networkPositionVelocity;
    public float characterPositionSmoothTime = 0.1f;

    public NetworkVariable<Quaternion> characterRotation = new NetworkVariable<Quaternion>( Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    public float characterRotationSmoothTime = 0.1f;

    [Header( "Animator" )]
    public NetworkVariable<float> animatorHorizontalMovement = new NetworkVariable<float>( 0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    public NetworkVariable<float> animatorVerticalMovement = new NetworkVariable<float>( 0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    public NetworkVariable<float> animatorMovementMode = new NetworkVariable<float>( 0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );


    protected virtual void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    [ServerRpc( RequireOwnership = false )]
    public void PlayAnimationServerRpc( ulong clientID, string animationID, bool applyRootMotion )
    {
        PlayAnimationClientRpc( clientID, animationID, applyRootMotion );
    }

    [ClientRpc]
    public void PlayAnimationClientRpc( ulong clientID, string animationID, bool applyRootMotion )
    {
        // DONT LISTEN IF SENDER
        if ( clientID == NetworkManager.Singleton.LocalClientId )
            return;

        PerformOtherClientsAnimation( animationID, applyRootMotion );
    }

    private void PerformOtherClientsAnimation( string animationID, bool applyRootMotion )
    {
        characterManager.animator.CrossFade( animationID, 0.2f );
    }
}