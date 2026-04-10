using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSound : MonoBehaviour
{
    [SerializeField] AudioSource objectAudio;

    // 공격 애니메이션의 이벤트에 등록해 지정한 프레임에 소리가 나오는 함수
    public void PlayAttackSound(AudioClip attackClip)
    {
        if (attackClip != null)
        {
            objectAudio.clip = attackClip;
            objectAudio.Play();
        }
    }

    public void PlayDeathSound(AudioClip deathClip)
    {
        if (deathClip != null)
        {
            objectAudio.clip = deathClip;
            objectAudio.Play();
        }
    }

    public void PlayShieldSound(AudioClip shieldClip)
    {
        if (shieldClip != null)
        {
            objectAudio.clip = shieldClip;
            objectAudio.Play();
        }
    }
}
