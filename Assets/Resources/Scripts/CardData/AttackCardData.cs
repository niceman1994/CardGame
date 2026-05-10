using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackCardData", menuName = "CardScriptable/CreateAttackCardData")]
public class AttackCardData : CardData
{
    public int damage;
    public CardSideEffect cardSideEffect = new CardSideEffect();
    [Header("강화")]
    public int addDamage;

    public override void Execute(CardInstance cardInstance, IHealth target)
    {
        int finalDamage = cardInstance.isUpgraded ? damage + addDamage : damage;

        // 적에게 상태이상을 적용시킴
        if (cardInstance.statusEffectData != null)
        {
            int finalDuration = cardInstance.isUpgraded ? cardSideEffect.statusEffect.upgradeDuration : cardSideEffect.statusEffect.duration;
            target.AddStatusEffect(cardInstance.statusEffectData, finalDuration);
        }
        GameEvents.OnPlayerAttack?.Invoke(finalDamage, target);
        GameEvents.OnExtraCardDraw?.Invoke(cardSideEffect.draw);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        int finalDamage = cardInstance.isUpgraded ? damage + addDamage : damage;
        string finalDescription = string.Empty;

        if (cardSideEffect.statusEffect != null)
        {
            int finalDuration = cardInstance.isUpgraded ? cardSideEffect.statusEffect.upgradeDuration : cardSideEffect.statusEffect.duration;

            finalDescription = description.Replace("{damage}", $"{finalDamage}")
            .Replace("{duration}", $"{finalDuration}")
            .Replace("{extraDraw}", $"{cardSideEffect.draw}");
        }
        else
            finalDescription = description.Replace("{damage}", $"{finalDamage}");

        return finalDescription;
    }
}
