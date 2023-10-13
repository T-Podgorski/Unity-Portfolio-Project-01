using UnityEngine;

public class AnimationResetActionFlag : StateMachineBehaviour
{
    private CharacterManager characterManager;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if( characterManager == null )
        {
            // the animations can see the animator they are a part of, which in turn is on a character
            characterManager = animator.GetComponent<CharacterManager>();
        }

        // SET THIS WHEN AN ACTION ENDS ( THEY ALL RETURN TO THIS 'Empty' STATE ).
        characterManager.isPerformingAction = false;
        characterManager.applyRootMotion = false;
        characterManager.canRotate = true;
        characterManager.canMove = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
