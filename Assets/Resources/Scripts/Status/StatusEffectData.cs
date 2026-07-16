using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectJsonData
{
    public string effectName;
    public int duration;
    public int damage;
}

public class StatusEffectJsonList
{
    public Dictionary<string, StatusEffectJsonData> statusEffects;
}

public abstract class StatusEffectData
{
    protected string effectName;
    protected int duration;
    protected int damage;

    public string EffectName => effectName;
    public int Duration => duration;
    public int Damage => damage;

    public abstract void CreateStatusEffectData(StatusEffectJsonData data);

    public void ShowEffect(HealthStat healthStat) 
    {
        healthStat.ActiveStatusEffect(effectName);
    }

    public void RemoveEffect(HealthStat healthStat)
    {
        healthStat.DeactiveStatusEffect(effectName);
    }

    public bool HasStatusEffect(string effectName)
    {
        if (this.effectName.Contains(effectName))
            return true;

        return false;
    }
}
