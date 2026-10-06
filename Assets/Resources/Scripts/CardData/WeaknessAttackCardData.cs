using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessAttackCardData : CardData
{
    private int damage;
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
        damage = data.cardEffectValue;
        statusDuration = statusEffectData.Duration;
        CardEffects = new List<ICardEffect> { new AttackEffect(), new DrawEffect(), new AddStatusEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(damage, 1, 0, 0, statusDuration);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        cardRuntimeValue.statusDuration = cardInstance.IsUpgraded ? statusDuration + 1 : statusDuration;

        if (cardInstance.IsOverload)
            cardRuntimeValue.skillValue += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{cardDamage}", $"{cardRuntimeValue.skillValue}")
            .Replace("{duration}", $"{cardRuntimeValue.statusDuration}")
            .Replace("{extraDraw}", $"{cardRuntimeValue.addDrawCard}");
    }
}
