using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaAttackCardData : CardData
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
        skillValue = data.cardEffectValue;
    }

    private int skillValue;

    public int SkillValue => skillValue;

    public override void CreateCardEffect()
    {
        CardEffect = new AreaAttackEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCost = cardInstance.IsUpgraded ? cardCost - 1 : cardCost;
        return finalCost;
    }
}
