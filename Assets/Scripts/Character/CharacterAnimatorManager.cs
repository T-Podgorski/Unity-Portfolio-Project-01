using Unity.Netcode;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    private CharacterManager characterManager;

    private float horizontal;
    private float vertical;

    protected virtual void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        characterManager.animator.SetFloat( "Horizontal", horizontalValue, 0.1f, Time.deltaTime );
        characterManager.animator.SetFloat( "Vertical", verticalValue, 0.1f, Time.deltaTime );
    }

    public virtual void PlayTargetActionAnimation(
        string targetAnimationName,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false )
    {
        // APPLY ROOT MOTION MEANS TO TRANSLATE THE ANIMATION's BUILT-IN MOVEMENT INTO ACTUAL MOVEMENT ( they probably can't have 'in place' status )
        characterManager.applyRootMotion = applyRootMotion;
        characterManager.animator.CrossFade( targetAnimationName, 0.2f );

        // CAN BE USED TO STOP CHARACTER FROM ATTEMPTING NEW ACTIONS
        // ex. if character takes damage and perform 'got hit' animation, this flag will turn true for stunning attacks
        characterManager.isPerformingAction = isPerformingAction;
        characterManager.canRotate = canRotate;
        characterManager.canMove = canMove;

        // TELL SERVER WE PLAYED AN ANIMATION, AND TO PLAY THAT ANIMATION ON EVERY CLIENT
        characterManager.characterNetworkManager.PlayAnimationServerRpc( NetworkManager.Singleton.LocalClientId, targetAnimationName, applyRootMotion );
    }
}
