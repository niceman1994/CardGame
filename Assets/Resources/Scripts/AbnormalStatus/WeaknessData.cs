using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaknessData", menuName = "StatusEffectScriptable/CreateWeaknessData")]
public class WeaknessData : StatusEffectData
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
