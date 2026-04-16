using System;
using System.Collections;
using System.Collections.Generic;

public static class GameEvents
{
    // 전투 시작, 종료
    public static Action OnBattleStart;
    public static Action OnBattleEnd;

    // 턴 시작, 종료
    public static Action OnTurnStart;
    public static Action OnTurnEnd;

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
    public static Action<IHealth> OnPlayerRegistered;
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
