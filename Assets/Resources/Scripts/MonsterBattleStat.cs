using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterActionType
{
    Attack, Shield
}

public class MonsterBattleStat : MonoBehaviour
{
    [SerializeField] MonsterActionType actionType;
    [SerializeField] Text attackPowerText;

    public void SetAttackPower(int attackPower)
    {
        attackPowerText.text = $"{attackPower}";
    }

    public MonsterActionType DecideAction()
    {
        return Random.Range(0, 2) == 0 ? MonsterActionType.Attack : MonsterActionType.Shield;
    }
}
