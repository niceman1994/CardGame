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
    [SerializeField] Image cardBackImage;
    [SerializeField] AudioSource cardAudio;
    [SerializeField] CardSound cardSound;
    [SerializeField] CardState handCardState;

    private Vector3 shufflePos;
    private Vector3 cardOriginPos;
    private RectTransform parentRectTransform;      // 마우스 위치에 따라 카드를 따라가게 만들기 위한 RectTransform 변수
    private Draw draw;

    public event Action<Card> onUsedCard;

    // Deck의 Start에서 CardDraw함수가 실행되기 때문에 오류가 나지 않기 위해 Awake에서 실행함
    private void Awake()
    {
        draw = new Draw(transform);
        SetCardState(CardState.InDeck);
    }

    public void SetCardData(CardData cardData)
    {
        cardFront.SetCardData(cardData);
    }

    public void SetCardPos(float drawDelay, Vector3 startPos, Vector3 endScale, float cardRotateZ)
    {
        cardOriginPos = startPos;
        // TODO : 여러 카드를 빠르게 드래그했을 때 일부 카드가 마우스를 따라가지 않는 현상이 생겨 수정이 필요
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
                    cardSound.PlayDrawSound(cardAudio);
                });
        }
        else
            cardDrawSequence.Join(transform.DORotate(new Vector3(0, 0, cardRotateZ), 0.01f).SetEase(Ease.InOutCubic));

        SetCardState(CardState.InHand);
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

    public void GetCardParent(RectTransform rectTransform)
    {
        parentRectTransform = rectTransform;
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

    /// <summary>
    /// CardTransition 내부에 ResetCardPos를 넣으면 이동이 끝나기 전에 셔플이 실행되서 외부에서 호출 시점을 정하기 위해 public으로 선언함<para/>
    /// <see cref="Deck.MoveToDeck"/>, <see cref="DiscardPile.MoveToDiscardPile"/>에서 사용함
    /// </summary>
    public void ResetCardPos(List<Card> cardZoneList)
    {
        for (int i = 0; i < cardZoneList.Count; i++)
        {
            int index = i;
            // 덱을 셔플했을 때 카드가 이상하게 배치되는 현상을 발견함
            // z 값을 기준으로 UI를 렌더링하기 때문에 SetSiblingIndex 로 무작위로 섞은 카드와 하이어라키의 구조를 같게 해줘야함
            cardZoneList[index].transform.DOLocalMove(new Vector3(0, 0, index), 0.3f);
        }
    }

    #region Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handCardState == CardState.InDeck) return;
        if (handCardState == CardState.InDiscardPile) return;

        SetCardState(CardState.Hover);
        transform.DOLocalMove(transform.localPosition + Vector3.up * 30.0f, 0.2f);
        cardSound.PlayHoverSound(cardAudio);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (handCardState == CardState.Hover)
        {
            SetCardState(CardState.InHand);
            transform.DOLocalMove(cardOriginPos, 0.2f).OnStart(() => cardEdgeImage.raycastTarget = false)
                .OnComplete(() => cardEdgeImage.raycastTarget = true);
        }
    }
    #endregion

    #region Drag&Use
    public void OnPointerDown(PointerEventData eventData)
    {
        if (handCardState == CardState.Hover || handCardState == CardState.InHand)
            SetCardState(CardState.Drag);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (handCardState == CardState.Drag)
        {
            if (parentRectTransform == null)
            {
                Debug.LogError($"카드의 {parentRectTransform}이 없습니다!");
                return;
            }
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,                // 부모 RectTransform
                eventData.position,                 // 마우스 스크린 좌표
                eventData.pressEventCamera,         // 카메라
                out localPoint                      // 변환된 로컬 좌표
            );
            transform.localPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handCardState == CardState.InDiscardPile) return;

        if (handCardState == CardState.Drag)
            SetCardState(CardState.Used);

        if (handCardState == CardState.Used)
        {
            onUsedCard?.Invoke(this);
            onUsedCard = null;          // 카드를 재사용할 때 함수 중복 등록을 피하기 위해 null을 넣음
        }
    }
    #endregion
}
