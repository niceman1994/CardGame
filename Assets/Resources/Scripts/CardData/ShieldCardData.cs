using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldCardData : CardData
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
        shield = data.cardEffectValue;
    }

    [SerializeField] private int shield;

    public int Shield => shield;

    public override void CreateCardEffect()
    {
        CardEffect = new ShieldEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCardCost = cardInstance.IsUpgraded ? cardCost - 1 : cardCost;
        return finalCardCost;
    }
}
