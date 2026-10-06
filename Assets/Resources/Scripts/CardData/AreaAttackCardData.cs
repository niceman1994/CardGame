using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttackCardData : CardData
{
    private int skillPower;

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
        skillPower = data.cardEffectValue;
        CardEffects = new List<ICardEffect> { new AreaAttackEffect(), new AreaAttackEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(skillPower, 0, 0, 0, 0);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        if (cardInstance.IsOverload)
            cardRuntimeValue.skillValue += overloadValue * cardInstance.OverloadStack;

        if (cardInstance.IsUpgraded)
            return upgradeDescription.Replace("{skillPower}", $"{cardRuntimeValue.skillValue}");
        else
            return description.Replace("{skillPower}", $"{cardRuntimeValue.skillValue}");
    }
}
