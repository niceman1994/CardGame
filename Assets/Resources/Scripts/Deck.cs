using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardEntry
{
    public CardData cardData;
    public int cardCount;

    public CardEntry(CardData cardData, int cardCount)
    {
        this.cardData = cardData;
        this.cardCount = cardCount;
    }
}

public class Deck : MonoBehaviour
{
    [SerializeField] Card deckCardPrefab;
    [SerializeField] AudioSource deckShuffleSound;
    [SerializeField] int drawCardCount;

    private bool isCardInit;
    private Dictionary<string, CardJsonData> cardJsonData = new Dictionary<string, CardJsonData>();
    private Dictionary<string, StatusEffectJsonData> statusEffectDatas = new Dictionary<string, StatusEffectJsonData>();
    private List<CardEntry> cardEntrys = new List<CardEntry>();
    private List<CardInstance> cardInstances = new List<CardInstance>();
    private List<Card> currnetDeckList = new List<Card>();
    private Sequence deckShuffleSequence;

    public bool IsCardInit => isCardInit;
    public List<Card> CurrentDeckList => currnetDeckList;
    public int CardCount => cardInstances.Count;

    public event Action<Card> OnCardDraw;
    public event Action<List<CardInstance>> OnClickUpgradeButton;

    private void Awake()
    {
        InitCardData();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isCardInit);
        MakeCard();
    }

    private void InitCardData()
    {
        // 상태이상 데이터 json 파일의 Key와 카드 데이터 json 파일의 StatusEffect 값을 일치시켜 데이터 매핑 용도로 사용
        Addressables.LoadAssetAsync<TextAsset>("StatusEffectDatas").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                StatusEffectJsonList data = JsonConvert.DeserializeObject<StatusEffectJsonList>(handle.Result.text);
                statusEffectDatas = data.statusEffects;
            }
        };
        Addressables.LoadAssetAsync<TextAsset>("CardDatas").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CardJsonList data = JsonConvert.DeserializeObject<CardJsonList>(handle.Result.text);
                cardJsonData = data.cards;
                
                LoadCardImages();
            }
        };
    }

    private void LoadCardImages()
    {
        int completeCount = 0;
        var cardJsonKeys = cardJsonData.Keys.ToList();
        var cardJsonValues = cardJsonData.Values.ToList();

        for (int i = 0; i < cardJsonData.Count; i++)
        {
            int index = i;      // 클로저 문제를 해결하기 위한 int 변수(for문 위에 두면 같은 이미지를 넣기 때문에 안쪽에 변수를 뒀음)
            Addressables.LoadAssetAsync<Sprite>(cardJsonValues[index].spriteName).Completed += (handle) =>
            {
                CardSideEffect cardSideEffect = new CardSideEffect(cardJsonValues[index].cardSideEffect);
                
                if (statusEffectDatas.ContainsKey(cardJsonValues[index].cardSideEffect.statusEffect))
                {
                    string effectName = cardJsonValues[index].cardSideEffect.statusEffect;
                    StatusEffectData effectData = StatusEffectFactory.GetStatusEffect(effectName);
                    effectData.CreateStatusEffectData(statusEffectDatas[effectName]);
                    cardSideEffect.CreateStatusEffect(effectData);
                }
                
                CardData cardData = CardDataFactory.GetCard(cardJsonKeys[index]);
                cardData.CreateCardData(cardJsonValues[index], handle.Result, cardSideEffect);
                cardEntrys.Add(new CardEntry(cardData, cardJsonValues[index].cardCount));

                ++completeCount;
                // 필요한 카드 데이터가 다 들어갔을 때 카드를 생성함
                if (completeCount == cardJsonData.Keys.Count)
                    InitCardEntry();
            };
        }
    }

    private void InitCardEntry()
    {
        for (int i = 0; i < cardEntrys.Count; i++)
        {
            for (int j = 0; j < cardEntrys[i].cardCount; j++)
                cardInstances.Add(new CardInstance(false, cardEntrys[i].cardData));
        }
        isCardInit = true;
    }

    private void MakeCard()
    {
        EventBus.Subscribe(GameEventType.CARD_DRAW, CardDraw);                                                  // 턴을 시작할 때 드로우하는 함수
        EventBus<CardGameData>.Subscribe(GameEventType.CARD_DRAW, (data) => AddCardToHand(data.Value));         // 카드를 사용해 드로우하는 함수
        EventBus.Subscribe(GameEventType.RESTART, () => StartCoroutine(GameRestart()));

        for (int i = 0; i < cardInstances.Count; i++)
        {
            Card cardGameobject = Instantiate(deckCardPrefab, transform);
            cardGameobject.SetCardData(cardInstances[i]);
            cardGameobject.name = cardInstances[i].GetCardName();
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
                EventBus.Publish(GameEventType.RETURN_TO_DECK);

            // 덱 맨 위부터 드로우하기 때문에 리스트의 마지막 요소부터 시작함
            var targetCard = currnetDeckList[currnetDeckList.Count - 1];
            currnetDeckList.Remove(targetCard);
            OnCardDraw?.Invoke(targetCard);
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
        cardInstance.SetCardUpgrade();
        EventBus<CardGameData>.Publish(GameEventType.CARD_TEXT_UPGRADE, new CardGameData { CardInstance = cardInstance });
    }

    private IEnumerator GameRestart()
    {
        yield return new WaitUntil(() => currnetDeckList.Count == cardInstances.Count);
        DeckShuffle();
        yield return new WaitUntil(() => deckShuffleSequence.IsPlaying());
        ObjectPoolManager.Instance.SetMonsters();
        EventBus.Publish(GameEventType.BATTLE_START);
        EventBus.Publish(GameEventType.TURN_START);
    }
}
