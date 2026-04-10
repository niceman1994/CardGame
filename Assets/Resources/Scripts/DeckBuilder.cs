using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEntry
{
    public CardData cardData;
    public int cardCount;
}

public class DeckBuilder : Singleton<DeckBuilder>
{
    [SerializeField] List<CardEntry> cardDatas = new List<CardEntry>();
    [SerializeField] Deck deck;
    [SerializeField] Card deckCardPrefab;

    // Deck의 Start에서 CardDraw함수가 실행되기 때문에 오류가 나지 않기 위해 Awake에서 실행함
    protected override void Awake()
    {
        base.Awake();
        deck.OnAddCard += MakeCard;
    }

    private void MakeCard(List<Card> currentDeckList, Transform parent)
    {
        for (int i = 0; i < cardDatas.Count; i++)
        {
            for (int j = 0; j < cardDatas[i].cardCount; j++)
            {
                Card cardGameobject = Instantiate(deckCardPrefab, parent);
                cardGameobject.SetCardData(cardDatas[i].cardData, j);

                currentDeckList.Add(cardGameobject);
            } 
        }
        InitDeckShuffle(currentDeckList);
    }

    private void InitDeckShuffle(List<Card> currentDeckList)
    {
        for (int i = currentDeckList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card tempCard = currentDeckList[i];
            currentDeckList[i] = currentDeckList[randomIndex];
            currentDeckList[randomIndex] = tempCard;
        }
    }
}
