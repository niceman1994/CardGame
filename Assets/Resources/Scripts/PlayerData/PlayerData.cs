using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerScriptable/CreatePlayerData")]
public class PlayerData : ScriptableObject
{
    public int playerHp;
    public int playerMaxHp;
    public AudioClip attackSound;
}
