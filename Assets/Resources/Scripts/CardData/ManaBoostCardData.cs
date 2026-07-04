using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaBoostCardData", menuName = "CardScriptable/CreateManaBoostCardData")]
public class ManaBoostCardData : CardData
{
    private int finalAddMana;

    public override void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.MANABOOST, new CardGameData { Value = finalAddMana });

        if (cardInstance.isUpgraded)
            EventBus<CardGameData>.Publish(GameEventType.COSTDOWN, new CardGameData { Value = cardSideEffect.costChange });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        string finalDescription = cardInstance.isUpgraded ?
            $"{description}\n임의의 카드 비용을 {Mathf.Abs(cardSideEffect.costChange)} 줄입니다." : $"{description}";

        finalAddMana = cardInstance.isOverload ? cardSideEffect.addMana + overloadValue : cardSideEffect.addMana;

        return finalDescription.Replace("{addMana}", $"{finalAddMana}");
    }
}
