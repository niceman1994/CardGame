using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource skillAudio;
    [SerializeField] AudioSource turnChangeAudio;

    public void PlaySkillSound(AudioClip audioClip)
    {
        skillAudio.clip = audioClip;
        skillAudio.Play();
    }

    public void PlayTurnChangeSound()
    {
        if (!turnChangeAudio.isPlaying)
            turnChangeAudio.Play();
    }
}
