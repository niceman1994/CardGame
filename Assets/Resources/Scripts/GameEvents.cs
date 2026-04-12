using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    // 턴 시작, 종료
    public static Action OnTurnStart;
    public static Action OnTurnEnd;

    // 카드 드로우
    public static Action OnCardDraw;
    public static Action<int> OnExtraCardDraw;

    // 마나 관련
    public static Action OnManaRestore;
    public static Action<int> OnManaBoost;

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
