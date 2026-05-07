using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Draw
{
    // 이미 드로우한 카드의 시퀀스 중복 실행을 방지하기 위한 bool 변수
    private bool isDraw;
    private Transform cardTransform;

    public bool IsDraw => isDraw;

    public Draw(Transform cardTransform)
    {
        this.cardTransform = cardTransform;
    }

    public Sequence DrawSequence(float drawDelay, Vector3 startPos, Vector3 endScale)
    {
        Sequence drawSequence = DOTween.Sequence();

        if (isDraw == false)
        {
            drawSequence.SetDelay(drawDelay)
            .Append(cardTransform.DOLocalMove(startPos, 0.1f).SetEase(Ease.OutExpo))
            .Join(cardTransform.DOScale(endScale, 0.1f));        // 덱에 있는 카드를 드로우하면서 커지게 함
        }
       else
        {
            drawSequence.SetDelay(drawDelay)
                .Append(cardTransform.DOLocalMove(startPos, 0.01f).SetEase(Ease.OutExpo));
        }

        return drawSequence;
    }

    public void SetIsDraw(bool isDraw)
    {
        this.isDraw = isDraw;
    }
}
