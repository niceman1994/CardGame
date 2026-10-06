using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
// TODO : RESTART 버튼을 눌러서 게임을 재시작했을 때 강화된 과부하 카드 코스트가 0->1로 변경되는 현상 발견
public class OverloadCardData : CardData
{
    private int overloadCost;
    private StringBuilder sbOverload = new StringBuilder();

    public override void CreateCardData(CardJsonData data, Sprite handleSprite, StatusEffectData effectData)
    {
        requiresTarget = data.requiresTarget;
        cardCost = data.cost;
        cardName = data.cardName;
        description = data.description;
        upgradeDescription = data.upgradeDescription;
        cardImage = handleSprite;
        statusEffectData = effectData;
        overloadValue = data.overloadValue;
        overloadCost = data.cardEffectValue;
        CardEffects = new List<ICardEffect> { new OverloadEffect() };
    }

    public override CardRuntimeValue CreateRuntimeValue()
    {
        return new CardRuntimeValue(0, 0, 0, overloadCost, 0);
    }

    public override int GetCardCost(CardInstance cardInstance)
    {
        return cardCost;
    }

    public override string GetDescription(CardInstance cardInstance, CardRuntimeValue cardRuntimeValue)
    {
        sbOverload.Clear();
        
        if (cardInstance.IsUpgraded)
        {
            cardRuntimeValue.costChange = 0;
            sbOverload.Append(upgradeDescription);
        }
        else
        {
            cardRuntimeValue.costChange = overloadCost;
            sbOverload.Append(description);
        }
        
        if (cardInstance.IsOverload)
            sbOverload.Append($"(임의의 카드 {overloadValue}장의 효과를 강화합니다.)");

        return sbOverload.ToString();
    }
}
