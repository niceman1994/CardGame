using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddDrawCardData", menuName = "CardScriptable/CreateAddDrawCardData")]
public class AddDrawCardData : CardData
{
    public override void CreateCardEffect()
    {
        CardEffect = new AddDrawEffect();
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
