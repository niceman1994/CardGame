using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource areaAttackAudio;
    [SerializeField] AudioSource turnChangeAudio;
    [SerializeField] AudioSource winAudio;
    [SerializeField] AudioSource loseAudio;
    [SerializeField] AudioSource cardUpgradeAudio;

    public void PlayAreaAttackSound()
    {
        areaAttackAudio.Play();
    }

    public void PlayTurnChangeSound()
    {
        if (!turnChangeAudio.isPlaying)
            turnChangeAudio.Play();
    }

    public IEnumerator PlayWinSound()
    {
        EventBus.Publish(GameEventType.BATTLE_END);
        EventBus.Publish(GameEventType.WIN);
        winAudio.Play();
        yield return new WaitForSeconds(2.0f);
    }

    public IEnumerator PlayLoseSound()
    {
        EventBus.Publish(GameEventType.BATTLE_END);
        EventBus.Publish(GameEventType.LOSE);
        loseAudio.Play();
        yield return new WaitForSeconds(2.0f);
    }

    public void PlayCardUpgradeSound()
    {
        if (!cardUpgradeAudio.isPlaying)
            cardUpgradeAudio.Play();
    }
}
