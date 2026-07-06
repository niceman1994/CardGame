using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackCardData", menuName = "CardScriptable/CreateAttackCardData")]
public class AttackCardData : CardData
{
    public int damage;
    [Header("°­È­")]
    public int addDamage;

    private int finalDamage;

    public override bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public override void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not IHealth) return;

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = finalDamage, Target = target });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        finalDamage = cardInstance.IsUpgraded ? damage + addDamage : damage;

        if (cardInstance.IsOverload)
            finalDamage += overloadValue * cardInstance.OverloadStack;

        return description.Replace("{damage}", $"{finalDamage}");
    }
}
