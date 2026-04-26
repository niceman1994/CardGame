using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource skillAudio;
    [SerializeField] AudioSource turnChangeAudio;
    [SerializeField] AudioSource winAudio;
    [SerializeField] AudioSource loseAudio;
    [SerializeField] AudioSource cardUpgradeAudio;

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

    public IEnumerator PlayWinSound()
    {
        GameEvents.OnBattleEnd?.Invoke();
        GameEvents.OnBattleWin?.Invoke();
        winAudio.Play();
        yield return new WaitForSeconds(2.0f);
    }

    public IEnumerator PlayLoseSound()
    {
        GameEvents.OnBattleEnd?.Invoke();
        GameEvents.OnBattleLose?.Invoke();
        loseAudio.Play();
        yield return new WaitForSeconds(2.0f);
    }

    public void PlayCardUpgradeSound()
    {
        if (!cardUpgradeAudio.isPlaying)
            cardUpgradeAudio.Play();
    }
}
