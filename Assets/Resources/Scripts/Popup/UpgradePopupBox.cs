using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePopupBox : PopupBox
{
    private CardInstance cardInstance;

    public event Action OnOpen;
    public event Action<CardInstance> OnConfirm;
    public event Action OnClose;

    private void Start()
    {
        checkButton.onClick.AddListener(OnClickConfirmButton);
        xButton.onClick.AddListener(OnClickCloseButton);
    }

    public void GetCardInstanceInfo(CardInstance cardInstance)
    {
        if (!gameObject.activeInHierarchy)
            this.cardInstance = cardInstance;
    }

    public override void OpenPopup(string popupText)
    {
        OnOpen?.Invoke();
        gameObject.SetActive(true);
        questionText.text = $"{popupText}";
    }

    protected override void OnClickConfirmButton()
    {
        OnConfirm?.Invoke(cardInstance);
        gameObject.SetActive(false);
        GameEvents.OnBattleStart?.Invoke();
        GameEvents.OnTurnStart?.Invoke();
        ObjectPoolManager.Instance.SetMonsters();
    }

    protected override void OnClickCloseButton()
    {
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }
}
