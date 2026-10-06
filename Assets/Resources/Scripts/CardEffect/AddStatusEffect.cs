using System.Collections;

public class AddStatusEffect : ICardEffect
{
    public void Execute(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue, ISelectable target)
    {
        // 적에게 상태이상을 적용시킴
        if (target is IHealth)
            (target as IHealth).AddStatusEffect(cardInstance.StatusEffectData, cardRuntimeValue.statusDuration);
    }

    public float GetEffectInterval() => 0.0f;
}
