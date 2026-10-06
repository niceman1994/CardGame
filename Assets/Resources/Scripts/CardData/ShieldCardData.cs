using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCardData : CardData
{
    private int shield;

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
        shield = data.cardEffectValue;
        CardEffects = new List<ICardEffect> { new ShieldEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(shield, 0, 0, 0, 0);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCardCost = cardInstance.IsUpgraded ? cardCost - 1 : cardCost;
        return finalCardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        cardRuntimeValue.skillValue = cardInstance.IsUpgraded ? shield + 2 : shield;

        if (cardInstance.IsOverload)
            cardRuntimeValue.skillValue += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{shield}", $"{cardRuntimeValue.skillValue}");
    }
}
