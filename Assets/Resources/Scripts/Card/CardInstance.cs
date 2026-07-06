using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    private bool isUpgraded;
    private int overloadStack;                   // 과부하가 중첩이 가능하도록 하는 과부하 스택 변수
    private CardData originalCardData;           // 카드가 덱으로 돌아갈 때, 게임을 재시작할 때 원본으로 되돌리기 위한 변수
    private CardData currentCardData;            // 사용 중인 카드 데이터 변수
    private StatusEffectData statusEffectData;   // 상태이상 데이터

    public bool IsUpgraded => isUpgraded;
    public bool IsOverload => overloadStack > 0;
    public int OverloadStack => overloadStack;
    public CardData OriginalCardData => originalCardData;
    public CardData CurrentCardData => currentCardData;
    public StatusEffectData StatusEffectData => statusEffectData;

    public CardInstance(bool isUpgraded, CardData originalCardData, CardData currentCardData, StatusEffectData statusEffectData)
    {
        this.isUpgraded = isUpgraded;
        this.originalCardData = originalCardData;
        this.currentCardData = currentCardData;
        this.statusEffectData = statusEffectData;
    }

    public void SetCardUpgrade()
    {
        isUpgraded = true;
    }

    public bool CheckRequiresTarget()
    {
        return currentCardData.requiresTarget == true;
    }

    public string GetCardName()
    {
        return currentCardData.GetCardName(this);
    }

    public void Execute(ISelectable target)
    {
        currentCardData.Execute(this, target);
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
