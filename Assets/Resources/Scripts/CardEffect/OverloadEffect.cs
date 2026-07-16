using System.Text;

public class OverloadEffect : ICardEffect
{
    private int overloadCost;
    private string addDescription;
    private StringBuilder sbOverload = new StringBuilder();

    public bool IsValidTarget(ISelectable target)
    {
        return target is ICard;
    }

    public void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not ICard) return;

        (target as ICard).ApplyCardOverload(overloadCost);

        if (cardInstance.IsOverload)
            EventBus.Publish(GameEventType.OVERLOAD);
    }

    public string GetDescription(CardInstance cardInstance)
    {
        OverloadCardData data = (OverloadCardData)cardInstance.CardData;

        overloadCost = cardInstance.IsUpgraded ? 0 : data.OverloadCost;
        sbOverload.Clear();

        addDescription = cardInstance.IsUpgraded ? "카드 1장을 코스트 증가 없이 효과를 강화합니다." : $"{data.Description}";
        sbOverload.Append(addDescription);

        if (cardInstance.IsOverload)
            sbOverload.Append($"(임의의 카드 {data.OverloadValue}장의 효과를 강화합니다.)");

        return sbOverload.ToString();
    }
}
