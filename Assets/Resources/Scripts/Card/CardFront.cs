using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardFront : MonoBehaviour
{
    [SerializeField] CardData cardData;
    [SerializeField] Image cardFrontImage;
    [SerializeField] Text cardCost;
    [SerializeField] Text cardName;
    [SerializeField] Text description;

    private int displayCardCost;
    private Sequence costChangeSequence;
    private Vector3 costTextLocalScale;

    public int CardCost => displayCardCost;

    public void SetCardText(CardInstance cardInstance)
    {
        cardData = cardInstance.cardData;
        cardFrontImage.sprite = cardInstance.cardData.cardFrontImage;
        displayCardCost = cardData.GetCardCost(cardInstance);
        cardCost.text = $"{displayCardCost}";
        cardName.text = cardInstance.GetCardName();
        description.text = cardData.GetDescription(cardInstance);
    }

    public void CardCostDown(int costDownAmount)
    {
        costTextLocalScale = cardCost.transform.localScale;
        displayCardCost -= costDownAmount;

        if (displayCardCost <= 0) displayCardCost = 0;

        // 어떤 카드의 코스트가 줄었는지 보여주기 위해 사용하는 시퀀스
        costChangeSequence = DOTween.Sequence();
        costChangeSequence.Append(cardCost.transform.DOScale(6.0f, 0.1f))
            .Append(cardCost.transform.DOScale(costTextLocalScale, 0.1f))
            .OnComplete(() => cardCost.text = $"{displayCardCost}");
    }
}
