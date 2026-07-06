using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverloadCardData", menuName = "CardScriptable/CreateSearchCardData")]
public class OverloadCardData : CardData
{
    public int overloadCost;

    private StringBuilder sbOverload = new StringBuilder();

    public override bool IsValidTarget(ISelectable target)
    {
        return target is ICard;
    }

    public override void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not ICard) return;

        int finalOverloadCost = cardInstance.IsUpgraded ? 0 : overloadCost;
        (target as ICard).ApplyCardOverload(finalOverloadCost);

        if (cardInstance.IsOverload)
            EventBus.Publish(GameEventType.OVERLOAD);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance)
    {
        sbOverload.Clear();

        string finalDescription = cardInstance.IsUpgraded ? "카드 1장을 코스트 증가 없이 효과를 강화합니다." : $"{description}";
        sbOverload.Append(finalDescription);

        if (cardInstance.IsOverload)
            sbOverload.Append($"(임의의 카드 {overloadValue}장의 효과를 강화합니다.)");

        return sbOverload.ToString();
    }
}
