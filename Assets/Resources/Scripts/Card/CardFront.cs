using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFront : MonoBehaviour
{
    [SerializeField] CardData cardData;
    [SerializeField] Image cardFrontImage;
    [SerializeField] Text cardCost;
    [SerializeField] Text cardName;
    [SerializeField] Text description;

    private void Start()
    {
        SetCardData();
    }

    private void SetCardData()
    {
        cardFrontImage.sprite = cardData.cardFrontImage;
        cardCost.text = $"Cost : {cardData.cardCost}";
        cardName.text = cardData.cardName;
        description.text = cardData.description;
    }
}
