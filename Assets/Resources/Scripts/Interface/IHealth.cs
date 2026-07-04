public interface IHealth : ISelectable
{
    int CurrentHp();
    void TakeDamage(int damage);
    void AddStatusEffect(StatusEffectData data, int duration);
}
