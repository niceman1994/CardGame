using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSound : MonoBehaviour
{
    [SerializeField] AudioClip cardDrawClip;
    [SerializeField] AudioClip cardHoverClip;

    public void PlayDrawSound(AudioSource audioSource)
    {
        audioSource.clip = cardDrawClip;
        audioSource.Play();
    }

    public void PlayHoverSound(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = cardHoverClip;
            audioSource.Play();
        }
    }
}
