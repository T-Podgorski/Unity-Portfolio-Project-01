using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    private CharacterManager character;

    [Header( "Stamina Regen" )]
    [SerializeField] float staminaRegenDelay = 2f;
    [SerializeField] float staminaRegenAmount = 2f;
    private float staminaRegenTimer = 0;
    private float staminaTickTimer = 0;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public int CalcMaxStaminaBasedOnEnduranceLevel( int endurance )
    {
        float stamina;

        stamina = endurance * 10;

        return Mathf.RoundToInt( stamina );
    }

    public virtual void RegenerateStamina()
    {
        // ONLY REGENERATE OWN STAMINA
        if ( !character.IsOwner )
            return;

        // DON'T REGENERATE IF IT'S BEING USED
        if ( character.characterNetworkManager.isSprinting.Value )
            return;

        if ( character.isPerformingAction )
            return;

        staminaRegenTimer += Time.deltaTime;

        if ( staminaRegenTimer >= staminaRegenDelay )
        {
            if ( character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value )
            {
                staminaTickTimer += Time.deltaTime;

                if ( staminaTickTimer >= 0.1f )
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer( float staminaBeforeChange, float staminaAfterChange )
    {
        // ONLY RESET IF THE VALUE CHANGE WAS CAUSED BY AN ACTION CONSUMING STAMINA
        // otherwise, it would reset on every regen tick as well
        if( staminaAfterChange < staminaBeforeChange )
            staminaRegenTimer = 0f;
    }
}
