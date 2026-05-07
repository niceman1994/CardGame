using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum CardState
{
    InDeck, InHand, Hover, Drag, Used, InDiscardPile
}

public struct MouseInCard
{
    public int cardOriginIndex;
    public Vector3 shufflePos;
    public Vector3 cardOriginPos;
    public Vector3 cardOriginScale;
    public Vector3 cardOriginRotate;
}

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] Image cardEdgeImage;
    [SerializeField] CardFront cardFront;
    [SerializeField] CardArrow cardArrow;
    [SerializeField] Image cardBackImage;
    [SerializeField] CardSound cardSound;

    private Draw draw;
    private CardState handCardState;
    private MouseInCard mouseInCard;
    private RectTransform parentRectTransform;      // 마우스 위치에 따라 카드를 따라가게 만들기 위한 RectTransform 변수
    private RectTransform cardAreaRectTransform;
    private CardInstance cardInstance;

    private Sequence cardHoverSequence;
    private Sequence cardResetSequence;

    public event Action<Card, bool> OnRaycastChange;
    public event Action<Card, int> OnUsedCard;

    public CardFront CardFront => cardFront;
    public CardSound CardSound => cardSound;

    // Deck의 Start에서 CardDraw함수가 실행되기 때문에 오류가 나지 않기 위해 Awake에서 실행함
    private void Awake()
    {
        draw = new Draw(transform);
        SetCardState(CardState.InDeck);
        mouseInCard.cardOriginScale = transform.localScale;
        GameEvents.OnUpdateCardText += UpdateCardText;   // 강화한 카드의 텍스트를 갱신하기 위해 이벤트에 등록
    }

    public void SetCardData(CardInstance cardInstance)
    {
        this.cardInstance = cardInstance;
        cardFront.SetCardText(this.cardInstance);
    }

    public void UpdateCardText(CardInstance cardInstance)
    {
        if (this.cardInstance != cardInstance) return;

        cardFront.SetCardText(cardInstance);
        name = $"{cardInstance.GetCardName()}";
    }

    public void SetCardPos(float drawDelay, Vector3 startPos, Vector3 endScale, float cardRotateZ)
    {
        mouseInCard.cardOriginPos = startPos;
        cardEdgeImage.raycastTarget = false;

        draw.DrawSequence(drawDelay, startPos, endScale)
            .AppendCallback(() => CheckAlreadyInHand(draw.IsDraw, cardRotateZ))
            .OnComplete(() =>
            {
                cardEdgeImage.raycastTarget = true;
                mouseInCard.cardOriginIndex = transform.GetSiblingIndex();
            });
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
            SetCardState(CardState.InHand);
            mouseInCard.cardOriginRotate = transform.localRotation.eulerAngles;
        });
    }

    public Sequence CardShuffle(float delay)
    {
        mouseInCard.shufflePos = new Vector3(0, cardEdgeImage.rectTransform.rect.height, transform.localPosition.z);

        Sequence deckCardShuffleSequence = DOTween.Sequence();
        deckCardShuffleSequence.SetDelay(delay).Append(transform.DOLocalMove(mouseInCard.shufflePos, 0.08f).SetEase(Ease.InOutCubic))
            .Append(transform.DOLocalMove(new Vector3(0, 0, transform.localPosition.z), 0.08f).SetEase(Ease.InOutCubic))
            .JoinCallback(() => SetCardState(CardState.InDeck));

        return deckCardShuffleSequence;
    }

    public void FlipCard(bool isFlip)
    {
        cardEdgeImage.gameObject.SetActive(isFlip);
        cardFront.gameObject.SetActive(isFlip);
        cardBackImage.gameObject.SetActive(!isFlip);
    }

    public void SetCardState(CardState cardState)
    {
        handCardState = cardState;
    }

    public void SetCardParentRects(RectTransform parentRect, RectTransform cardAreaRect)
    {
        parentRectTransform = parentRect;
        cardAreaRectTransform = cardAreaRect;
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

    private void ResetCardOriginPos()
    {
        transform.SetParent(cardAreaRectTransform);
        cardEdgeImage.raycastTarget = false;

        cardHoverSequence.Kill();
        cardResetSequence = DOTween.Sequence();
        cardResetSequence.JoinCallback(() => transform.SetSiblingIndex(mouseInCard.cardOriginIndex))
            .Append(transform.DOLocalMove(mouseInCard.cardOriginPos, 0.2f))
            .Join(transform.DOScale(mouseInCard.cardOriginScale, 0.2f))
            .Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(mouseInCard.cardOriginRotate), 0.2f))
            .OnComplete(() => cardEdgeImage.raycastTarget = true);  // 여러 카드를 빠르게 쓸 때 일부 카드가 마우스를 따라가지 않는 현상을 방지하기 위한 코드
    }

    public void SetCardRaycastTarget(bool isActive)
    {
        cardEdgeImage.raycastTarget = isActive;
    }

    private bool ShouldUseCard(PointerEventData eventData)
    {
        // 공격 타입이 아닌 카드가 손패 영역 밖에 위치했다면 사용으로 간주하고 아니라면 카드 위치를 되돌림
        if (cardInstance.cardData.cardType != CardType.Attack)
            return !RectTransformUtility.RectangleContainsScreenPoint(cardAreaRectTransform, eventData.position);

        // 공격 타입 카드의 화살표가 대상을 찾았다면 사용으로 간주하고 아니라면 카드를 되돌림
        return cardArrow.CheckValidTarget();
    }

    public void CancelCard()
    {
        SetCardState(CardState.InHand);
        ResetCardOriginPos();
        OnRaycastChange?.Invoke(this, true);
    }

    public void ExecuteCard()
    {
        cardInstance.Execute(cardArrow.GetCardArrowTarget());
    }

    #region Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handCardState == CardState.InDeck || handCardState == CardState.InDiscardPile) return;
        if (cardEdgeImage.raycastTarget == false) return;
        // 드래그 시 cardEdgeImage의 Raycast 비활성화 + 화살표(ArrowHeadImage)가 Raycast를 잡아서 Pointer 대상이 변경되므로 무시
        if (handCardState == CardState.Drag) return;

        SetCardState(CardState.Hover);

        float cardHoverScale = 1.65f;
        float scaledHeight = cardEdgeImage.rectTransform.rect.height * cardHoverScale;
        Vector3 cardPos = new Vector3(transform.localPosition.x, -Screen.height * 0.5f + scaledHeight * 0.5f, transform.localPosition.z);

        cardResetSequence.Kill();
        cardHoverSequence = DOTween.Sequence();
        // 카드 위에 마우스가 있다면 해당 카드를 위로 조금 이동시키고 크기를 키우면서 다른 카드에 의해 텍스트가 가려지지 않게 하는 시퀀스
        cardHoverSequence.AppendCallback(() =>
            {
                transform.SetParent(parentRectTransform);
                transform.SetAsLastSibling();
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            })
            .Append(transform.DOLocalMove(cardPos, 0.2f))
            .Join(transform.DOScale(transform.localScale * cardHoverScale, 0.2f));

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
            cardArrow.SetArrowPos();

            // 카드가 마우스를 잘 따라가도록 최상단 캔버스를 부모로 설정함
            if (cardInstance.cardData.cardType != CardType.Attack)
                transform.SetParent(parentRectTransform);

            OnRaycastChange?.Invoke(this, false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (handCardState == CardState.Drag)
        {
            // 카드 이동을 위한 좌표값 설정
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position,
                eventData.pressEventCamera, out localPoint);

            // 화면 밖으로 나가지 않게 카드의 최대 이동 범위 제한
            float halfWidth = parentRectTransform.rect.width * 0.5f;
            float halfHeight = parentRectTransform.rect.height * 0.5f;
            localPoint.x = Mathf.Clamp(localPoint.x, -halfWidth, halfWidth);
            localPoint.y = Mathf.Clamp(localPoint.y, -halfHeight, halfHeight);

            if (cardInstance.cardData.cardType == CardType.Attack)                  // 사용한 카드가 공격 카드일 경우
                cardArrow.DrawArrow(transform.position, eventData.position);
            else                                                                    // 사용한 카드가 공격 이외의 카드일 경우
                transform.localPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handCardState == CardState.InDiscardPile) return;

        if (handCardState == CardState.Drag)
        {
            transform.SetParent(cardAreaRectTransform);

            if (ShouldUseCard(eventData))
                SetCardState(CardState.Used);
            else
                CancelCard();
        }

        if (handCardState == CardState.Used)
        {
            OnUsedCard?.Invoke(this, cardFront.CardCost);           // 현재 카드 코스트를 기준으로 마나를 소모함
            GameEvents.OnUpdateCardText?.Invoke(cardInstance);      // 임시로 변환된 코스트를 원래대로 되돌리기 위해 이벤트를 호출함
        }

        cardArrow.gameObject.SetActive(false);
    }
    #endregion
}
