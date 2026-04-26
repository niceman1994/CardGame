using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPopupBox : PopupBox
{
    public event Action OnOpen;
    public event Action OnConfirm;
    public event Action OnClose;

    private void Start()
    {
        checkButton.onClick.AddListener(OnClickConfirmButton);
        xButton.onClick.AddListener(OnClickCloseButton);
    }

    public override void OpenPopup(string popupText)
    {
        OnOpen?.Invoke();
        gameObject.SetActive(true);
        questionText.text = $"{popupText}";
    }

    protected override void OnClickConfirmButton()
    {
        OnConfirm?.Invoke();
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
