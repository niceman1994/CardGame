using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverloadCardData", menuName = "CardScriptable/CreateSearchCardData")]
public class OverloadCardData : CardData
{
    [SerializeField] private int overloadCost;

    public int OverloadCost => overloadCost;

    public override void CreateCardEffect()
    {
        CardEffect = new OverloadEffect();
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
