using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField] HealthBar healthBar;
    [SerializeField] MonsterBattleStat monsterBattleStat;
    [SerializeField] ObjectSound objectSound;

    private Animator animator;
    private Sequence deathSequence;

    private void OnEnable()
    {
        GameEvents.OnPlayerAoeAttack += TakeDamage;
        GameEvents.OnEnemyDefend += SetShield;
        GameEvents.OnEnemyDeath += PlayDeathAni;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerAoeAttack -= TakeDamage;
        GameEvents.OnEnemyDefend -= SetShield;
        GameEvents.OnEnemyDeath -= PlayDeathAni;
    }

    public void InitMonster()
    {
        animator = GetComponent<Animator>();
        runtimeStat.currentHp = monsterData.monsterHp;
        runtimeStat.maxHp = monsterData.monsterMaxHp;
        runtimeStat.currentAtk = monsterData.monsterAtk;

        healthBar.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);
        monsterBattleStat.SetAttackPower(runtimeStat.currentAtk);
    }

    public IEnumerator ExecuteMonsterAction(IHealth target)
    {
        switch (monsterBattleStat.DecideAction())
        {
            case MonsterActionType.Attack:
                yield return new WaitForSeconds(0.5f);  // 살짝 지연시켜서 몬스터들의 공격이 빨리 처리되지 않게 하기 위해서임
                animator.Play("Attack");
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                target.TakeDamage(runtimeStat.currentAtk);
                break;
            case MonsterActionType.Shield:
                yield return new WaitForSeconds(1.0f);
                SetShield(3);
                break;
        }
    }

    private void SetShield(int shieldAmount)
    {
        runtimeStat.currentShield += shieldAmount;
        healthBar.SetShield(runtimeStat.currentShield);
        objectSound.PlayShieldSound(monsterData.shieldClip);
    }

    public void TakeDamage(int damage)
    {
        animator.Play("Take Hit");

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
        healthBar.SetShield(runtimeStat.currentShield);
        runtimeStat.currentHp -= damage;
        healthBar.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);

        if (runtimeStat.currentHp <= 0)
            GameEvents.OnEnemyDeath?.Invoke(this);
    }

    public void PlayDeathAni(Monster monster)
    {
        if (monster != this) return;

        deathSequence = DOTween.Sequence();
        deathSequence.AppendCallback(() => PlayDeathSound())
            .AppendInterval(animator.GetCurrentAnimatorStateInfo(0).length)
            .OnComplete(() => ObjectPoolManager.Instance.ReturnPooledObject(monster));  // 몬스터 사망 애니메이션이 완료된 후 회수
    }

    private void PlayDeathSound()
    {
        animator.Play("Death");
        objectSound.PlayDeathSound(monsterData.deathSound);
    }
}
