using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterData : ScriptableObject
{
    public int monsterHp;
    public int monsterMaxHp;
    public int monsterAtk;
    public string monsterName;
    public AudioClip attackSound;
}
