using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardData", menuName = "CardScriptable/CreateSkillCardData")]
public class SkillCardData : CardData
{
    public int skillValue;
    public AudioClip skillClip;
    [Header("°­È­")]
    public int upgradeCardCost;
    public int upgradeSkillValue;

    public override void Execute(CardInstance cardInstance, IHealth target)
    {
        int finalSkillValue = cardInstance.isUpgraded ? upgradeSkillValue : skillValue;

        GameEvents.OnPlayerAoeAttack.Invoke(finalSkillValue);
        SoundManager.Instance.PlaySkillSound(skillClip);  
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        int finalCost = cardInstance.isUpgraded ? upgradeCardCost : cardCost;
        return finalCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        int finalSkillValue = cardInstance.isUpgraded ? upgradeSkillValue : skillValue;
        return description.Replace("{skillValue}", $"{finalSkillValue}");
    }
}
