using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessAttackEffect : ICardEffect
{
    private int damage;
    private int draw;
    private int statusDuration;

    public bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not IHealth) return;

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = damage, Target = target });
        EventBus<CardGameData>.Publish(GameEventType.CARD_DRAW, new CardGameData { Value = draw });

        // 적에게 상태이상을 적용시킴
        if (target is IHealth)
            (target as IHealth).AddStatusEffect(cardInstance.StatusEffectData, statusDuration);
    }

    public string GetDescription(CardInstance cardInstance)
    {
        WeaknessAttackCardData data = (WeaknessAttackCardData)cardInstance.CardData;

        damage = data.Damage;
        draw = data.CardSideEffect.Draw;
        statusDuration = cardInstance.IsUpgraded ? data.StatusDuration + 1 : data.StatusDuration;

        if (cardInstance.IsOverload)
            damage += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{damage}", $"{damage}")
            .Replace("{duration}", $"{statusDuration}")
            .Replace("{extraDraw}", $"{draw}");
    }
}
