public class PosionEffect : ICardEffect
{
    private int damage;
    private int statusDuration;

    public bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public void Execute(CardInstance cardInstance, ISelectable target)
    {
        if (target is not IHealth) return;

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = damage, Target = target });

        // 적에게 상태이상을 적용시킴
        if (target is IHealth)
            (target as IHealth).AddStatusEffect(cardInstance.StatusEffectData, statusDuration);
    }

    public string GetDescription(CardInstance cardInstance)
    {
        PosionAttackCardData data = (PosionAttackCardData)cardInstance.CardData;

        damage = data.Damage;
        statusDuration = cardInstance.IsUpgraded ? data.StatusDuration + 1 : data.StatusDuration;

        if (cardInstance.IsOverload)
            statusDuration += data.OverloadValue * cardInstance.OverloadStack;

        return data.Description.Replace("{cardDamage}", $"{damage}")
            .Replace("{duration}", $"{statusDuration}");
    }
}
