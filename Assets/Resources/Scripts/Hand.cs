using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] DiscardPile discardPile;
    [SerializeField] RectTransform canvas;
    [SerializeField] RectTransform cardUseArea;
    [SerializeField] ManaController manaController;

    private List<Card> handCardList = new List<Card>();
    private float handRotateOffset;
    private float cardSpacing;                  // 카드 간격 설정

    // Deck의 Start에서 CardDraw함수가 실행되기 때문에 이벤트 등록을 포함하는 InitHandCard함수는 Awake에서 실행함
    private void Awake()
    {
        InitHandCard();
    }

    private void InitHandCard()
    {
        cardSpacing = 180.0f;
        handRotateOffset = 6000.0f;
        deck.OnHandToCard += GetCardToHand;
        GameEvents.OnCostDown += RandomCostDownInHand;
        GameEvents.OnTurnEnd += DiscardAllCards;
        GameEvents.OnBattleEnd += MoveToDeck;
        // 옵션 팝업과 관련된 함수 등록
        GameEvents.OnOpenPopup += OpenOptionPopup;
        GameEvents.OnClosePopup += CloseOptionPopup;
        GameEvents.OnGameRestart += GameRestart;
    }

    private void GetCardToHand(Card targetCard)
    {
        handCardList.Add(targetCard);
        targetCard.transform.SetParent(transform);
        targetCard.SetCardParentRects(canvas, cardUseArea);
        SetCardPos();

        targetCard.OnRaycastChange += SetOtherCardsRaycastTarget;
        targetCard.OnUsedCard += OnUsedCard;     // 손으로 카드를 가져올 때 카드 사용 관련 함수를 등록함
    }

    private void SetOtherCardsRaycastTarget(Card targetCard, bool raycastTarget)
    {
        for (int i = 0; i < handCardList.Count; i++)
        {
            if (targetCard != handCardList[i])
                handCardList[i].SetCardRaycastTarget(raycastTarget);
        }
    }

    private void RandomCostDownInHand(int costDownAmount)
    {
        int index = UnityEngine.Random.Range(0, handCardList.Count);
        var randomCard = handCardList[index];
        randomCard.CardFront.CardCostDown(costDownAmount);
    }

    private void OnUsedCard(Card usedCard, int cardCost)
    {
        if (manaController.TrySpendMana(cardCost))
        {
            handCardList.Remove(usedCard);
            discardPile.MoveToDiscardPile(usedCard);
            usedCard.ExecuteCard();
            SetCardPos();
            RemoveCardEvent(usedCard);
        }
        else
        {
            usedCard.CancelCard();  // 코스트 부족으로 카드를 쓰지 못하면 제자리로 되돌림
            usedCard.CardSound.PlayInsufficientCostSound();
        }
    }

    private void SetCardPos()
    {
        for (int i = 0; i < handCardList.Count; i++)
        {
            // 보유한 카드 수와 상관없이 간격을 일정하게 유지하면서 중앙을 기준으로 x좌표 계산
            float cardPosX = cardSpacing * (i - (handCardList.Count - 1) * 0.5f);
            // 원의 방정식을 이용해 (r 제곱 - x 제곱)의 제곱근을 y에 대입함
            float cardPosY = Mathf.Sqrt(handRotateOffset * handRotateOffset - cardPosX * cardPosX);
            // x축의 양의 방향 기준으로 Atan2로 각도를 구하기 때문에 90을 뺌
            float cardRotZ = Mathf.Atan2(cardPosY, cardPosX) * Mathf.Rad2Deg - 90;

            // y에 반지름을 뺀 값과 x값을 넣어 손패의 좌표 정렬 함수를 실행함
            handCardList[i].SetCardPos(0.03f * (i + 1), new Vector3(cardPosX, cardPosY - handRotateOffset, 0), Vector3.one, cardRotZ);
        }
    }

    private void RemoveCardEvent(Card card)
    {
        // 카드를 사용하면 등록한 함수를 해제함
        card.OnRaycastChange -= SetOtherCardsRaycastTarget;
        card.OnUsedCard -= OnUsedCard;
    }

    // 턴 종료시 손패 카드를 전부 묘지로 보냄
    private void DiscardAllCards()
    {
        for (int i = 0; i < handCardList.Count; i++)
        {
            RemoveCardEvent(handCardList[i]);
            discardPile.MoveToDiscardPile(handCardList[i]);
        }
        handCardList.Clear();
    }

    // 전투 종료시 손패 카드를 덱으로 되돌림
    private void MoveToDeck()
    {
        for (int i = 0; i < handCardList.Count; i++)
        {
            RemoveCardEvent(handCardList[i]);
            deck.MoveToDeck(handCardList[i]);
        }
        handCardList.Clear();
    }

    private void OpenOptionPopup()
    {
        for (int i = 0; i < handCardList.Count; i++)
            handCardList[i].SetCardState(CardState.InDeck);
    }

    private void CloseOptionPopup()
    {
        for (int i = 0; i < handCardList.Count; i++)
            handCardList[i].SetCardState(CardState.InHand);
    }

    private void GameRestart()
    {
        for (int i = 0; i < handCardList.Count; i++)
        {
            RemoveCardEvent(handCardList[i]);
            handCardList[i].CardTransition(deck.transform, deck.CurrentDeckList, CardState.InDeck);
            handCardList[i].FlipCard(false);
        }
        handCardList.Clear();
    }
}
