using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StunCardData : CardData
{
    public override void CreateCardData(CardJsonData data, Sprite handleSprite, CardSideEffect cardSideEffect)
    {
        requiresTarget = data.requiresTarget;
        cardCost = data.cost;
        cardName = data.cardName;
        description = data.description;
        cardImage = handleSprite;
        this.cardSideEffect = cardSideEffect;
        overloadValue = data.overloadValue;
        statusDuration = this.cardSideEffect.StatusEffect.Duration;
    }

    private int statusDuration;

    public int StatusDuration => statusDuration;

    public override void CreateCardEffect()
    {
        CardEffect = new StunEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }
}
