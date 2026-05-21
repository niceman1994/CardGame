using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventType
{
    PLAYER_REGISTER, ENEMY_REGISTER, BATTLE_START, BATTLE_END,
    TURN_START, MANA_RESTORE, CARD_DRAW, RETURN_TO_DECK, CARD_TEXT_UPDATE,
    TURN_END, RESTART, PAUSE, QUIT, WIN, LOSE, OPENPOPUP, CLOSEPOPUP,
    MANABOOST, COSTDOWN, AREAATTACK, PLAYERATTACK, PLAYERDEFEND, PLAYERDEATH, ENEMYATTACK, ENEMYDEATH
}

public struct CardGameData
{
    public int value;
    public IHealth target;
    public CardInstance cardInstance;
    public List<Monster> registerMonsters;
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
