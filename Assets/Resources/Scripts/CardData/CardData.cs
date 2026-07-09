using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드의 기본 정보를 가진 스크립터블 오브젝트
/// </summary>
public abstract class CardData : ScriptableObject
{
    [SerializeField] protected bool requiresTarget;                // 대상 지정 여부
    [SerializeField] protected int cardCost;
    [SerializeField] protected string cardName;
    [SerializeField, TextArea] protected string description;
    [SerializeField] protected Sprite cardFrontImage;
    [SerializeField] protected CardSideEffect cardSideEffect = new CardSideEffect();
    [SerializeField] protected int overloadValue;

    public bool RequiresTarget => requiresTarget;
    public string CardName => cardName;
    public string Description => description;
    public Sprite CardFrontImage => cardFrontImage;
    public CardSideEffect CardSideEffect => cardSideEffect;
    public int OverloadValue => overloadValue;
    public ICardEffect CardEffect { get; protected set; }

    public abstract void CreateCardEffect();
    // 카드 효과 처리만을 담당하는 함수(코스트는 Hand 스크립트에서 처리함)
    public abstract int GetCardCost(CardInstance cardInstance);
    public abstract string GetDescription(CardInstance cardInstance);

    public StatusEffectData GetStatusEffectData()
    {
        if (cardSideEffect.RequiresStatusEffect == true)
            return cardSideEffect.StatusEffect;

        return null;
    }
}
