using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseCardData", menuName = "CardScriptable/CreateDefenseCardData")]
public class DefenseCardData : CardData
{
    public int shield;
    [Header("°­È­")]
    public int upgradeShield;

    public override void Execute(CardInstance cardInstance, IHealth target = null)
    {
        int finalShield = cardInstance.isUpgraded ? upgradeShield : shield;
        GameEvents.OnPlayerDefend?.Invoke(finalShield);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCardCost = cardInstance.isUpgraded ? cardCost - 1 : cardCost;
        return finalCardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        int finalShield = cardInstance.isUpgraded ? upgradeShield : shield;
        return description.Replace("{shield}", $"{finalShield}");
    }
}
