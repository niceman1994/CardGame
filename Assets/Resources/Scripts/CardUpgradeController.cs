using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardUpgradeController : MonoBehaviour
{
    [SerializeField] Image cardUpgradeBackground;
    [SerializeField] Button cardUpgradeButton;
    [SerializeField] Text cardNameText;

    private UpgradePopupBox upgradeQuestionPopup;
    private CardInstance cardInstance;

    private void Start()
    {
        // 임의의 카드 이름이 적힌 버튼을 클릭하면 해당 카드를 1회 강화
        cardUpgradeButton.onClick.AddListener(OnClickUpgradeButton);
    }

    public void OnClickCardUpgrade(CardInstance cardInstance, UpgradePopupBox upgradeQuestionPopup)
    {
        if (this.upgradeQuestionPopup == null)
            this.upgradeQuestionPopup = upgradeQuestionPopup;

        this.cardInstance = cardInstance;
        cardUpgradeBackground.raycastTarget = true;

        if (cardInstance.isUpgraded == true)
            cardUpgradeButton.interactable = false;
        else
            cardUpgradeButton.interactable = true;

        cardNameText.text = cardInstance.GetCardName();
    }

    public void SetRaycastTarget(bool raycastTarget)
    {
        cardUpgradeBackground.raycastTarget = raycastTarget;
    }

    private void OnClickUpgradeButton()
    {
        string popupText = $"({this.cardInstance.GetCardName()}) 카드를 강화하시겠습니까?";
        upgradeQuestionPopup.GetCardInstanceInfo(cardInstance);
        upgradeQuestionPopup.OpenPopup(popupText);
    }
}
