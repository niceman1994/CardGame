using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRuntimeValue
{
    private int defaultSkillvalue;
    public int skillValue;

    private int defaultAddDrawCard;
    public int addDrawCard;

    private int defaultAddMana;
    public int addMana;

    private int defaultCostChange;
    public int costChange;

    private int defaultStatusDuration;
    public int statusDuration;

    public CardRuntimeValue(int skillValue, int addDrawCard, int addMana, int costChange, int statusDuration)
    {
        this.skillValue = defaultSkillvalue = skillValue;
        this.addDrawCard = defaultAddDrawCard = addDrawCard;
        this.addMana = defaultAddMana = addMana;
        this.costChange = defaultCostChange = costChange;
        this.statusDuration = defaultStatusDuration = statusDuration;
    }

    public void ResetRuntimValue()
    {
        skillValue = defaultSkillvalue;
        addDrawCard = defaultAddDrawCard;
        addMana = defaultAddMana;
        costChange = defaultCostChange;
        statusDuration = defaultStatusDuration;
    }
}

public class CardInstance
{
    private bool isUpgraded;
    private int overloadStack;                   // 과부하가 중첩 가능하도록 하는 스택 변수
    private CardData cardData;
    private CardRuntimeValue cardRuntimeValue;

    public bool IsUpgraded => isUpgraded;
    public bool IsOverload => overloadStack > 0;
    public int OverloadStack => overloadStack;
    public CardData CardData => cardData;
    public StatusEffectData StatusEffectData => cardData.StatusEffectData;

    public CardInstance(bool isUpgraded, CardData cardData)
    {
        this.isUpgraded = isUpgraded;
        this.cardData = cardData;
        cardRuntimeValue = this.cardData.CreateRuntimeValue();
    }

    public void SetCardUpgrade()
    {
        isUpgraded = true;
    }

    public bool CheckRequiresTarget()
    {
        return cardData.RequiresTarget == true;
    }

    // 카드가 가리킨 대상이 유효한 대상인지 확인하는 함수
    public bool IsValidTarget(ISelectable target)
    {
        return cardData.CardEffects.Any(x => x.IsValidTarget(target));
    }

    // 카드를 강화하면 이름 뒤에 +를 붙임
    public string GetCardName()
    {
        return isUpgraded ? $"{cardData.CardName}+" : $"{cardData.CardName}";
    }

    public string GetDescription()
    {
        return cardData.GetDescription(this, cardRuntimeValue);
    }

    public IEnumerator Execute(ISelectable target)
    {
        for (int i = 0; i < cardData.CardEffects.Count; i++)
        {
            cardData.CardEffects[i].Execute(this, cardRuntimeValue, target);
            yield return new WaitForSeconds(cardData.CardEffects[i].GetEffectInterval());
        }
    }

    public void ResetRuntimeValue()
    {
        cardRuntimeValue.ResetRuntimValue();
    }

    public void AddOverloadStack()
    {
        overloadStack += 1;
    }

    public void ResetOverloadStack()
    {
        overloadStack = 0;
    }
}
