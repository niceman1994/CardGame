using System.Collections;

public class AttackEffect : ICardEffect
{
    public bool IsValidTarget(ISelectable target)
    {
        return target is IHealth;
    }

    public void Execute(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue, ISelectable target)
    {
        if (target is not IHealth) return;

        EventBus<CardGameData>.Publish(GameEventType.PLAYERATTACK, new CardGameData { Value = cardRuntimeValue.skillValue, Target = target });
    }

    public float GetEffectInterval() => 0.0f;
}
