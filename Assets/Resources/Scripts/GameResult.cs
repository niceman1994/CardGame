using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameResult : MonoBehaviour
{
    [SerializeField] Image winImage;
    [SerializeField] Image loseImage;
    [SerializeField] CardUpgradeViewer cardUpgradeViewer;
    [SerializeField] RestartPopupBox restartPopup;

    private Sequence winSequence;
    private Sequence popupSequence;

    private void Start()
    {
        GameEvents.OnBattleWin += FadeOutWinImage;
        GameEvents.OnBattleLose += FadeOutLoseImage;
    }

    private void FadeOutWinImage()
    {
        winSequence = DOTween.Sequence();
        winSequence.Append(winImage.DOColor(Color.white, 1.0f))
            .Append(winImage.DOColor(new Color(winImage.color.r, winImage.color.g, winImage.color.b, 0.0f), 1.0f))
            .OnComplete(() => cardUpgradeViewer.ActiveUpgradeButton());
    }

    private void FadeOutLoseImage()
    {
        string popupText = "게임을 다시 시작할 수 있습니다";

        popupSequence = DOTween.Sequence();
        popupSequence.Append(loseImage.DOColor(Color.white, 1.0f))
            .Append(loseImage.DOColor(new Color(loseImage.color.r, loseImage.color.g, loseImage.color.b, 0.0f), 1.0f))
            .OnComplete(() => restartPopup.OpenPopup(popupText));
    }
}
