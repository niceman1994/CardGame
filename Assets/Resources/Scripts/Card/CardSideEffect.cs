/// <summary>
/// 카드의 부가 효과를 다루는 클래스
/// </summary>
[System.Serializable]
public class CardSideEffect
{
    public bool requiresStatusEffect;              // 상태 이상 필요 여부
    public int draw = 0;                           // 드로우
    public int addMana = 0;                        // 마나 증가량
    public int costChange = 0;                     // 코스트 변화
    public StatusEffectData statusEffect = null;   // 상태이상
}
