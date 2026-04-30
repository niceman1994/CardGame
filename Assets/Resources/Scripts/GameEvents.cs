using System;
using System.Collections;
using System.Collections.Generic;

public static class GameEvents
{
    // 전투 시작, 종료
    public static Action OnBattleStart;
    public static Action OnBattleEnd;

    public static Action OnBattleWin;
    public static Action OnBattleLose;

    public static Action OnTurnStart;
    public static Action OnTurnEnd;

    public static Action OnOpenPopup;
    public static Action OnClosePopup;
    public static Action OnGameRestart;
    public static Action<IHealth> OnPlayerRegistered;

    // 카드 관련
    public static Action OnCardDraw;
    public static Action<int> OnExtraCardDraw;
    public static Action OnReturnToDeck;
    public static Action<CardInstance> OnUpdateCardText;

    // 마나 관련
    public static Action OnManaRestore;
    public static Action<int> OnManaBoost;
    public static Action<int> OnCostDown;

    // 플레이어
    public static Action<int, IHealth> OnPlayerAttack;
    public static Action<int> OnPlayerDefend;
    public static Action OnPlayerDeath;
    // 광역 공격
    public static Action<int> OnPlayerAoeAttack;

    // 적
    public static Action<List<Monster>> OnEnemyRegistered;
    public static Action<int> OnEnemyDefend;
    public static Action<Monster> OnEnemyDeath;
}
