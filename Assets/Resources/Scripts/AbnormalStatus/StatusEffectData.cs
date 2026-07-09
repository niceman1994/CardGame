using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    [SerializeField] protected string effectName;
    [SerializeField] protected int duration;

    public int Duration => duration;

    public abstract bool HasStatusEffect(string effectName);
    public virtual void ShowEffect(HealthStat healthStat) { }
    public virtual void HideEffect(HealthStat healthStat) { }
}
