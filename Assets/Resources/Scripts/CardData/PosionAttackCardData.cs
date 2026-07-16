using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosionAttackCardData : CardData
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
        damage = data.cardEffectValue;
        statusDuration = this.cardSideEffect.StatusEffect.Duration;
    }

    private int damage;
    private int statusDuration;

    public int Damage => damage;
    public int StatusDuration => statusDuration;

    public override void CreateCardEffect()
    {
        CardEffect = new PosionEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }
}
