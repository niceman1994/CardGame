using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseCardData", menuName = "CardScriptable/CreateDefenseCardData")]
public class DefenseCardData : CardData
{
    public int shield;

    public override void Execute(IHealth target = null)
    {
        GameEvents.OnPlayerDefend?.Invoke(shield);
    }
}
