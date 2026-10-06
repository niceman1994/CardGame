using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDrawCardData : CardData
{
    private int addDrawCard;

    public override void CreateCardData(CardJsonData data, Sprite handleSprite, StatusEffectData effectData)
    {
        requiresTarget = data.requiresTarget;
        cardCost = data.cost;
        cardName = data.cardName;
        description = data.description;
        upgradeDescription = data.upgradeDescription;
        cardImage = handleSprite;
        statusEffectData = effectData;
        overloadValue = data.overloadValue;
        addDrawCard = data.cardEffectValue;
        CardEffects = new List<ICardEffect> { new DrawEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(0, addDrawCard, 0, 0, 0);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        cardRuntimeValue.addDrawCard = cardInstance.IsUpgraded ? addDrawCard + 1 : addDrawCard;

        if (cardInstance.IsOverload)
            cardRuntimeValue.addDrawCard += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{draw}", $"{cardRuntimeValue.addDrawCard}");
    }
}
