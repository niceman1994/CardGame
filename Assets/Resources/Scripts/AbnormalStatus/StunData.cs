using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StunData", menuName = "StatusEffectScriptable/CreateStunData")]
public class StunData : StatusEffectData
{
    public override bool HasStatusEffect(string effectName)
    {
        if (this.effectName.Contains(effectName))
            return true;

        return false;
    }

    public override void ShowEffect(HealthStat healthStat)
    {
        healthStat.ActiveStatusEffect(effectName);
    }

    public override void HideEffect(HealthStat healthStat)
    {
        healthStat.DeactiveStatusEffect(effectName);
    }
}
