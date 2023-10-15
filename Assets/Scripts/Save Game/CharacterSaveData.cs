using System;
using UnityEngine;

[Serializable]
public class CharacterSaveData
{
    public CharacterSaveData( string playerName )
    {
        this.playerName = playerName;
    }


    [Header( "Character Name" )]
    public string playerName = "DEFAULT";

    [Header( "Time Played" )]
    public float secondsPlayed;

    [Header( "World Transform" )]
    public float posX;
    public float posY;
    public float posZ;
}
