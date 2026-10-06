using System.Collections;
using UnityEngine;

public class AreaAttackEffect : ICardEffect
{
    public void Execute(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue, ISelectable target = null)
    {
        EventBus<int>.Publish(GameEventType.AREAATTACK, cardRuntimeValue.skillValue);
        SoundManager.Instance.PlayAreaAttackSound();
    }

    public float GetEffectInterval() => 0.15f;
}
