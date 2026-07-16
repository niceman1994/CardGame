using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardDataFactory
{
    private static Dictionary<string, CardData> cardFactory = new Dictionary<string, CardData>
    {
        { "AttackCard", new AttackCardData() },
        { "AddDrawCard", new AddDrawCardData() },
        { "AreaAttackCard", new AreaAttackCardData() },
        { "ManaBoostCard", new ManaBoostCardData() },
        { "OverloadCard", new OverloadCardData() },
        { "ShieldCard", new ShieldCardData() },
        { "StunCard", new StunCardData() },
        { "WeaknessAttackCard", new WeaknessAttackCardData() },
        { "PosionAttackCard", new PosionAttackCardData() }
    };

    public static CardData GetCard(string cardType)
    {
        return cardFactory[cardType];
    }
}

public static class StatusEffectFactory
{
    private static Dictionary<string, StatusEffectData> statusEffectFactory = new Dictionary<string, StatusEffectData>
    {
        { "StunData", new StunData() },
        { "WeaknessData", new WeaknessData() },
        { "PosionData", new PosionData() }
    };

    public static StatusEffectData GetStatusEffect(string statusEffectType)
    {
        if (!string.IsNullOrEmpty(statusEffectType))
            return statusEffectFactory[statusEffectType];

        return null;
    }
}

