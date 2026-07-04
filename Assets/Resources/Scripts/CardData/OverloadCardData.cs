using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverloadCardData", menuName = "CardScriptable/CreateSearchCardData")]
public class OverloadCardData : CardData
{
    public int overloadCost;

    public override bool IsValidTarget(ISelectable target)
    {
        return target is ICard;
    }

    public override void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not ICard) return;

        (target as ICard).ApplyCardOverload(overloadCost);

        if (cardInstance.isUpgraded)
            EventBus<CardGameData>.Publish(GameEventType.MANABOOST, new CardGameData { Value = cardSideEffect.addMana });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        string finalDescription = cardInstance.isUpgraded ?
            $"{description}\n마나를 {cardSideEffect.addMana} 증가시킵니다." : $"{description}";

        return finalDescription;
    }
}
