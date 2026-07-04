public interface ICard : ISelectable
{
    void SetCardData(CardInstance cardInstance);
    void ApplyCardOverload(int overloadCost);
}
