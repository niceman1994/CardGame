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

    public CardType GetCardType => cardData.cardType;
    public int CardCost => int.Parse(cardCost.text);

    private void Start()
    {
        GetCardData();
    }

    private void GetCardData()
    {
        cardFrontImage.sprite = cardData.cardFrontImage;
        cardCost.text = $"{cardData.cardCost}";
        cardName.text = cardData.cardName;
        description.text = cardData.description;
    }

    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
    }

    public void Execute(IHealth target)
    {
        cardData.Execute(target);
    }
}
