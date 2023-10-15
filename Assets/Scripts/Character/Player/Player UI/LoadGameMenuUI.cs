using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenuUI : MonoBehaviour
{
    [SerializeField] private Transform saveSlotPrefab;
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private Button returnButton;

    [Header( "Other Menu Refs" )]
    [SerializeField] private GameObject mainMenuGameObject;


    private void Start()
    {
        Hide();
    }

    private void UpdateVisuals()
    {
        // DELETE EXISTING SLOTS
        foreach ( Transform child in saveSlotContainer )
        {
            if ( child == saveSlotContainer )
                continue;

            Destroy( child.gameObject );
        }

        // INSTANTIATE ACCORDING TO CURRENT DATA
        List<string> saveFilePathList = GlobalSaveGameManager.instance.GetSaveFilePathList();

        foreach ( var filePath in saveFilePathList )
        {
            Transform saveSlot = Instantiate( saveSlotPrefab, saveSlotContainer );
            saveSlot.gameObject.GetComponent<CharacterSaveSlotUI>().SetSaveFilePath( filePath );
        }
    }

    public void Show()
    {
        gameObject.SetActive( true );
        UpdateVisuals();
    }

    public void Hide()
    {
        gameObject.SetActive( false );
    }
}