using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaknessAttackCardData", menuName = "CardScriptable/CreateWeaknessAttackCardData")]
public class WeaknessAttackCardData : CardData
{
    public int damage;

    private int finalDamage;
    private int finalStatusDuration;

    public override bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public override void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not IHealth) return;

        // 적에게 상태이상을 적용시킴
        if (target is IHealth)
            (target as IHealth).AddStatusEffect(cardInstance.statusEffectData, finalStatusDuration);

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = damage, Target = target });
        EventBus<CardGameData>.Publish(GameEventType.CARD_DRAW, new CardGameData { Value = cardSideEffect.draw });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        finalDamage = damage;
        finalStatusDuration = cardInstance.isUpgraded ? cardSideEffect.statusEffect.upgradeDuration : cardSideEffect.statusEffect.duration;

        if (cardInstance.isOverload)
            finalDamage += overloadValue;

        return description.Replace("{damage}", $"{finalDamage}")
            .Replace("{duration}", $"{finalStatusDuration}")
            .Replace("{extraDraw}", $"{cardSideEffect.draw}");
    }
}
