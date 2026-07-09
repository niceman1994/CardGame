using UnityEngine;

/// <summary>
/// 카드의 부가 효과를 다루는 클래스
/// </summary>
[System.Serializable]
public class CardSideEffect
{
    [SerializeField] private bool requiresStatusEffect;              // 상태이상 필요 여부
    [SerializeField] private int draw = 0;                           // 드로우
    [SerializeField] private int costChange = 0;                     // 코스트 변화
    [SerializeField] private StatusEffectData statusEffect = null;   // 상태이상

    public bool RequiresStatusEffect => requiresStatusEffect;
    public int Draw => draw;
    public int CostChange => costChange;
    public StatusEffectData StatusEffect => statusEffect;
}
