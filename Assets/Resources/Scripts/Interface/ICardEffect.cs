public interface ICardEffect
{
    // 카드가 가리킨 대상이 유효한 대상인지 확인하는 함수
    virtual bool IsValidTarget(ISelectable target) => true;
    // 카드 효과 처리만을 담당하는 함수(코스트는 Hand 스크립트에서 처리함)
    void Execute(CardInstance cardInstance, ISelectable target);
    string GetDescription(CardInstance cardInstance);
}
