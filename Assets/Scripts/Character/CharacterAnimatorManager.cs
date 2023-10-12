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
}
