using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AddDrawCardData : CardData
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
    }

    public override void CreateCardEffect()
    {
        CardEffect = new AddDrawEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }
}
