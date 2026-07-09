using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : ICardEffect
{
    private int shield;

    public void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.PLAYERDEFEND, new CardGameData { Value = shield });
    }

    public string GetDescription(CardInstance cardInstance)
    {
        ShieldCardData data = (ShieldCardData)cardInstance.CardData;

        shield = cardInstance.IsUpgraded ? data.Shield + 2 : data.Shield;

        if (cardInstance.IsOverload)
            shield += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{shield}", $"{shield}");
    }
}
