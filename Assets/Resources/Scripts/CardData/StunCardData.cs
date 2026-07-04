using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StunCardData", menuName = "CardScriptable/CreateStunCardData")]
public class StunCardData : CardData
{
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

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = 0, Target = target });
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        finalStatusDuration = cardInstance.isUpgraded ? cardSideEffect.statusEffect.upgradeDuration : cardSideEffect.statusEffect.duration;

        if (cardInstance.isOverload)
            finalStatusDuration += overloadValue;

        return description.Replace("{duration}", $"{finalStatusDuration}");
    }
}
