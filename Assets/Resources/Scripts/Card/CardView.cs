using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] CardData cardData;
    [SerializeField] Image cardFrontImage;
    [SerializeField] Text cardCost;
    [SerializeField] Text cardName;
    [SerializeField] Text description;

    private bool isCostUp;
    private bool isCostDown;
    private int displayCardCost;
    private Sequence costChangeSequence;
    private Vector3 costTextDefaultScale;

    public int CardCost => displayCardCost;

    public void SetCardText(CardInstance cardInstance)
    {
        cardData = cardInstance.CurrentCardData;
        SetCardView(cardInstance);
    }

    public void OverloadCardText(CardInstance cardInstance, int costChangeAmount)
    {
        CardCostChange(costChangeAmount);
        description.text = cardData.GetDescription(cardInstance);
    }

    public void ResetCardInstance(CardInstance cardInstance)
    {
        cardData = cardInstance.OriginalCardData;
        SetCardView(cardInstance);
    }

    private void SetCardView(CardInstance cardInstance)
    {
        SetCardCost(cardInstance);
        cardFrontImage.sprite = cardData.cardFrontImage;
        cardName.text = cardInstance.GetCardName();
        description.text = cardData.GetDescription(cardInstance);
    }

    private void SetCardCost(CardInstance cardInstance)
    {
        displayCardCost = cardData.GetCardCost(cardInstance);
        cardCost.text = $"{displayCardCost}";
    }

    public void CardCostChange(int costChangeAmount)
    {
        if (costChangeAmount < 0)
        {
            isCostDown = true;
            isCostUp = false;
        }
        else if (costChangeAmount > 0)
        {
            isCostUp = true;
            isCostDown = false;
        }
        
        costTextDefaultScale = cardCost.transform.localScale;
        displayCardCost += costChangeAmount;

        costChangeSequence.Complete();
        // 어떤 카드의 코스트가 줄었는지 보여주기 위해 사용하는 시퀀스
        costChangeSequence = DOTween.Sequence();
        costChangeSequence.Append(cardCost.transform.DOScale(6.0f, 0.1f))
            .Append(cardCost.transform.DOScale(costTextDefaultScale, 0.1f))
            .OnComplete(() => ChangeCostColor());
    }

    private void ChangeCostColor()
    {
        if (isCostDown == true)
        {
            if (displayCardCost < 0)
            {
                displayCardCost = 0;
                cardCost.color = Color.white;
            }
            else
                cardCost.color = Color.green;
        }
        else if (isCostUp == true)
            cardCost.color = Color.red;
        
        cardCost.text = $"{displayCardCost}";
    }

    public void ResetCardCostColor(Color textColor)
    {
        cardCost.color = textColor;
        isCostDown = false;
        isCostUp = false;
    }

    public bool IsCostReset()
    {
        return isCostUp == false && isCostDown == false;
    }
}
