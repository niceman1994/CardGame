using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCardData : CardData
{
    private int damage;

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
        CardEffects = new List<ICardEffect> { new AttackEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(damage, 0, 0, 0, 0);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        cardRuntimeValue.skillValue = cardInstance.IsUpgraded ? damage + 2 : damage;

        if (cardInstance.IsOverload)
            cardRuntimeValue.skillValue += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{cardDamage}", $"{cardRuntimeValue.skillValue}");
    }
}
