using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttackEffect : ICardEffect
{
    private int skillPower;

    public void Execute(CardInstance cardInstance, ISelectable target = null)
    {
        EventBus<int>.Publish(GameEventType.AREAATTACK, skillPower);
        SoundManager.Instance.PlayAreaAttackSound();
    }

    public string GetDescription(CardInstance cardInstance)
    {
        AreaAttackCardData data = (AreaAttackCardData)cardInstance.CardData;

        skillPower = cardInstance.IsUpgraded ? data.SkillValue + 1 : data.SkillValue;

        if (cardInstance.IsOverload)
            skillPower += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{skillValue}", $"{skillPower}");
    }
}
