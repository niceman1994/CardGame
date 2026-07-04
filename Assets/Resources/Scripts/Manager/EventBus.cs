using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace System.Runtime.CompilerServices
{
    // IsExternalInit 클래스가 없으면 init을 사용할 수 없음
    internal static class IsExternalInit { }
}

public enum GameEventType
{
    PLAYER_REGISTER, ENEMY_REGISTER, BATTLE_START, BATTLE_END,
    TURN_START, MANA_RESTORE, CARD_DRAW, CARD_SELECT, RETURN_TO_DECK, CARD_TEXT_UPGRADE,
    TURN_END, RESTART, PAUSE, QUIT, WIN, LOSE, OPENPOPUP, CLOSEPOPUP,
    MANABOOST, COSTDOWN, OVERLOAD, AREAATTACK, PLAYERATTACK, PLAYERDEFEND, PLAYERDEATH, ENEMYATTACK, ENEMYDEATH
}

public struct CardGameData
{
    public int Value { get; init; }
    public ISelectable Target { get; init; }
    public CardInstance CardInstance { get; init; }
    public IReadOnlyList<Monster> RegisterMonsters { get; init; }
}

public class EventBus
{
    private static readonly IDictionary<GameEventType, UnityEvent> Events = new Dictionary<GameEventType, UnityEvent>();

    public static void Subscribe(GameEventType eventType, UnityAction listener)
    {
        UnityEvent thisEvent;

        if (Events.TryGetValue(eventType, out thisEvent))
            thisEvent.AddListener(listener);
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(GameEventType type, UnityAction listener)
    {
        UnityEvent thisEvent;

        if (Events.TryGetValue(type, out thisEvent))
            thisEvent.RemoveListener(listener);
    }

    public static void Publish(GameEventType type)
    {
        UnityEvent thisEvent;

        if (Events.TryGetValue(type, out thisEvent))
            thisEvent.Invoke();
    }
}

public class EventBus<T>
{
    private static readonly IDictionary<GameEventType, UnityEvent<T>> Events = new Dictionary<GameEventType, UnityEvent<T>>();

    public static void Subscribe(GameEventType eventType, UnityAction<T> listener)
    {
        UnityEvent<T> thisEvent;

        if (Events.TryGetValue(eventType, out thisEvent))
            thisEvent.AddListener(listener);
        else
        {
            thisEvent = new UnityEvent<T>();
            thisEvent.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(GameEventType type, UnityAction<T> listener)
    {
        UnityEvent<T> thisEvent;

        if (Events.TryGetValue(type, out thisEvent))
            thisEvent.RemoveListener(listener);
    }

    public static void Publish(GameEventType type, T value)
    {
        UnityEvent<T> thisEvent;

        if (Events.TryGetValue(type, out thisEvent))
            thisEvent.Invoke(value);
    }
}
