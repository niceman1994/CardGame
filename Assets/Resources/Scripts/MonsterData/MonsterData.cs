using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterScriptable/CreateMonsterData")]
public class MonsterData : ScriptableObject
{
    public int monsterHp;
    public int monsterMaxHp;
    public int monsterAtk;
    public string monsterName;
    public AudioClip shieldClip;
    public AudioClip deathSound;
}
