using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardUpgradeController : MonoBehaviour
{
    [SerializeField] Button cardUpgradeButton;
    [SerializeField] Text cardNameText;

    private UnityAction cardUpgradeAction;  // 이벤트 중복 등록을 피하기 위한 UnityAction 변수

    private void Start()
    {
        // 임의의 카드 이름이 적힌 버튼을 클릭하면 해당 카드를 1회 강화
        cardUpgradeButton.onClick.RemoveAllListeners();
        cardUpgradeButton.onClick.AddListener(cardUpgradeAction);
    }

    public void OnClickCardUpgrade(CardInstance cardInstance, UnityAction cardUpgradeAction)
    {
        if (this.cardUpgradeAction == null)
            this.cardUpgradeAction = cardUpgradeAction;

        if (cardInstance.isUpgraded == true)
            cardUpgradeButton.interactable = false;
        else
            cardUpgradeButton.interactable = true;

        cardNameText.text = cardInstance.GetCardName();
    }
}
