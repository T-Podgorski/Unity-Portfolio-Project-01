using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instance { get; private set; }


    int worldSceneIndex = 1; // TODO implement Loader and enums instead

    private void Awake()
    {
        if ( instance != null )
        {
            Destroy( gameObject );
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad( gameObject );
    }

    public IEnumerator LoadNewGame()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync( worldSceneIndex );

        yield return null;
    }
}
