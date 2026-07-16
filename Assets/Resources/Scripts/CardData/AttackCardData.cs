using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackCardData : CardData
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
    }

    private int damage;

    public int Damage => damage;

    public override void CreateCardEffect()
    {
        CardEffect = new AttackEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }
}
