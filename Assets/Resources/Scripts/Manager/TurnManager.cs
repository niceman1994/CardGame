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
    [SerializeField] Button menuButton;
    [SerializeField] Button turnEndButton;
    [SerializeField] Text turnText;

    private TurnState currentTurnState;
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
        GameEvents.OnPlayerRegistered += SetPlayer;
        GameEvents.OnTurnStart += StartPlayerTurn;
        GameEvents.OnEnemyRegistered += SetEnemyRegister;
        turnEndButton.onClick.AddListener(EndPlayerTurn);

        GameEvents.OnBattleEnd += () => turnEndButton.interactable = false;
        GameEvents.OnOpenOptionPopup += () => turnEndButton.interactable = false;
        GameEvents.OnCloseOptionPopup += () => turnEndButton.interactable = true;
        GameEvents.OnGameRestart += GameRestart;
    }

    private void SetPlayer(IHealth player)
    {
        this.player = player;
    }

    private void StartPlayerTurn()
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
                currentTurnState = TurnState.PlayerTurn;
                turnText.transform.DOLocalMoveX(0, 0.2f);
            })
            .AppendInterval(0.7f)
            .Append(turnText.transform.DOLocalMoveX(1600, 0.2f))
            .AppendCallback(() =>
            {
                GameEvents.OnCardDraw?.Invoke();
                menuButton.interactable = true;
            })
            .AppendInterval(0.3f)
            .OnComplete(() => turnEndButton.interactable = true);

        GameEvents.OnManaRestore?.Invoke();
    }

    private void SetEnemyRegister(List<Monster> dequeueMonsters)
    {
        currentTurnState = TurnState.EnemyTurn;
        activeMonsters = dequeueMonsters;
    }

    private void EndPlayerTurn()
    {
        turnEndButton.interactable = false;
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
                currentTurnState = TurnState.EnemyTurn;
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
        
        GameEvents.OnTurnStart?.Invoke();           // 몬스터들의 공격이 끝나면 플레이어의 턴을 시작함
    }

    // 재시작할 때 실행된 코루틴을 강제로 멈추게 하는 함수
    private void GameRestart()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }
}
