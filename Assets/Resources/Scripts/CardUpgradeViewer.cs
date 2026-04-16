using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUpgradeViewer : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] GameObject cardUpgradeUI;
    [SerializeField] Transform upgradeButtonParent;
    [SerializeField] CardUpgradeController cardUpgradeController;

    private List<CardUpgradeController> cardUpgradeControllers = new List<CardUpgradeController>();

    private void Awake()
    {
        GameEvents.OnBattleStart += () => cardUpgradeUI.gameObject.SetActive(false);
        GameEvents.OnBattleEnd += ActiveUpgradeButton;
        MakeUpgradeButton();
        deck.OnClickUpgradeButton += OnClickUpgradeButton;
    }

    private void ActiveUpgradeButton()
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
        {
            int index = i;
            cardUpgradeControllers[i].OnClickCardUpgrade(cardInstances[i], () => deck.UpgradeCard(cardInstances[index]));
        }
    }
}
