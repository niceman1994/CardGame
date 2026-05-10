using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[System.Serializable]
public class CardEntry
{
    public CardData cardData;
    public int cardCount;
}

public class CardInstance
{
    public bool isUpgraded;
    public CardData cardData;
    public StatusEffectData statusEffectData;   // 상태이상 데이터

    public string GetCardName()
    {
        return cardData.GetCardName(this);
    }

    public void Execute(IHealth target)
    {
        cardData.Execute(this, target);
    }
}

public class Deck : MonoBehaviour
{
    [SerializeField] List<CardEntry> cardDatas = new List<CardEntry>();
    [SerializeField] Card deckCardPrefab;
    [SerializeField] AudioSource deckShuffleSound;
    [SerializeField] int drawCardCount;

    private List<Card> currnetDeckList = new List<Card>();
    private List<CardInstance> cardInstances = new List<CardInstance>();
    private Sequence deckShuffleSequence;

    public List<Card> CurrentDeckList => currnetDeckList;
    public int CardCount => cardInstances.Count;

    public event Action<Card> OnHandToCard;
    public event Action<List<CardInstance>> OnClickUpgradeButton;

    private void Awake()
    {
        InitCard();
    }

    private void Start()
    {
        InitCardDraw();
    }

    private void InitCard()
    {
        for (int i = 0; i < cardDatas.Count; i++)
        {
            for (int j = 0; j < cardDatas[i].cardCount; j++)
            {
                cardInstances.Add(new CardInstance
                {
                    cardData = cardDatas[i].cardData,
                    statusEffectData = cardDatas[i].cardData.cardType == CardType.Attack ? (cardDatas[i].cardData as AttackCardData).cardSideEffect.statusEffect : null,
                    isUpgraded = false
                }) ;
            }
        }
    }

    private void InitCardDraw()
    {
        MakeCard();
        GameEvents.OnCardDraw += CardDraw;              // 턴을 시작할 때 드로우하는 함수
        GameEvents.OnExtraCardDraw += AddCardToHand;    // 카드를 사용해 드로우하는 함수
        GameEvents.OnGameRestart += () => StartCoroutine(GameRestart());
    }

    private void MakeCard()
    {
        for (int i = 0; i < cardInstances.Count; i++)
        {
            Card cardGameobject = Instantiate(deckCardPrefab, transform);
            cardGameobject.SetCardData(cardInstances[i]);
            cardGameobject.name = $"{cardInstances[i].GetCardName()}_{i}";
            currnetDeckList.Add(cardGameobject);
        }

        // 카드 생성 후 덱에서 무작위로 드로우되도록 카드를 섞음
        for (int i = currnetDeckList.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card tempCard = currnetDeckList[i];
            currnetDeckList[i] = currnetDeckList[randomIndex];
            currnetDeckList[randomIndex] = tempCard;
        }
        ResetDeckCardPos();
    }

    private void CardDraw()
    {
        OnClickUpgradeButton?.Invoke(cardInstances);

        // 드로우할 카드가 현재 덱의 카드 수보다 많으면 그 수만큼만 드로우되게 함
        if (drawCardCount > currnetDeckList.Count && currnetDeckList.Count != 0)
            drawCardCount = currnetDeckList.Count;

        AddCardToHand(drawCardCount);
    }

    private void AddCardToHand(int drawCardCount)
    {
        for (int i = 0; i < drawCardCount; i++)
        {
            // 덱의 카드 수가 0이면 묘지의 카드를 덱으로 되돌림
            if (currnetDeckList.Count <= 0)
                GameEvents.OnReturnToDeck?.Invoke();

            // 덱 맨 위부터 드로우하기 때문에 리스트의 마지막 요소부터 시작함
            var targetCard = currnetDeckList[currnetDeckList.Count - 1];
            currnetDeckList.Remove(targetCard);
            OnHandToCard?.Invoke(targetCard);
        }
    }

    public void DeckShuffle()
    {
        if (currnetDeckList.Count == 0)
        {
            Debug.LogError("덱에 카드가 없어 셔플할 수 없습니다!");
            return;
        }
        deckShuffleSequence = DOTween.Sequence();

        for (int i = currnetDeckList.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card tempCard = currnetDeckList[i];
            currnetDeckList[i] = currnetDeckList[randomIndex];
            currnetDeckList[randomIndex] = tempCard;

            deckShuffleSequence.Join(currnetDeckList[i].CardShuffle(0.01f * i));
        }
        deckShuffleSound.Play();
        // ResetDeckCardPos 함수의 호출 타이밍을 뒤로 미루기 위해 OnComplete를 사용함
        deckShuffleSequence.OnComplete(() => ResetDeckCardPos()); 
    }

    private void ResetDeckCardPos()
    {
        for (int i = 0; i < currnetDeckList.Count; i++)
        {
            currnetDeckList[i].transform.localScale = Vector3.one;
            currnetDeckList[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
            currnetDeckList[i].transform.localPosition = new Vector3(0, 0, i);
            // 덱을 셔플했을 때 카드가 이상하게 배치되는 현상을 발견함
            // z 값을 기준으로 UI를 렌더링하기 때문에 SetSiblingIndex 로 무작위로 섞은 카드와 하이어라키의 구조를 같게 해줘야함
            currnetDeckList[i].transform.SetSiblingIndex(i);
        }
    }

    public Sequence MoveToDeck(Card returnCard)
    {
        return returnCard.CardTransitionSequence(transform, currnetDeckList, CardState.InDeck)
            .JoinCallback(() => returnCard.FlipCard(false));
    }

    public void UpgradeCard(CardInstance cardInstance)
    {
        cardInstance.isUpgraded = true;
        GameEvents.OnUpdateCardText?.Invoke(cardInstance);
    }

    private IEnumerator GameRestart()
    {
        yield return new WaitUntil(() => currnetDeckList.Count == cardInstances.Count);
        DeckShuffle();
        yield return new WaitUntil(() => deckShuffleSequence.IsPlaying());
        ObjectPoolManager.Instance.SetMonsters();
        GameEvents.OnBattleStart?.Invoke();
        GameEvents.OnTurnStart?.Invoke();
    }
}
