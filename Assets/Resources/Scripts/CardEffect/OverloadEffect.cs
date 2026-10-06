using System.Collections;
using UnityEngine;

public class OverloadEffect : ICardEffect
{
    public bool IsValidTarget(ISelectable target)
    {
        return target is ICard;
    }

    public void Execute(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue, ISelectable target)
    {
        if (target is not ICard) return;
        //Debug.Log($"isUpgrade : {cardInstance.IsUpgraded}, costChange : {cardRuntimeValue.costChange}");
        (target as ICard).ApplyCardOverload(cardRuntimeValue.costChange);
        
        if (cardInstance.IsOverload)
            EventBus.Publish(GameEventType.OVERLOAD);
    }

    public float GetEffectInterval() => 0.0f;
}
