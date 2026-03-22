using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Attack, Defense, Skill
}

/// <summary>
/// 카드의 기본 정보를 가진 스크립터블 오브젝트
/// </summary>
public class CardData : ScriptableObject
{
    public int cardCost;
    public string cardName;
    public string description;
    public Sprite cardFrontImage;
    public CardType cardType;
}
