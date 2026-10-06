using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] Button menuButton;
    [SerializeField] Button turnEndButton;
    [SerializeField] Text turnText;
    [SerializeField] GameObject panel;
    
    private IReadOnlyList<Monster> activeMonsters = new List<Monster>();
    private IHealth player;
    private Sequence turnSequence;
    private Coroutine attackCoroutine;

    protected override void Awake()
    {
        base.Awake();
        InitDeckManager();
    }

    private void InitDeckManager()
    {
        turnEndButton.interactable = false;
        EventBus<CardGameData>.Subscribe(GameEventType.PLAYER_REGISTER, SetPlayer);
        EventBus<CardGameData>.Subscribe(GameEventType.ENEMY_REGISTER, SetEnemyRegister);
        EventBus.Subscribe(GameEventType.TURN_START, StartPlayerTurn);
        turnEndButton.onClick.AddListener(EndPlayerTurn);

        EventBus.Subscribe(GameEventType.BATTLE_END, DeactiveTurnEndButton);
        EventBus.Subscribe(GameEventType.OPENPOPUP, DeactiveTurnEndButton);
        EventBus.Subscribe(GameEventType.CLOSEPOPUP, ActiveTurnEndButton);
        EventBus.Subscribe(GameEventType.RESTART, GameRestart);
    }

    private void SetPlayer(CardGameData data)
    {
        player = data.Target as IHealth;
    }

    private void ActiveTurnEndButton()
    {
        turnEndButton.interactable = true;
    }

    private void DeactiveTurnEndButton()
    {
        turnEndButton.interactable = false;
    }

    private void StartPlayerTurn()
    {
        turnText.transform.localPosition = new Vector3(-3000, 0, 0);
        // 플레이어 턴인걸 텍스트로 알려준 다음, 드로우가 실행되게하는 시퀀스
        turnSequence = DOTween.Sequence();
        turnSequence.AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.PlayTurnChangeSound();
                turnText.text = "Player Turn";
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .Append(turnText.transform.DOLocalMoveX(3000, 0.2f))
            .AppendCallback(() =>
            {
                EventBus.Publish(GameEventType.CARD_DRAW);
                menuButton.interactable = true;
            })
            .AppendInterval(0.3f)
            .OnComplete(() =>
            {
                panel.gameObject.SetActive(false);
                turnEndButton.interactable = true;
            });

        EventBus.Publish(GameEventType.MANA_RESTORE);
    }

    private void SetEnemyRegister(CardGameData cardGameData)
    {
        activeMonsters = cardGameData.RegisterMonsters;
    }

    private void EndPlayerTurn()
    {
        turnText.transform.localPosition = new Vector3(-3000, 0, 0);
        turnEndButton.interactable = false;
        EventBus.Publish(GameEventType.TURN_END);

        // 적 턴인걸 텍스트로 알려준 다음, 적의 공격을 차례대로 실행시키는 시퀀스
        turnSequence = DOTween.Sequence();
        turnSequence.AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.PlayTurnChangeSound();
                turnText.gameObject.SetActive(true);
                turnText.text = "Enemy Turn";
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .AppendCallback(() => turnText.transform.DOLocalMoveX(3000, 0.2f))
            .OnComplete(() => attackCoroutine = StartCoroutine(AttackInOrder()));
    }

    // 행동 순서를 보장하기 위해 TurnManager에서 몬스터들의 ExecuteMonsterAction 함수를 호출함
    private IEnumerator AttackInOrder()
    {
        for (int i = 0; i < activeMonsters.Count; i++)
        {
            var monsterAction = activeMonsters[i].ExecuteMonsterAction(player);
            yield return monsterAction;
        }

        // 공격 이후 상태이상 처리를 진행함
        for (int i = 0; i < activeMonsters.Count; i++)
            activeMonsters[i].CheckStatusEffect();

        if (player.CurrentHp() < 0)
            yield break;

        EventBus.Publish(GameEventType.TURN_START);     // 몬스터들의 공격이 끝나면 플레이어의 턴을 시작함
        panel.gameObject.SetActive(true);
    }

    // 재시작할 때 실행된 코루틴을 강제로 멈추게 하는 함수
    private void GameRestart()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }
}
