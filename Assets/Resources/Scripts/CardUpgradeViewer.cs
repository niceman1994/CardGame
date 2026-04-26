using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUpgradeViewer : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] GameObject cardUpgradeUI;
    [SerializeField] Transform upgradeButtonParent;
    [SerializeField] CardUpgradeController cardUpgradeController;
    [SerializeField] UpgradePopupBox upgradeQuestionPopup;

    private List<CardUpgradeController> cardUpgradeControllers = new List<CardUpgradeController>();

    private void Awake()
    {
        GameEvents.OnBattleStart += () => cardUpgradeUI.gameObject.SetActive(false);
        InitPopup();
        MakeUpgradeButton();
        deck.OnClickUpgradeButton += OnClickUpgradeButton;
    }

    // 강화 여부를 묻는 팝업이 나왔을 때 호출할 이벤트를 담은 함수
    private void InitPopup()
    {
        upgradeQuestionPopup.OnOpen += () => cardUpgradeControllers.ForEach(x => x.SetRaycastTarget(false));
        upgradeQuestionPopup.OnConfirm += OnCofirmCardUpgrade;
        upgradeQuestionPopup.OnClose += () => cardUpgradeControllers.ForEach(x => x.SetRaycastTarget(true));
    }

    public void ActiveUpgradeButton()
    {
        cardUpgradeUI.gameObject.SetActive(true);
        cardUpgradeControllers.ForEach(x => x.gameObject.SetActive(true));
    }

    private void MakeUpgradeButton()
    {
        for (int i = 0; i < deck.CardCount; i++)
        {
            CardUpgradeController cardUpgrade = Instantiate(cardUpgradeController, upgradeButtonParent);
            cardUpgrade.name = $"CardUpgradeButton_{i}";
            cardUpgradeControllers.Add(cardUpgrade);
        }
    }

    private void OnClickUpgradeButton(List<CardInstance> cardInstances)
    {
        for (int i = 0; i < cardInstances.Count; i++)
            cardUpgradeControllers[i].OnClickCardUpgrade(cardInstances[i], upgradeQuestionPopup);
    }

    public void OnCofirmCardUpgrade(CardInstance cardInstance)
    {
        deck.UpgradeCard(cardInstance);
    }
}
