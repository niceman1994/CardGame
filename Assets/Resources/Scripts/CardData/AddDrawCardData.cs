using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddDrawCardData", menuName = "CardScriptable/CreateAddDrawCardData")]
public class AddDrawCardData : CardData
{
    public int drawCount;
    [Header("°­È­")]
    public int addDrawCardCount;

    public override void Execute(CardInstance cardInstance, IHealth target)
    {
        int finalDrawCardCount = cardInstance.isUpgraded ? drawCount + addDrawCardCount : drawCount;
        GameEvents.OnExtraCardDraw?.Invoke(finalDrawCardCount);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        int finalDrawCardCount = cardInstance.isUpgraded ? drawCount + addDrawCardCount : drawCount;
        return description.Replace("{draw}", $"{finalDrawCardCount}");
    }
}
