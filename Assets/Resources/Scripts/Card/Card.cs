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

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] Image cardEdgeImage;
    [SerializeField] CardFront cardFront;
    [SerializeField] CardArrow cardArrow;
    [SerializeField] Image cardBackImage;
    [SerializeField] CardSound cardSound;

    private CardState handCardState;
    private Vector3 shufflePos;
    private Vector3 cardOriginPos;
    private RectTransform parentRectTransform;      // 마우스 위치에 따라 카드를 따라가게 만들기 위한 RectTransform 변수
    private RectTransform cardAreaRectTransform;
    private Draw draw;

    public event Action<Card> OnRaycastChange;
    public event Action OnCancelCard;
    public event Action<Card, int> OnUsedCard;

    // Deck의 Start에서 CardDraw함수가 실행되기 때문에 오류가 나지 않기 위해 Awake에서 실행함
    private void Awake()
    {
        draw = new Draw(transform);
        SetCardState(CardState.InDeck);
    }

    public void SetCardData(CardData cardData, int index)
    {
        cardFront.SetCardData(cardData);
        name = $"{cardData.cardName}_{index}";
    }

    public void SetCardPos(float drawDelay, Vector3 startPos, Vector3 endScale, float cardRotateZ)
    {
        cardOriginPos = startPos;

        draw.DrawSequence(drawDelay, startPos, endScale)
            .OnStart(() => cardEdgeImage.raycastTarget = false)
            .JoinCallback(() => CheckFirstDraw(draw.IsDraw, cardRotateZ))
            .SetDelay(0.1f)
            .OnComplete(() => cardEdgeImage.raycastTarget = true);
    }

    private void CheckFirstDraw(bool isDraw, float cardRotateZ)
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
                });
        }
        else
            cardDrawSequence.Join(transform.DORotate(new Vector3(0, 0, cardRotateZ), 0.01f).SetEase(Ease.InOutCubic));

        cardDrawSequence.OnComplete(() => SetCardState(CardState.InHand));
    }

    public Sequence CardShuffle(float delay)
    {
        shufflePos = new Vector3(0, cardEdgeImage.rectTransform.rect.height, transform.localPosition.z);

        Sequence deckCardShuffleSequence = DOTween.Sequence();

        deckCardShuffleSequence.SetDelay(delay).Append(transform.DOLocalMove(shufflePos, 0.08f).SetEase(Ease.InOutCubic))
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

    private void SetCardState(CardState cardState)
    {
        handCardState = cardState;
    }

    public void SetCardParentRects(RectTransform parentRect, RectTransform cardAreaRect)
    {
        parentRectTransform = parentRect;
        cardAreaRectTransform = cardAreaRect;
    }

    // 손패->묘지, 묘지->덱으로 카드가 이동할 때 사용하는 시퀀스
    public Sequence CardTransition(Transform parent, List<Card> cardZoneList, CardState cardState)
    {
        Sequence cardTransitionSequecne = DOTween.Sequence();

        cardTransitionSequecne.AppendCallback(() =>
            {
                transform.SetParent(parent);
                cardZoneList.Add(this);
                draw.SetIsDraw(false);
                SetCardState(cardState);
            })
            .Append(transform.DOScale(Vector3.one, 0.2f))
            .Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), 0.2f));

        return cardTransitionSequecne;
    }

    private void ResetCardOriginPos()
    {
        transform.DOLocalMove(cardOriginPos, 0.2f).OnStart(() => cardEdgeImage.raycastTarget = false)
                .OnComplete(() => cardEdgeImage.raycastTarget = true);  // 여러 카드를 빠르게 쓸 때 일부 카드가 마우스를 따라가지 않는 현상을 방지하기 위한 코드
    }

    public void SetCardRaycastTarget(bool isActive)
    {
        cardEdgeImage.raycastTarget = isActive;
    }

    public void ExecuteCard()
    {
        cardFront.Execute(cardArrow.GetCardArrowTarget());
    }

    #region Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handCardState == CardState.InDeck) return;
        if (handCardState == CardState.InDiscardPile) return;
        // 드래그 시 cardEdgeImage의 Raycast 비활성화 + 화살표(ArrowHeadImage)가 Raycast를 잡아서 Pointer 대상이 변경되므로 무시
        if (handCardState == CardState.Drag) return;

        SetCardState(CardState.Hover);
        transform.DOKill();
        transform.DOLocalMove(transform.localPosition + Vector3.up * 30.0f, 0.2f);
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
            if (cardFront.GetCardType != CardType.Attack)
                transform.SetParent(parentRectTransform);
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

            if (cardFront.GetCardType == CardType.Attack)                           // 사용한 카드가 공격타입 카드일 경우
                cardArrow.DrawArrow(transform.position, eventData.position);
            else                                                                    // 사용한 카드가 공격타입 이외의 카드(방어, 스킬)일 경우
                transform.localPosition = localPoint;

            OnRaycastChange?.Invoke(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handCardState == CardState.InDiscardPile) return;

        if (handCardState == CardState.Drag)
        {
            transform.SetParent(cardAreaRectTransform);

            if (cardFront.GetCardType != CardType.Attack)       // 공격 카드가 아닐 때
            {
                // 핸드 영역 밖에 카드가 위치했다면 사용으로 간주하고 아니라면 카드를 원복함
                if (!RectTransformUtility.RectangleContainsScreenPoint(cardAreaRectTransform, eventData.position))
                    SetCardState(CardState.Used);
                else
                {
                    ResetCardOriginPos();
                    SetCardState(CardState.InHand);
                    OnCancelCard?.Invoke();                     // 카드를 쓰지 않으면 등록한 함수를 호출만 시킴
                }
            }
            else
            {
                // 공격 카드의 화살표가 대상을 찾았다면 사용으로 간주하고 아니라면 카드를 원복함
                if (cardArrow.CheckValidTarget())
                    SetCardState(CardState.Used);
                else
                {
                    ResetCardOriginPos();
                    SetCardState(CardState.InHand);
                    OnCancelCard?.Invoke();
                }
            }
        }

        if (handCardState == CardState.Used)
            OnUsedCard?.Invoke(this, cardFront.CardCost);

        cardArrow.gameObject.SetActive(false);
    }
    #endregion
}
