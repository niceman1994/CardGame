using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBoostCardData : CardData
{
    private int addMana;

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
        addMana = data.cardEffectValue;
        CardEffects = new List<ICardEffect> { new ManaBoostEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(0, 0, addMana,-1, 0);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        cardRuntimeValue.addMana = cardInstance.IsOverload ? cardRuntimeValue.addMana + (overloadValue * cardInstance.OverloadStack) : cardRuntimeValue.addMana;

        if (cardInstance.IsUpgraded)
            return upgradeDescription.Replace("{addMana}", $"{cardRuntimeValue.addMana}")
                .Replace("{costChange}", $"{Mathf.Abs(cardRuntimeValue.costChange)}");
        else
            return description.Replace("{addMana}", $"{cardRuntimeValue.addMana}");
    }
}
