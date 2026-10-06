using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardJsonData
{
    public bool requiresTarget;
    public int cost;
    public string cardName;
    public string description;
    public string upgradeDescription;
    public string spriteName;
    public string statusEffect;
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
    protected string upgradeDescription;          // 강화된 카드 텍스트
    protected Sprite cardImage;
    protected StatusEffectData statusEffectData;
    protected int overloadValue;

    public bool RequiresTarget => requiresTarget;
    public string CardName => cardName;
    public Sprite CardImage => cardImage;
    public string Description => description;
    public string UpgradeDescription => upgradeDescription;
    public StatusEffectData StatusEffectData => statusEffectData;
    public int OverloadValue => overloadValue;
    public List<ICardEffect> CardEffects { get; protected set; }

    public abstract void CreateCardData(CardJsonData data, Sprite handleSprite, StatusEffectData effectData);
    public abstract CardRuntimeValue CreateRuntimeValue();
    // 카드 효과 처리만을 담당하는 함수(코스트는 Hand 스크립트에서 처리함)
    public abstract int GetCardCost(CardInstance cardInstance);
    public abstract string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue);
}
