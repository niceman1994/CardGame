using System.Collections;

public class DrawEffect : ICardEffect
{
    public void Execute(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue, ISelectable target = null)
    {
        EventBus<CardGameData>.Publish(GameEventType.CARD_DRAW, new CardGameData { Value = cardRuntimeValue.addDrawCard });
    }

    public float GetEffectInterval() => 0.0f;
}
