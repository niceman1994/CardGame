using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum TurnState
{
    PlayerTurn, EnemyTurn, Busy
}

public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] List<Monster> activeMonsters = new List<Monster>();
    [SerializeField] Button turnEndButton;
    [SerializeField] Text turnText;
    [SerializeField] TurnState currentTurnState;

    private IHealth player;
    private Queue<IEnumerator> monsterAttackIEnumerator = new Queue<IEnumerator>();
    private Sequence turnSequence;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnPlayerRegistered += SetPlayer;
        GameEvents.OnTurnStart += StartPlayerTurn;
        GameEvents.OnEnemyRegistered += SetEnemyRegister;
        turnEndButton.onClick.AddListener(EndPlayerTurn);
    }

    private void SetPlayer(IHealth player)
    {
        this.player = player;
    }

    public void StartPlayerTurn()
    {
        // 플레이어 턴인걸 텍스트로 알려준 다음, 드로우가 실행되게하는 시퀀스
        turnSequence = DOTween.Sequence();
        turnSequence.AppendCallback(() =>
            {
                currentTurnState = TurnState.Busy;
                turnText.transform.localPosition = new Vector3(-1600, 0, 0);
            })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.PlayTurnChangeSound();
                turnText.text = "Player Turn";
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .AppendCallback(() => turnText.transform.DOLocalMoveX(1600, 0.2f)
            .OnComplete(() =>
            {
                currentTurnState = TurnState.PlayerTurn;
                GameEvents.OnCardDraw?.Invoke();
            }));
        GameEvents.OnManaRestore?.Invoke();
    }

    private void SetEnemyRegister(List<Monster> dequeueMonsters)
    {
        currentTurnState = TurnState.EnemyTurn;
        activeMonsters = dequeueMonsters;
    }

    private void EndPlayerTurn()
    {
        GameEvents.OnTurnEnd?.Invoke();

        // 적 턴인걸 텍스트로 알려준 다음, 적의 공격을 차례대로 실행시키는 시퀀스
        turnSequence = DOTween.Sequence();
        turnSequence.AppendCallback(() =>
            {
                currentTurnState = TurnState.Busy;
                turnText.transform.localPosition = new Vector3(-1600, 0, 0);
            })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.PlayTurnChangeSound();
                turnText.text = "Enemy Turn";
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .AppendCallback(() => turnText.transform.DOLocalMoveX(1600, 0.2f)
            .OnComplete(() =>
            {
                currentTurnState = TurnState.EnemyTurn;

                foreach (var monster in activeMonsters)
                    monsterAttackIEnumerator.Enqueue(monster.ExecuteMonsterAction(player));

                StartCoroutine(AttackInOrder());
            }));
    }

    // 몬스터들이 차례대로 공격하게 하는 함수
    private IEnumerator AttackInOrder()
    {
        while (monsterAttackIEnumerator.Count > 0)
        {
            var monsterAction = monsterAttackIEnumerator.Dequeue();
            yield return StartCoroutine(monsterAction);
        }
        yield return null;

        foreach (var monster in activeMonsters)
            monster.CheckStatusEffect();

        currentTurnState = TurnState.PlayerTurn;
        GameEvents.OnTurnStart?.Invoke();           // 몬스터들의 공격이 끝나면 플레이어의 턴을 시작함
    }
}
