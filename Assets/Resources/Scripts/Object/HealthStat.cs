using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthStat : MonoBehaviour
{
    [SerializeField] Image healthImage;
    [SerializeField] Text currentHpText;
    [SerializeField] Text maxHpText;
    [SerializeField] GameObject shield;
    [SerializeField] Text currentShieldText;
    [SerializeField] Transform damageTextPos;
    [SerializeField] Text statusText1;
    [SerializeField] Text statusText2;

    private int currentHp;
    private int currentShield;

    public void ResetStatusEffect()
    {
        statusText1.gameObject.SetActive(false);
        statusText2.gameObject.SetActive(false);
    }

    public void SetHealthBar(int currentHp, int maxHp)
    {
        this.currentHp = currentHp;
        currentHpText.text = $"{currentHp}";
        maxHpText.text = $"{maxHp}";
        healthImage.fillAmount = (float)this.currentHp / maxHp;
    }

    public void SetShield(int shieldAmount)
    {
        currentShield = shieldAmount;

        if (currentShield > 0)
        {
            shield.SetActive(true);
            currentShieldText.text = $"{currentShield}";
        }
        else
            shield.SetActive(false);
    }

    public void SetDamageTextTransform(int damage)
    {
        TextPoolManager.Instance.ShowDamageText(damage, damageTextPos);
    }

    public void ActiveStatusEffect(string statusEffectName)
    {
        if (statusText1.text.Contains(statusEffectName))
            statusText1.gameObject.SetActive(true);
        else
            statusText2.gameObject.SetActive(true);
    }

    public void DeactiveStatusEffect(string statusEffectName)
    {
        if (statusText1.text.Contains(statusEffectName))
            statusText1.gameObject.SetActive(false);
        else
            statusText2.gameObject.SetActive(false);
    }
}
