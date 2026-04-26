using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    int CurrentHp();
    void TakeDamage(int damage);
    void AddStatusEffect(StatusEffectData data, int duration);
}
