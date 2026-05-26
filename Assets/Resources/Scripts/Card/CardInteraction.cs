using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public struct MouseInCard
{
    public int cardOriginIndex;
    public Vector3 shufflePos;
    public Vector3 cardOriginPos;
    public Vector3 cardOriginScale;
    public Vector3 cardOriginRotate;
}

public class CardInteraction : MonoBehaviour
{
    [SerializeField] Image cardEdgeImage;
    [SerializeField] Image cardBackImage;
    [SerializeField] CardArrow cardArrow;

    private MouseInCard mouseInCard;
    private RectTransform cardParentRectTransform;      // 마우스 위치에 따라 카드를 따라가게 만들기 위한 RectTransform 변수
    private RectTransform cardAreaRectTransform;
    private IHealth arrowTarget;
    private Sequence cardHoverSequence;
    private Sequence cardResetSequence;

    public bool cardRaycast => cardEdgeImage.raycastTarget;

    private void Awake()
    {
        mouseInCard.cardOriginScale = transform.localScale;
    }

    public void SetCardParentInArea() => transform.SetParent(cardAreaRectTransform);
    public void SetCardOriginRotate(Vector3 originRotate) => mouseInCard.cardOriginRotate = originRotate;
    public void SetCardRaycaseTarget(bool raycastTarget) => cardEdgeImage.raycastTarget = raycastTarget;
    public void DeactiveCardArrow() => cardArrow.gameObject.SetActive(false);

    public void SetFlip(bool isFlip)
    {
        cardEdgeImage.gameObject.SetActive(isFlip);
        cardBackImage.gameObject.SetActive(!isFlip);
    }

    public void SetCardParentRects(RectTransform parentRect, RectTransform cardAreaRect)
    {
        cardParentRectTransform = parentRect;
        cardAreaRectTransform = cardAreaRect;
    }

    public void SetCardOriginIndex(int index)
    {
        SetCardRaycaseTarget(true);
        mouseInCard.cardOriginIndex = index;
    }

    public Sequence SetShffulePos(float delay, CardState cardState)
    {
        mouseInCard.shufflePos = new Vector3(0, cardEdgeImage.rectTransform.rect.height, transform.localPosition.z);

        Sequence shuffleSequence = DOTween.Sequence();
        shuffleSequence.SetDelay(delay).Append(transform.DOLocalMove(mouseInCard.shufflePos, 0.08f).SetEase(Ease.InOutCubic))
            .Append(transform.DOLocalMove(new Vector3(0, 0, transform.localPosition.z), 0.08f).SetEase(Ease.InOutCubic))
            .JoinCallback(() => cardState = CardState.InDeck);

        return shuffleSequence;
    }

    public void SetCardOriginPos(Vector3 originPos)
    {
        mouseInCard.cardOriginPos = originPos;
        SetCardRaycaseTarget(false);
    }

    public void CardResetSequence()
    {
        // 마우스가 손패 카드들 위를 빠르게 오갈 때 이전 트윈에 의해 카드가 아래로 튀는 현상을 방지하기 위함
        cardHoverSequence?.Kill();

        cardResetSequence = DOTween.Sequence();
        cardResetSequence.JoinCallback(() => transform.SetSiblingIndex(mouseInCard.cardOriginIndex))
            .Append(transform.DOLocalMove(mouseInCard.cardOriginPos, 0.2f))
            .Join(transform.DOScale(mouseInCard.cardOriginScale, 0.2f))
            .Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(mouseInCard.cardOriginRotate), 0.2f))
            .OnComplete(() => cardEdgeImage.raycastTarget = true);  // 여러 카드를 빠르게 쓸 때 일부 카드가 마우스를 따라가지 않는 현상을 방지하기 위한 코드
    }

    public void CardHoverSequence()
    {
        float cardHoverScale = 1.65f;
        float scaledHeight = cardEdgeImage.rectTransform.rect.height * cardHoverScale;
        Vector3 cardPos = new Vector3(transform.localPosition.x, -Screen.height * 0.5f + scaledHeight * 0.5f, transform.localPosition.z);

        cardResetSequence?.Kill();

        cardHoverSequence = DOTween.Sequence();
        cardHoverSequence.AppendCallback(() =>
            {
                transform.SetParent(cardParentRectTransform);
                transform.SetAsLastSibling();
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            })
            .Append(transform.DOLocalMove(cardPos, 0.2f))
            .Join(transform.DOScale(transform.localScale * cardHoverScale, 0.2f));
    }

    public void PointerDown(CardInstance cardInstance)
    {
        cardArrow.SetArrowPos();

        // 카드가 마우스를 잘 따라가도록 최상단 캔버스를 부모로 설정함
        if (cardInstance.cardData.cardType != CardType.Attack)
            transform.SetParent(cardParentRectTransform);
    }

    public void CardDrag(PointerEventData eventData, CardInstance cardInstance)
    {
        // 카드 이동을 위한 좌표값 설정
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardParentRectTransform, eventData.position,
            eventData.pressEventCamera, out localPoint);

        // 화면 밖으로 나가지 않게 카드의 최대 이동 범위 제한
        float halfWidth = cardParentRectTransform.rect.width * 0.5f;
        float halfHeight = cardParentRectTransform.rect.height * 0.5f;
        localPoint.x = Mathf.Clamp(localPoint.x, -halfWidth, halfWidth);
        localPoint.y = Mathf.Clamp(localPoint.y, -halfHeight, halfHeight);

        if (cardInstance.cardData.cardType == CardType.Attack)                  // 사용한 카드가 공격 카드일 경우
            cardArrow.DrawArrow(transform.position, eventData.position);
        else                                                                    // 사용한 카드가 공격 이외의 카드일 경우
            transform.localPosition = localPoint;
    }

    public bool ShouldUseCard(PointerEventData eventData, CardInstance cardInstance)
    {
        // 공격 타입이 아닌 카드가 손패 영역 밖에 위치했다면 사용으로 간주하고 아니라면 카드 위치를 되돌림
        if (cardInstance.cardData.cardType != CardType.Attack)
            return !RectTransformUtility.RectangleContainsScreenPoint(cardAreaRectTransform, eventData.position);

        return CheckValidTarget();
    }

    public bool CheckValidTarget()
    {
        // UI 레이캐스트로 마우스 위치의 오브젝트 감지
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        bool isValidTarget = false;

        foreach (var result in results)
        {
            // 몬스터 하위에 몬스터 크기인 RaycastImage 를 만들고 감지를 확인하기 때문에 GetComponentInParent 를 사용함
            var target = result.gameObject.GetComponentInParent<IHealth>();

            if (target != null)
            {
                arrowTarget = target;
                isValidTarget = true;
                break;
            }
        }

        return isValidTarget;
    }

    public IHealth GetCardArrowTarget()
    {
        return arrowTarget;
    }
}
