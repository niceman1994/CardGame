using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBoostEffect : ICardEffect
{
    private int addMana;
    private int costChange;
    private string addDescription;

    public void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.MANABOOST, new CardGameData { Value = addMana });

        if (cardInstance.IsUpgraded)
            EventBus<CardGameData>.Publish(GameEventType.COSTDOWN, new CardGameData { Value = costChange });
    }

    public string GetDescription(CardInstance cardInstance)
    {
        ManaBoostCardData data = (ManaBoostCardData)cardInstance.CardData;

        addDescription = cardInstance.IsUpgraded ?
            $"{data.Description}\n임의의 카드 비용을 {Mathf.Abs(data.CardSideEffect.CostChange)} 줄입니다." : data.Description;

        costChange = data.CardSideEffect.CostChange;
        addMana = cardInstance.IsOverload ? data.AddMana + (data.OverloadValue * cardInstance.OverloadStack) : data.AddMana;

        return addDescription.Replace("{addMana}", $"{addMana}");
    }
}
