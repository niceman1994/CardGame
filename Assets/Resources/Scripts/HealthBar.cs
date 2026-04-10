using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthImage;
    [SerializeField] Text currentHpText;
    [SerializeField] Text maxHpText;
    [SerializeField] GameObject shield;
    [SerializeField] Text currentShieldText;

    private int currentHp;
    private int currentShield;

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
}
