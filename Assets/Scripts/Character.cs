using UnityEngine;

public class Character : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad( gameObject );
    }
}
