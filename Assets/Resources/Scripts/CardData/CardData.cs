using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSideEffectData
{
    public bool requiresStatusEffect;
    public int draw;
    public int costChange;
    public string statusEffect;
}

public class CardJsonData
{
    public bool requiresTarget;
    public int cost;
    public string cardName;
    public string description;
    public string spriteName;
    public CardSideEffectData cardSideEffect;
    public int overloadValue;
    public int cardEffectValue;
    public int cardCount;
}

public class CardJsonList
{
    public Dictionary<string, CardJsonData> cards;
}

/// <summary>
/// 카드 원본 클래스
/// </summary>
public abstract class CardData
{
    protected bool requiresTarget;                // 대상 지정 여부
    protected int cardCost;
    protected string cardName;
    protected string description;
    protected Sprite cardImage;
    protected CardSideEffect cardSideEffect;
    protected int overloadValue;
    
    public bool RequiresTarget => requiresTarget;
    public string CardName => cardName;
    public string Description => description;
    public Sprite CardImage => cardImage;
    public CardSideEffect CardSideEffect => cardSideEffect;
    public int OverloadValue => overloadValue;
    public ICardEffect CardEffect { get; protected set; }

    public abstract void CreateCardData(CardJsonData data, Sprite handleSprite, CardSideEffect cardSideEffect);
    public abstract void CreateCardEffect();
    // 카드 효과 처리만을 담당하는 함수(코스트는 Hand 스크립트에서 처리함)
    public abstract int GetCardCost(CardInstance cardInstance);

    public virtual string GetDescription(CardInstance cardInstance)
    {
        return CardEffect.GetDescription(cardInstance);
    }
}
