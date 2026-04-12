using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectInstance
{
    public StatusEffectData data;
    public int remainingTurn;

    public StatusEffectInstance(StatusEffectData data)
    {
        this.data = data;
        remainingTurn = data.duration;
    }

    public void AddStatusTurn()
    {
        remainingTurn += data.duration;
    }
}
