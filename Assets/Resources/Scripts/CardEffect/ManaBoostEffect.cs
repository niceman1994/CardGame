using System.Collections;

public class ManaBoostEffect : ICardEffect
{
    public void Execute(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.MANABOOST, new CardGameData { Value = cardRuntimeValue.addMana });

        if (cardInstance.IsUpgraded)
            EventBus<CardGameData>.Publish(GameEventType.COSTDOWN, new CardGameData { Value = cardRuntimeValue.costChange });
    }

    public float GetEffectInterval() => 0.0f;
}
