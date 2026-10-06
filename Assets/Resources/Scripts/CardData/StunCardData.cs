using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunCardData : CardData
{
    private int statusDuration;

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
        statusDuration = statusEffectData.Duration;
        CardEffects = new List<ICardEffect> { new AttackEffect(), new AddStatusEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(0, 0, 0, 0, statusDuration);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        cardRuntimeValue.statusDuration = cardInstance.IsUpgraded ? statusDuration + 1 : statusDuration;

        if (cardInstance.IsOverload)
            cardRuntimeValue.statusDuration += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{duration}", $"{cardRuntimeValue.statusDuration}");
    }
}
