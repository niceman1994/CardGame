using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSound : MonoBehaviour
{
    [SerializeField] AudioSource cardAudio;
    [SerializeField] AudioClip cardDrawClip;
    [SerializeField] AudioClip cardHoverClip;

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
}
