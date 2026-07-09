using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDrawEffect : ICardEffect
{
    private int addDrawCard;

    public void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.CARD_DRAW, new CardGameData { Value = addDrawCard });
    }

    public string GetDescription(CardInstance cardInstance)
    {
        AddDrawCardData data = (AddDrawCardData)cardInstance.CardData;

        addDrawCard = cardInstance.IsUpgraded ? data.CardSideEffect.Draw + 1 : data.CardSideEffect.Draw;

        if (cardInstance.IsOverload)
            addDrawCard += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{draw}", $"{addDrawCard}");
    }
}
