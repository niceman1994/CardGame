using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSound : MonoBehaviour
{
    [SerializeField] AudioSource cardAudio;
    [SerializeField] AudioClip cardDrawClip;
    [SerializeField] AudioClip cardHoverClip;
    [SerializeField] AudioClip cardCancelClip;

    public void PlayDrawSound()
    {
        cardAudio.clip = cardDrawClip;
        cardAudio.Play();
    }

    public void PlayHoverSound()
    {
        if (!cardAudio.isPlaying)
        {
            cardAudio.clip = cardHoverClip;
            cardAudio.Play();
        }
    }

    // 코스트 부족으로 카드를 쓰지 못할 때 사용하는 함수
    public void PlayInsufficientCostSound()
    {
        cardAudio.clip = cardCancelClip;
        cardAudio.Play();
    }
}
