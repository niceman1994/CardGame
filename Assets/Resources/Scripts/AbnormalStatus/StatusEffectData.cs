using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    public string effectName;
    public int duration;

    public virtual void Apply(Monster target) { }
    public virtual void Remove(Monster target) { }

    public virtual int ModifyDamageTaken(int damage) { return damage; }
}
