using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Attack, Defense, Skill, Buff, Draw
}

/// <summary>
/// 카드의 기본 정보를 가진 스크립터블 오브젝트
/// </summary>
public abstract class CardData : ScriptableObject
{
    public int cardCost;
    public string cardName;
    [TextArea] public string description;
    public Sprite cardFrontImage;
    public CardType cardType;

    public abstract void Execute(CardInstance cardInstance, IHealth target);
    public abstract int GetCardCost(CardInstance cardInstance);
    public abstract string GetDescription(CardInstance cardInstance);

    // 카드 이름이 강화 여부에 따라 바뀜
    public virtual string GetCardName(CardInstance cardInstance)
    {
        return cardInstance.isUpgraded ? $"{cardName}+" : $"{cardName}";
    }
}
