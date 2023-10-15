using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private PlayerManager player;


    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    // THIS IS LIKE UPDATE(), BUT FOR ANIMATIONS
    // CALLED AFTER ANIMATION STATE IS EVALUATED, BUT BEFORE IK
    private void OnAnimatorMove()
    {
        // CONTROL ANIMATIONS HERE WITH NUANCE
        // ex. this replaces Animator's applyRootMotion check field, because this script controls it
        if( player.applyRootMotion )
        {
            Vector3 velocity = player.animator.deltaPosition;
            player.characterController.Move( velocity );
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }
}
