using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : ICardEffect
{
    private int damage;

    public bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not IHealth) return;

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = damage, Target = target });
    }

    public string GetDescription(CardInstance cardInstance)
    {
        AttackCardData data = (AttackCardData)cardInstance.CardData;
        damage = cardInstance.IsUpgraded ? data.Damage + 2 : data.Damage;

        if (cardInstance.IsOverload)
            damage += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{damage}", $"{damage}");
    }
}
