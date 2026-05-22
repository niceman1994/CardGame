using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaBoostCardData", menuName = "CardScriptable/CreateManaBoostCardData")]
public class ManaBoostCardData : CardData
{
    public int addMana;
    public CardSideEffect cardSideEffect = new CardSideEffect();

    public override void Execute(CardInstance cardInstance, IHealth target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.MANABOOST, new CardGameData { Value = addMana });

        if (cardInstance.isUpgraded)
            EventBus<CardGameData>.Publish(GameEventType.COSTDOWN, new CardGameData { Value = cardSideEffect.costDown });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        string finalDescription = cardInstance.isUpgraded ?
            $"{description}\n임의의 카드 비용을 {cardSideEffect.costDown} 줄입니다." : $"{description}";

        return finalDescription.Replace("{addMana}", $"{addMana}");
    }
}
