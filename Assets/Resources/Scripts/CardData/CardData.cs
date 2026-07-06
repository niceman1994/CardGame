using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드의 기본 정보를 가진 스크립터블 오브젝트
/// </summary>
public abstract class CardData : ScriptableObject
{
    public bool requiresTarget;                    // 대상 지정 여부
    public int cardCost;
    public string cardName;
    [TextArea] public string description;
    public Sprite cardFrontImage;
    public CardSideEffect cardSideEffect = new CardSideEffect();
    public int overloadValue;

    // 카드 효과 처리만을 담당하는 함수(코스트는 Hand 스크립트에서 처리함)
    public abstract void Execute(CardInstance cardInstance, ISelectable target);
    public abstract int GetCardCost(CardInstance cardInstance);
    public abstract string GetDescription(CardInstance cardInstance);

    // 카드가 가리킨 대상이 유효한 대상인지 확인하는 함수
    public virtual bool IsValidTarget(ISelectable target)
    {
        return true;
    }

    public StatusEffectData GetStatusEffectData()
    {
        if (cardSideEffect.requiresStatusEffect == true)
            return cardSideEffect.statusEffect;

        return null;
    }

    // 카드 이름이 강화 여부에 따라 바뀜
    public virtual string GetCardName(CardInstance cardInstance)
    {
        return cardInstance.IsUpgraded ? $"{cardName}+" : $"{cardName}";
    }
}
