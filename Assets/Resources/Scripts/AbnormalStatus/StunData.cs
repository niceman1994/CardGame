using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StunData", menuName = "StatusEffectScriptable/CreateStunData")]
public class StunData : StatusEffectData
{
    public override void ShowEffect(HealthStat healthStat)
    {
        healthStat.ActiveStatusEffect(effectName);
    }

    public override void HideEffect(HealthStat healthStat)
    {
        healthStat.DeactiveStatusEffect(effectName);
    }
}
