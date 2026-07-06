using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseCardData", menuName = "CardScriptable/CreateDefenseCardData")]
public class DefenseCardData : CardData
{
    public int shield;
    [Header("°­È­")]
    public int upgradeShield;

    private int finalShield;

    public override void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.PLAYERDEFEND, new CardGameData { Value = finalShield });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCardCost = cardInstance.IsUpgraded ? cardCost - 1 : cardCost;
        return finalCardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        finalShield = cardInstance.IsUpgraded ? upgradeShield : shield;

        if (cardInstance.IsOverload)
            finalShield += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{shield}", $"{finalShield}");
    }
}
