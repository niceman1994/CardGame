using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaBoostCardData", menuName = "CardScriptable/CreateManaBoostCardData")]
public class ManaBoostCardData : CardData
{
    public int addMana;

    public override void Execute(IHealth target = null)
    {
        GameEvents.OnManaBoost?.Invoke(addMana);
    }
}
