using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public enum CardState
{
    InDeck, InHand, Hover, Drag, Used, InDiscardPile
}

// 마우스 상호작용의 주체는 Card 스크립트라서 IPointer 인터페이스를 여기에 추가함
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, ICard
{
    [SerializeField] Draw draw;
    [SerializeField] CardView cardView;
    [SerializeField] CardSound cardSound;
    [SerializeField] CardInteraction cardInteraction;

    private CardState handCardState;
    private CardInstance cardInstance;
    private UnityAction<CardGameData> upgradeTextAction;

    public event Action<Card, bool> OnIsHoverChange;
    public event Action<Card, int> OnUsedCard;
    public event Action<Color> OnResetCostColor;

    public CardSound CardSound => cardSound;

    private void Awake()
    {
        cardInteraction.Init(this);
        SetCardState(CardState.InDeck);
        upgradeTextAction = (data) => UpgradeCardText(data.CardInstance);
        OnResetCostColor += cardView.ResetCardCostColor;
    }

    private void OnEnable()
    {
        EventBus<CardGameData>.Subscribe(GameEventType.CARD_TEXT_UPGRADE, upgradeTextAction);   // 버튼으로 강화된 카드의 텍스트를 갱신하기 위해 이벤트에 등록
    }

    private void OnDisable()
    {
        EventBus<CardGameData>.Unsubscribe(GameEventType.CARD_TEXT_UPGRADE, upgradeTextAction);
    }

    public void SetCardData(CardInstance cardInstance)
    {
        this.cardInstance = cardInstance;
        cardView.SetCardText(this.cardInstance);
    }

    private void UpgradeCardText(CardInstance cardInstance)
    {
        if (this.cardInstance != cardInstance) return;

        cardView.SetCardText(this.cardInstance);
    }

    public void ResetCardText()
    {
        cardInstance.isOverload = false;
        cardView.ResetCardInstance(cardInstance);
        OnResetCostColor?.Invoke(Color.white);
    }

    public void ApplyCardOverload(int overloadCost)
    {
        cardInstance.isOverload = true;
        cardView.OverloadCardText(cardInstance, overloadCost);
        cardInstance.isOverload = false;                // 1회성 강화라서 false를 다시 넣음
    }

    public void SetCardPos(float drawDelay, Vector3 startPos, Vector3 endScale, float cardRotateZ)
    {
        cardInteraction.SetCardOriginPos(startPos);

        draw.DrawSequence(drawDelay, startPos, endScale)
            .AppendCallback(() => CheckAlreadyInHand(draw.IsDraw, cardRotateZ))
            .OnComplete(() => cardInteraction.SetCardOriginIndex(transform.GetSiblingIndex()));
    }

    private void CheckAlreadyInHand(bool isDraw, float cardRotateZ)
    {
        Sequence cardDrawSequence = DOTween.Sequence();
        
        // 이미 드로우한 카드의 시퀀스 재실행을 방지하기 위한 코드
        if (isDraw == false)
        {
            cardDrawSequence.Join(transform.DORotate(new Vector3(0, 90, transform.localRotation.z), 0.2f).SetEase(Ease.InOutCubic))
                .Append(transform.DORotate(new Vector3(0, 0, cardRotateZ), 0.2f).SetEase(Ease.InOutCubic))
                .JoinCallback(() =>
                {
                    FlipCard(true);
                    cardSound.PlayDrawSound();
                    draw.SetIsDraw(true);
                });
        }
        else
            cardDrawSequence.Join(transform.DORotate(new Vector3(0, 0, cardRotateZ), 0.01f).SetEase(Ease.InOutCubic));

        cardDrawSequence.OnComplete(() =>
        {
            cardInteraction.SetHover(true);
            SetCardState(CardState.InHand);
            cardInteraction.SetCardOriginRotate(transform.localRotation.eulerAngles);
        });
    }

    public Sequence CardShuffle(float delay)
    {
        Sequence deckCardShuffleSequence = cardInteraction.SetShffulePos(delay, handCardState);
        return deckCardShuffleSequence;
    }

    public void FlipCard(bool isFlip)
    {
        cardInteraction.SetFlip(isFlip);
        cardView.gameObject.SetActive(isFlip);
    }

    public void SetCardState(CardState cardState)
    {
        handCardState = cardState;
    }

    // 손패->묘지, 묘지->덱으로 카드가 이동할 때 사용하는 시퀀스
    public Sequence CardTransitionSequence(Transform parent, List<Card> cardZoneList, CardState cardState)
    {
        Sequence cardTransitionSequence = DOTween.Sequence();
        cardTransitionSequence.AppendCallback(() => CardTransition(parent, cardZoneList, cardState))
            .Append(transform.DOScale(Vector3.one, 0.2f))
            .Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), 0.2f));

        return cardTransitionSequence;
    }

    // 시퀀스없이 게임을 재시작할 때 사용하는 함수
    public void CardTransition(Transform parent, List<Card> cardZoneList, CardState cardState)
    {
        transform.SetParent(parent);
        cardZoneList.Add(this);
        draw.SetIsDraw(false);
        SetCardState(cardState);
    }

    public void ResetCardOriginPos()
    {
        cardInteraction.SetCardParentInArea();
        cardInteraction.SetHover(true);
        cardInteraction.CardResetSequence();  // 여러 카드를 빠르게 쓸 때 일부 카드가 마우스를 따라가지 않는 현상을 방지하기 위한 코드
    }

    public void SetIsHover(bool isActive)
    {
        cardInteraction.SetHover(isActive);
    }

    public void SetParentHandCard(Hand hand, RectTransform parentRect, RectTransform cardAreaRect)
    {
        transform.SetParent(hand.transform);
        cardInteraction.SetCardParentRects(parentRect, cardAreaRect);
    }

    public void CardCostChange(int costChangeAmount)
    {
        cardView.CardCostChange(costChangeAmount);
    }

    public void ExecuteCard()
    {
        cardInstance.Execute(cardInteraction.ArrowTaraget);

        if (cardView.IsCostReset())
            OnResetCostColor?.Invoke(Color.white);
    }

    public void CancelCard()
    {
        SetCardState(CardState.InHand);
        ResetCardOriginPos();
        OnIsHoverChange?.Invoke(this, true);
    }

    #region Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handCardState == CardState.InDeck || handCardState == CardState.InDiscardPile) return;
        if (cardInteraction.IsHover == false) return;
        // 드래그 시 cardEdgeImage의 Raycast 비활성화 + 화살표(ArrowHeadImage)가 Raycast를 잡아서 Pointer 대상이 변경되므로 무시
        if (handCardState == CardState.Drag) return;
        
        SetCardState(CardState.Hover);
        // 카드 위에 마우스가 있다면 해당 카드를 위로 조금 이동시키고 크기를 키우면서 다른 카드에 의해 텍스트가 가려지지 않게 하는 시퀀스
        cardInteraction.CardHoverSequence();
        cardSound.PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (handCardState == CardState.Hover)
        {
            SetCardState(CardState.InHand);
            ResetCardOriginPos();
        }
    }
    #endregion

    #region Drag&Use
    public void OnPointerDown(PointerEventData eventData)
    {
        if (handCardState == CardState.Hover)
        {
            SetCardState(CardState.Drag);
            cardInteraction.PointerDown(cardInstance);
            OnIsHoverChange?.Invoke(this, false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (handCardState == CardState.Drag)
            cardInteraction.CardDrag(eventData, cardInstance);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handCardState == CardState.InDiscardPile) return;

        if (handCardState == CardState.Drag)
        {
            cardInteraction.SetCardParentInArea();

            if (cardInteraction.ShouldUseCard(eventData, cardInstance))
            {
                if (cardInstance.currentCardData.IsValidTarget(cardInteraction.ArrowTaraget))   // 카드가 유효한 대상을 찾았을 경우
                    SetCardState(CardState.Used);
                else                                                                            // 카드가 유효한 대상을 찾지 못했을 경우
                    CancelCard();
            }
            else
                CancelCard();
        }

        if (handCardState == CardState.Used)
            OnUsedCard?.Invoke(this, cardView.CardCost);     // 현재 카드 코스트를 기준으로 마나를 소모함

        cardInteraction.DeactiveCardArrow();
    }
    #endregion
}
