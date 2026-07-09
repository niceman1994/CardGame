using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[System.Serializable]
public class MonsterRuntimeStat
{
    public int currentShield;
    public int currentHp;
    public int maxHp;
    public int currentAtk;
}

public class Monster : MonoBehaviour, IHealth
{
    [SerializeField] MonsterData monsterData;
    [SerializeField] MonsterRuntimeStat runtimeStat;
    [SerializeField] HealthStat healthStat;
    [SerializeField] MonsterBattleStat monsterBattleStat;
    [SerializeField] ObjectSound objectSound;

    private Animator animator;
    private Sequence deathSequence;
    private List<StatusEffectInstance> effects = new List<StatusEffectInstance>();
    private UnityAction restartAction;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        restartAction = () => ObjectPoolManager.Instance.ReturnPooledObject(this);
    }

    private void OnEnable()
    {
        EventBus<int>.Subscribe(GameEventType.AREAATTACK, TakeDamage);
        EventBus<Monster>.Subscribe(GameEventType.ENEMYDEATH, PlayDeathAni);
        EventBus.Subscribe(GameEventType.RESTART, restartAction);
    }

    private void OnDisable()
    {
        EventBus<int>.Unsubscribe(GameEventType.AREAATTACK, TakeDamage);
        EventBus<Monster>.Unsubscribe(GameEventType.ENEMYDEATH, PlayDeathAni);
        EventBus.Unsubscribe(GameEventType.RESTART, restartAction);
    }

    public void InitMonster()
    {
        effects.Clear();
        runtimeStat.currentHp = monsterData.monsterHp;
        runtimeStat.maxHp = monsterData.monsterMaxHp;
        runtimeStat.currentAtk = monsterData.monsterAtk;

        healthStat.ResetStatusEffect();
        healthStat.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);
        monsterBattleStat.SetAttackPower(runtimeStat.currentAtk);
    }

    public IEnumerator ExecuteMonsterAction(IHealth target)
    {
        if (!effects.Any(x => x.HasStatusEffect("기절")) && target.CurrentHp() > 0)
        {
            switch (monsterBattleStat.DecideAction())
            {
                case MonsterActionType.Attack:
                    yield return new WaitForSeconds(0.5f);  // 살짝 지연시켜서 몬스터들의 공격이 빠르게 처리되지 않도록 함
                    animator.Play("Attack");
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                    // 게임을 재시작하면 비활성화되기 때문에 대미지 처리를 하지 않도록 예외처리
                    if (gameObject.activeInHierarchy)
                        target.TakeDamage(runtimeStat.currentAtk);
                    break;
                case MonsterActionType.Shield:
                    yield return new WaitForSeconds(1.0f);
                    SetShield(3);
                    break;
            }
        }
    }

    private void SetShield(int shieldAmount)
    {
        runtimeStat.currentShield += shieldAmount;
        healthStat.SetShield(runtimeStat.currentShield);
        objectSound.PlayShieldSound(monsterData.shieldClip);
    }

    public void TakeDamage(int damage)
    {
        // 약점 상태일 때 받는 대미지 50% 증가(소수점 버림)
        if (effects.Any(x => x.HasStatusEffect("약점")))
            damage = (int)(damage * 1.5f);

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
            EventBus<Monster>.Publish(GameEventType.ENEMYDEATH, this);
    }

    public void AddStatusEffect(StatusEffectData data, int duration)
    {
        var effectExisting = effects.Find(e => e.IsSameStatusEffect(data));

        // 같은 상태이상이 추가될 경우 적용 턴을 추가함
        if (effectExisting == null)
            effects.Add(new StatusEffectInstance(data, duration));
        else
            effectExisting.AddStatusTurn(duration);

        data.ShowEffect(healthStat);
    }

    public void CheckStatusEffect()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].DecreaseStatusTurn();

            if (runtimeStat.currentHp <= 0 || effects[i].HasStatusDuration())
                RemoveStatusEffect(effects[i], i);
        }
    }

    private void RemoveStatusEffect(StatusEffectInstance statusEffectInstance, int index)
    {
        statusEffectInstance.HideEffect(healthStat);
        effects.RemoveAt(index);
    }

    public void PlayDeathAni(Monster monster)
    {
        if (monster != this) return;

        deathSequence = DOTween.Sequence();
        deathSequence.AppendCallback(() => MonsterDeath())
            .AppendInterval(animator.GetCurrentAnimatorStateInfo(0).length)
            .AppendCallback(() => ObjectPoolManager.Instance.ReturnPooledObject(monster))   // 몬스터 사망 애니메이션이 완료된 후 회수
            .OnComplete(() => ObjectPoolManager.Instance.OnBattleEnd());
    }

    private void MonsterDeath()
    {
        CheckStatusEffect();
        animator.Play("Death");
        objectSound.PlayDeathSound(monsterData.deathSound);
    }

    public int CurrentHp()
    {
        return runtimeStat.currentHp;
    }
}
