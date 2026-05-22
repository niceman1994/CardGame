using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] IReadOnlyList<Monster> activeMonsters = new List<Monster>();
    [SerializeField] Button menuButton;
    [SerializeField] Button turnEndButton;
    [SerializeField] Text turnText;

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

        EventBus.Subscribe(GameEventType.BATTLE_END, () => turnEndButton.interactable = false);
        EventBus.Subscribe(GameEventType.OPENPOPUP, () => turnEndButton.interactable = false);
        EventBus.Subscribe(GameEventType.CLOSEPOPUP, () => turnEndButton.interactable = true);
        EventBus.Subscribe(GameEventType.RESTART, GameRestart);
    }

    private void SetPlayer(CardGameData data)
    {
        this.player = data.Target;
    }

    private void StartPlayerTurn()
    {
        // 플레이어 턴인걸 텍스트로 알려준 다음, 드로우가 실행되게하는 시퀀스
        turnSequence = DOTween.Sequence();
        turnSequence.AppendCallback(() => turnText.transform.localPosition = new Vector3(-1600, 0, 0))
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.PlayTurnChangeSound();
                turnText.text = "Player Turn";
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .Append(turnText.transform.DOLocalMoveX(1600, 0.2f))
            .AppendCallback(() =>
            {
                EventBus.Publish(GameEventType.CARD_DRAW);
                menuButton.interactable = true;
            })
            .AppendInterval(0.3f)
            .OnComplete(() => turnEndButton.interactable = true);

        EventBus.Publish(GameEventType.MANA_RESTORE);
    }

    private void SetEnemyRegister(CardGameData cardGameData)
    {
        activeMonsters = cardGameData.RegisterMonsters;

        foreach (var monster in activeMonsters)
            EventBus<int>.Subscribe(GameEventType.AREAATTACK, monster.TakeDamage);
    }

    private void EndPlayerTurn()
    {
        turnEndButton.interactable = false;
        EventBus.Publish(GameEventType.TURN_END);

        // 적 턴인걸 텍스트로 알려준 다음, 적의 공격을 차례대로 실행시키는 시퀀스
        turnSequence = DOTween.Sequence();
        turnSequence.AppendCallback(() => turnText.transform.localPosition = new Vector3(-1600, 0, 0))
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.PlayTurnChangeSound();
                turnText.text = "Enemy Turn";
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .AppendCallback(() => turnText.transform.DOLocalMoveX(1600, 0.2f))
            .OnComplete(() => attackCoroutine = StartCoroutine(AttackInOrder()));
    }

    // 몬스터들이 차례대로 공격하게 하는 함수
    private IEnumerator AttackInOrder()
    {
        foreach (var monster in activeMonsters)
        {
            var monsterAction = monster.ExecuteMonsterAction(player);
            yield return StartCoroutine(monsterAction);
        }
        // 공격 이후에 상태이상에 대한 처리를 실행함
        foreach (var monster in activeMonsters)
            monster.CheckStatusEffect();

        if (player.CurrentHp() < 0)
            yield break;

        EventBus.Publish(GameEventType.TURN_START);     // 몬스터들의 공격이 끝나면 플레이어의 턴을 시작함
    }

    // 재시작할 때 실행된 코루틴을 강제로 멈추게 하는 함수
    private void GameRestart()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }
}
