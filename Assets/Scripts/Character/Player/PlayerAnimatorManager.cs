using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private PlayerManager playerManager;


    protected override void Awake()
    {
        base.Awake();

        playerManager = GetComponent<PlayerManager>();
    }

    // THIS IS LIKE UPDATE(), BUT FOR ANIMATIONS
    // CALLED AFTER ANIMATION STATE IS EVALUATED, BUT BEFORE IK
    private void OnAnimatorMove()
    {
        // CONTROL ANIMATIONS HERE WITH NUANCE
        // ex. this replaces Animator's applyRootMotion check field, because this script controls it
        if( playerManager.applyRootMotion )
        {
            Vector3 velocity = playerManager.animator.deltaPosition;
            playerManager.characterController.Move( velocity );
            playerManager.transform.rotation *= playerManager.animator.deltaRotation;
        }
    }
}
