using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackCardData", menuName = "CardScriptable/CreateAttackCardData")]
public class AttackCardData : CardData
{
    public int damage;
    public int extraDraw;
    public StatusEffectData statusEffect;
    [Header("강화")]
    public int upgradeDamage;

    public override void Execute(CardInstance cardInstance, IHealth target)
    {
        int finalDamage = cardInstance.isUpgraded ? upgradeDamage : damage;

        // 적에게 상태이상을 적용시킴
        if (cardInstance.statusEffectData != null)
        {
            int finalDuration = cardInstance.isUpgraded ? statusEffect.upgradeDuration : statusEffect.duration;
            target.AddStatusEffect(cardInstance.statusEffectData, finalDuration);
        }

        GameEvents.OnPlayerAttack?.Invoke(finalDamage, target);
        GameEvents.OnExtraCardDraw?.Invoke(extraDraw);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        int finalDamage = cardInstance.isUpgraded ? upgradeDamage : damage;
        string finalDescription = string.Empty;

        if (statusEffect != null)
        {
            int finalDuration = cardInstance.isUpgraded ? statusEffect.upgradeDuration : statusEffect.duration;

            finalDescription = description.Replace("{damage}", $"{finalDamage}")
            .Replace("{duration}", $"{finalDuration}")
            .Replace("{extraDraw}", $"{extraDraw}");
        }
        else
            finalDescription = description.Replace("{damage}", $"{finalDamage}");

        return finalDescription;
    }
}
