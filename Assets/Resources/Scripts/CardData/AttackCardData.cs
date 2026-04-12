using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackCardData", menuName = "CardScriptable/CreateAttackCardData")]
public class AttackCardData : CardData
{
    public int damage;
    public int extraDraw;
    public StatusEffectData statusEffect;

    public override void Execute(IHealth target)
    {
        GameEvents.OnPlayerAttack?.Invoke(damage, target);
        GameEvents.OnExtraCardDraw?.Invoke(extraDraw);

        // 카드가 상태이상을 적에게 적용시킴
        if (statusEffect != null)
            target.AddStatusEffect(statusEffect);
    }
}
