/// <summary>
/// 카드의 부가 효과를 다루는 클래스
/// </summary>
[System.Serializable]
public class CardSideEffect
{
    public int draw = 0;                           // 드로우
    public int costDown = 0;                       // 코스트 감소
    public StatusEffectData statusEffect = null;   // 상태이상
}
