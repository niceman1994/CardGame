using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DiscardPile : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] List<Card> discardPileList = new List<Card>();

    public void MoveToDiscardPile(Card card)
    {
        Sequence moveDiscardPileSequence = DOTween.Sequence();

        moveDiscardPileSequence.Join(card.CardTransition(transform, discardPileList, CardState.InDiscardPile))
            .AppendCallback(() => card.ResetCardPos(discardPileList));
    }

    public void ReturnToDeck()
    {
        if (discardPileList.Count == 0)
        {
            Debug.LogError($"{name}에 카드가 없습니다!");
            return;
        }

        Sequence moveDeckSequence = DOTween.Sequence();

        for (int i = 0; i < discardPileList.Count; i++)
            moveDeckSequence.Join(deck.MoveToDeck(discardPileList[i]));

        // 카드가 덱으로 다 이동한 다음에 덱을 셔플하기 위해 OnComplete로 호출 시점을 미룸
        moveDeckSequence.OnComplete(() =>
        {
            discardPileList.Clear();
            deck.DeckShuffle();
        });
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DiscardPile))]
public class DiscardPileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DiscardPile discardPileManager = (DiscardPile)target;

        if (GUILayout.Button("덱으로 되돌리기"))
        {
            discardPileManager.ReturnToDeck();
        }
    }
}
#endif