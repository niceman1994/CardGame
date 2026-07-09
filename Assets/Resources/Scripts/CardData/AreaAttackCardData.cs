using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaAttackCardData", menuName = "CardScriptable/CreateAreaAttackCardData")]
public class AreaAttackCardData : CardData
{
    [SerializeField] private int skillValue;

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

    public override string GetDescription(CardInstance cardInstance)
    {
        return CardEffect.GetDescription(cardInstance);
    }
}
