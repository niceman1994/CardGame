using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardData", menuName = "CardScriptable/CreateSkillCardData")]
public class SkillCardData : CardData
{
    public int skillValue;
    public AudioClip skillClip;
    [Header("°­È­")]
    public int upgradeSkillValue;

    private int finalSkillValue;

    public override void Execute(CardInstance cardInstance, ISelectable target)
    {
        EventBus<int>.Publish(GameEventType.AREAATTACK, finalSkillValue);
        SoundManager.Instance.PlaySkillSound(skillClip);  
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCost = cardInstance.isUpgraded ? cardCost - 1 : cardCost;
        return finalCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        finalSkillValue = cardInstance.isUpgraded ? upgradeSkillValue : skillValue;

        if (cardInstance.isOverload)
            finalSkillValue += overloadValue;

        return description.Replace("{skillValue}", $"{finalSkillValue}");
    }
}
