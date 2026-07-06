using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddDrawCardData", menuName = "CardScriptable/CreateAddDrawCardData")]
public class AddDrawCardData : CardData
{
    [Header("°­È­")]
    public int addDrawCard;

    private int finalAddDrawCard;

    public override void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.CARD_DRAW, new CardGameData { Value = finalAddDrawCard });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        finalAddDrawCard = cardInstance.IsUpgraded ? cardSideEffect.draw + addDrawCard : cardSideEffect.draw;

        if (cardInstance.IsOverload)
            finalAddDrawCard += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{draw}", $"{finalAddDrawCard}");
    }
}
