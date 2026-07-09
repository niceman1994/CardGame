using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldCardData", menuName = "CardScriptable/CreateShieldCardData")]
public class ShieldCardData : CardData
{
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

    public override string GetDescription(CardInstance cardInstance)
    {
        return CardEffect.GetDescription(cardInstance);
    }
}
