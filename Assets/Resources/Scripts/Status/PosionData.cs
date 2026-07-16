using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosionData : StatusEffectData
{
    public override void CreateStatusEffectData(StatusEffectJsonData data)
    {
        effectName = data.effectName;
        duration = data.duration;
        damage = data.damage;
    }
}
