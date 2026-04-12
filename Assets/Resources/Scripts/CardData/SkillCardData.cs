using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardData", menuName = "CardScriptable/CreateSkillCardData")]
public class SkillCardData : CardData
{
    public int skillValue;
    public AudioClip skillClip;

    public override void Execute(IHealth target)
    {
        GameEvents.OnPlayerAoeAttack.Invoke(skillValue);
        SoundManager.Instance.PlaySkillSound(skillClip);  
    }
}
