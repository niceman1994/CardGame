public class StunEffect : ICardEffect
{
    private int statusEffectDuration;

    public bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not IHealth) return;

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = 0, Target = target });

        // 적에게 상태이상을 적용시킴
        if (target is IHealth)
            (target as IHealth).AddStatusEffect(cardInstance.StatusEffectData, statusEffectDuration);
    }

    public string GetDescription(CardInstance cardInstance)
    {
        StunCardData data = (StunCardData)cardInstance.CardData;

        statusEffectDuration = cardInstance.IsUpgraded ? data.StatusDuration + 1 : data.StatusDuration;

        if (cardInstance.IsOverload)
            statusEffectDuration += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{duration}", $"{statusEffectDuration}");
    }
}
