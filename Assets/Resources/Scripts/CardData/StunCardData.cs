using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StunCardData", menuName = "CardScriptable/CreateStunCardData")]
public class StunCardData : CardData
{
    public int StatusDuration => cardSideEffect.StatusEffect.Duration;

    public override void CreateCardEffect()
    {
        CardEffect = new StunEffect();
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        return CardEffect.GetDescription(cardInstance);
    }
}
