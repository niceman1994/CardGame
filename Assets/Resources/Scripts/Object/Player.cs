using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    [SerializeField] int currentHp;
    [SerializeField] AudioSource attackAudio;

    private int maxHp;
    private Animator animator;

    private void Start()
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        animator = GetComponent<Animator>();
        currentHp = playerData.playerHp;
        maxHp = playerData.playerMaxHp;
    }

    public void PlayAttackAni()
    {
        animator.Play("Attack");

        attackAudio.clip = playerData.attackSound;
        attackAudio.Play();
    }

    public void PlayDeathAni()
    {
        animator.Play("Death");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Player player = (Player)target;

        if (GUILayout.Button("공격 실행"))
        {
            player.PlayAttackAni();
        }

        if (GUILayout.Button("사망 실행"))
        {
            player.PlayDeathAni();
        }
    }
}
#endif