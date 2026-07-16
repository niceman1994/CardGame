using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStat : MonoBehaviour
{
    [SerializeField] Image healthImage;
    [SerializeField] Text currentHpText;
    [SerializeField] Text maxHpText;
    [SerializeField] GameObject shield;
    [SerializeField] Text currentShieldText;
    [SerializeField] Transform damageTextPos;
    [SerializeField] List<TextMeshProUGUI> statusTexts = new List<TextMeshProUGUI>();

    private int currentHp;
    private int currentShield;
    private List<string> activeStatusEffects = new List<string>();      // 활성화된 상태이상 문자열을 가진 리스트

    public void ResetStatusEffect()
    {
        for (int i = 0; i < statusTexts.Count; i++)
            statusTexts[i].text = "";
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
        DamageTextManager.Instance.ShowDamageText(damage, damageTextPos);
    }

    public void ActiveStatusEffect(string statusEffectName)
    {
        activeStatusEffects.Add(statusEffectName);

        for (int i = 0; i < activeStatusEffects.Count; i++)
        {
            statusTexts[i].gameObject.SetActive(true);
            statusTexts[i].SetText(activeStatusEffects[i]);
        }
    }
    
    public void DeactiveStatusEffect(string statusEffectName)
    {
        activeStatusEffects.Remove(statusEffectName);
    }

    public void RefreshStatusEffectText()
    {
        for (int i = 0; i < statusTexts.Count; i++)
        {
            statusTexts[i].text = "";

            if (i <= activeStatusEffects.Count - 1)
                statusTexts[i].SetText(activeStatusEffects[i]);
        }
    }
}