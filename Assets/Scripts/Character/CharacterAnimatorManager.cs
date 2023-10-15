using Unity.Netcode;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    private CharacterManager character;

    private int horizontalBlendHash;
    private int verticalBlendHash;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        horizontalBlendHash = Animator.StringToHash( "Horizontal" );
        verticalBlendHash = Animator.StringToHash( "Vertical" );
    }

    public void UpdateAnimatorMovementParameters( float horizontalValue, float verticalValue, bool isSprinting )
    {
        if ( isSprinting )
        {
            verticalValue = 2;
        }

        character.animator.SetFloat( horizontalBlendHash, horizontalValue, 0.1f, Time.deltaTime );
        character.animator.SetFloat( verticalBlendHash, verticalValue, 0.1f, Time.deltaTime );
    }

    public virtual void PlayTargetActionAnimation(
        string targetAnimationName,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false )
    {
        // APPLY ROOT MOTION MEANS TO TRANSLATE THE ANIMATION's BUILT-IN MOVEMENT INTO ACTUAL MOVEMENT ( they probably can't have 'in place' status )
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade( targetAnimationName, 0.2f );

        // CAN BE USED TO STOP CHARACTER FROM ATTEMPTING NEW ACTIONS
        // ex. if character takes damage and perform 'got hit' animation, this flag will turn true for stunning attacks
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        // TELL SERVER WE PLAYED AN ANIMATION, AND TO PLAY THAT ANIMATION ON EVERY CLIENT
        character.characterNetworkManager.PlayAnimationServerRpc( NetworkManager.Singleton.LocalClientId, targetAnimationName, applyRootMotion );
    }
}