using UnityEngine;

public class PlayerHudUIManager : MonoBehaviour
{
    [SerializeField] private StatBarUI staminaBar;


    public void SetNewStaminaValue( float oldValue, float newValue )
    {
        staminaBar.SetStat( Mathf.RoundToInt( newValue ) );
    }

    public void SetMaxStaminaValue( int value )
    {
        staminaBar.SetMaxStat( value );
    }
}
