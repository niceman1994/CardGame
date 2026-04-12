using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnEnable()
    {
        GameEvents.OnPlayerAttack += PlayAttackAni;
        GameEvents.OnPlayerDefend += SetShieldFromCard;
        GameEvents.OnPlayerDeath += PlayDeathAni;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerAttack -= PlayAttackAni;
        GameEvents.OnPlayerDefend -= SetShieldFromCard;
        GameEvents.OnPlayerDeath -= PlayDeathAni;
    }

    private void Start()
    {
        InitPlayer();
    }

    public void InitPlayer()
    {
        GameEvents.OnPlayerRegistered.Invoke(this);
        GameEvents.OnTurnStart?.Invoke();

        animator = GetComponent<Animator>();
        runtimeStat.currentShield = 0;
        runtimeStat.currentHp = playerData.playerHp;
        runtimeStat.maxHp = playerData.playerMaxHp;
        healthStat.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);
    }

    private void PlayAttackAni(int attackPower, IHealth target)
    {
        animator.Play("Attack");
        target.TakeDamage(attackPower);
    }

    private void SetShieldFromCard(int shieldAmount)
    {
        runtimeStat.currentShield += shieldAmount;
        healthStat.SetShield(runtimeStat.currentShield);
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
        healthStat.SetShield(runtimeStat.currentShield);
        runtimeStat.currentHp -= damage;
        healthStat.SetHealthBar(runtimeStat.currentHp, runtimeStat.maxHp);

        if (runtimeStat.currentHp <= 0)
            GameEvents.OnPlayerDeath?.Invoke();
    }

    private void PlayDeathAni()
    {
        animator.Play("Death");
    }

    public void AddStatusEffect(StatusEffectData data) { }
}
