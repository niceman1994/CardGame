using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaknessAttackCardData", menuName = "CardScriptable/CreateWeaknessAttackCardData")]
public class WeaknessAttackCardData : CardData
{
    [SerializeField] private int damage;

    public int Damage => damage;
    public int StatusDuration => cardSideEffect.StatusEffect.Duration;

    public override void CreateCardEffect()
    {
        CardEffect = new WeaknessAttackEffect();
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
