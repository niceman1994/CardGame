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

        drawSequence.SetDelay(drawDelay).AppendCallback(() => CheckFirstDraw(startPos, endScale));

        return drawSequence;
    }

    private void CheckFirstDraw(Vector3 startPos, Vector3 endScale)
    {
        Sequence drawCheckSequence = DOTween.Sequence();

        // 이미 드로우한 카드의 시퀀스 재실행을 방지하기 위한 코드
        if (isDraw == false)
        {
            drawCheckSequence.Join(cardTransform.DOLocalMove(startPos, 0.2f).SetEase(Ease.OutExpo))
            .Join(cardTransform.DOScale(endScale, 0.2f))       // 덱에 있는 카드를 드로우하면서 커지게 함
            .JoinCallback(() => isDraw = true);
        }
        else
            drawCheckSequence.Join(cardTransform.DOLocalMove(startPos, 0.01f).SetEase(Ease.OutExpo));
    }

    public void SetIsDraw(bool isDraw)
    {
        this.isDraw = isDraw;
    }
}
