using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddDrawCardData", menuName = "CardScriptable/CreateAddDrawCardData")]
public class AddDrawCardData : CardData
{
    public CardSideEffect cardSideEffect = new CardSideEffect();
    [Header("°­È­")]
    public int addDrawCard;

    public override void Execute(CardInstance cardInstance, IHealth target)
    {
        int finalDrawCardCount = cardInstance.isUpgraded ? cardSideEffect.draw + addDrawCard : cardSideEffect.draw;
        GameEvents.OnExtraCardDraw?.Invoke(finalDrawCardCount);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        int finalDrawCardCount = cardInstance.isUpgraded ? cardSideEffect.draw + addDrawCard : cardSideEffect.draw;
        return description.Replace("{draw}", $"{finalDrawCardCount}");
    }
}
