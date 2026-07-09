using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    private bool isUpgraded;
    private int overloadStack;                   // 과부하가 중첩이 가능하도록 하는 과부하 스택 변수
    private CardData cardData;            
    private StatusEffectData statusEffectData;   // 상태이상 데이터

    public bool IsUpgraded => isUpgraded;
    public bool IsOverload => overloadStack > 0;
    public int OverloadStack => overloadStack;
    public CardData CardData => cardData;
    public StatusEffectData StatusEffectData => statusEffectData;

    public CardInstance(bool isUpgraded, CardData cardData, StatusEffectData statusEffectData)
    {
        this.isUpgraded = isUpgraded;
        this.cardData = cardData;
        this.statusEffectData = statusEffectData;
        this.cardData.CreateCardEffect();
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
        return cardData.CardEffect.IsValidTarget(target);
    }

    // 카드를 강화하면 이름 뒤에 +를 붙임
    public string GetCardName()
    {
        return isUpgraded ? $"{cardData.CardName}+" : cardData.CardName;
    }

    public void Execute(ISelectable target)
    {
        cardData.CardEffect.Execute(this, target);
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
