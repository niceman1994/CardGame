using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectInstance
{
    private StatusEffectData data;      // 상태이상 데이터
    private int remainingTurn;          // 남은 상태이상 턴 수

    public StatusEffectInstance(StatusEffectData data, int duration)
    {
        this.data = data;
        remainingTurn = duration;
    }

    public bool IsSameStatusEffect(StatusEffectData data)
    {
        return this.data == data;
    }

    public void AddStatusTurn(int duration)
    {
        remainingTurn += duration;
    }

    public void DecreaseStatusTurn()
    {
        remainingTurn -= 1;
    }

    public bool HasStatusDuration()
    {
        if (remainingTurn <= 0)
            return true;

        return false;
    }

    public bool CheckStatusEffectName(string effectName)
    {
        return data.effectName.Contains(effectName);
    }

    public void HideEffect(HealthStat healthStat)
    {
        data.HideEffect(healthStat);
    }
}
