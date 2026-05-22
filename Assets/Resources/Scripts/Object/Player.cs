using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class PlayerRuntimeStat
{
    public int currentShield;
    public int currentHp;
    public int maxHp;
}

public class Player : MonoBehaviour, IHealth
{
    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerRuntimeStat runtimeStat;
    [SerializeField] HealthStat healthStat;
    [SerializeField] ObjectSound objectSound;

    private Animator animator;
    private Sequence deathSequence;

    // ObjectPoolManager에서 생성 후 Player의 Awake가 실행됨
    private void Awake()
    {
        animator = GetComponent<Animator>();
        EventBus<CardGameData>.Publish(GameEventType.PLAYER_REGISTER, new CardGameData { Target = this });
        InitPlayer();
    }

    private void OnEnable()
    {
        EventBus<CardGameData>.Subscribe(GameEventType.PLAYERATTACK, (data) => PlayAttackAni(data.Value, data.Target));
        EventBus<CardGameData>.Subscribe(GameEventType.PLAYERDEFEND, (data) => SetShieldFromCard(data.Value));
        EventBus.Subscribe(GameEventType.PLAYERDEATH, PlayDeathAni);
        EventBus.Subscribe(GameEventType.BATTLE_START, () => StartCoroutine(BattleStart()));
    }

    private void OnDisable()
    {
        EventBus<CardGameData>.Unsubscribe(GameEventType.PLAYERATTACK, (data) => PlayAttackAni(data.Value, data.Target));
        EventBus<CardGameData>.Unsubscribe(GameEventType.PLAYERDEFEND, (data) => SetShieldFromCard(data.Value));
        EventBus.Unsubscribe(GameEventType.PLAYERDEATH, PlayDeathAni);
        EventBus.Unsubscribe(GameEventType.BATTLE_START, () => StartCoroutine(BattleStart()));
    }

    public void InitPlayer()
    {
        runtimeStat.currentShield = 0;
        runtimeStat.currentHp = playerData.playerHp;
        runtimeStat.maxHp = playerData.playerMaxHp;
        healthStat.SetShield(runtimeStat.currentShield);
        healthStat.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);
    }

    private void PlayAttackAni(int attackDamage, IHealth target)
    {
        animator.Play("Attack");
        target.TakeDamage(attackDamage);
    }

    private void SetShieldFromCard(int shieldAmount)
    {
        runtimeStat.currentShield += shieldAmount;
        healthStat.SetShield(runtimeStat.currentShield);
    }

    public void TakeDamage(int damage)
    {
        animator.Play("Take Hit");
        healthStat.SetDamageTextTransform(damage);

        if (runtimeStat.currentShield - damage < 0)
        {
            damage -= runtimeStat.currentShield;
            runtimeStat.currentShield = 0;
        }
        else
        {
            runtimeStat.currentShield -= damage;
            damage = 0;
        }
        healthStat.SetShield(runtimeStat.currentShield);
        runtimeStat.currentHp -= damage;
        healthStat.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);

        if (runtimeStat.currentHp <= 0)
            EventBus.Publish(GameEventType.PLAYERDEATH);
    }

    private void PlayDeathAni()
    {
        deathSequence = DOTween.Sequence();
        deathSequence.AppendCallback(() => PlayerDeath())
            .AppendInterval(animator.GetCurrentAnimatorStateInfo(0).length)
            .OnComplete(() =>
            {
                StartCoroutine(SoundManager.Instance.PlayLoseSound());
                ObjectPoolManager.Instance.ReturnPlayer(this);
            });
    }

    private void PlayerDeath()
    {
        animator.Play("Death");
        objectSound.PlayDeathSound(playerData.deathSound);
    }

    private IEnumerator BattleStart()
    {
        yield return new WaitForSeconds(0.1f);
        InitPlayer();
    }

    public int CurrentHp() => runtimeStat.currentHp;

    public void AddStatusEffect(StatusEffectData data, int duration) { }
}
