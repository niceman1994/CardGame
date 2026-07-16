using UnityEngine;

/// <summary>
/// 카드의 부가 효과를 다루는 클래스
/// </summary>
public class CardSideEffect
{
    private bool requiresStatusEffect;              // 상태이상 필요 여부
    private int draw = 0;                           // 드로우
    private int costChange = 0;                     // 코스트 변화
    private StatusEffectData statusEffect = null;   // 카드에 상태이상이 있다면 값을 넣음

    public CardSideEffect(CardSideEffectData data)
    {
        requiresStatusEffect = data.requiresStatusEffect;
        draw = data.draw;
        costChange = data.costChange;
        statusEffect = null;
    }

    public void CreateStatusEffect(StatusEffectData statusEffect)
    {
        this.statusEffect = statusEffect;
    }

    public bool RequiresStatusEffect => requiresStatusEffect;
    public int Draw => draw;
    public int CostChange => costChange;
    public StatusEffectData StatusEffect => statusEffect;
}
