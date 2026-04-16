using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaBoostCardData", menuName = "CardScriptable/CreateManaBoostCardData")]
public class ManaBoostCardData : CardData
{
    public int addMana;
    [Header("강화")]
    public int otherCardCostDown;

    public override void Execute(CardInstance cardInstance, IHealth target = null)
    {
        GameEvents.OnManaBoost?.Invoke(addMana);

        if (cardInstance.isUpgraded)
            GameEvents.OnCostDown?.Invoke(otherCardCostDown);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        string finalDescription = cardInstance.isUpgraded ?
            $"마나를 {addMana} 증가시키고\n임의의 카드 비용을 {otherCardCostDown} 줄입니다" : $"마나를 {addMana} 증가시킵니다";

        return description.Replace(description, finalDescription);
    }
}
