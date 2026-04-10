using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Deck : MonoBehaviour
{
    [SerializeField] List<Card> currentDeckList = new List<Card>();
    [SerializeField] AudioSource deckShuffleSound;
    [SerializeField] int drawCardCount;

    public event Action<Card> OnHandToCard;
    public event Action<List<Card>, Transform> OnAddCard;

    private void Start()
    {
        InitDeck();
    }

    private void InitDeck()
    {
        OnAddCard?.Invoke(currentDeckList, transform);
        ResetDeckCardPos();
        GameEvents.OnCardDraw += CardDraw;
    }

    public void CardDraw()
    {
        // 드로우할 카드가 현재 덱의 카드 수보다 많으면 그 수만큼만 드로우되게 함
        if (drawCardCount > currentDeckList.Count && currentDeckList.Count != 0)
            drawCardCount = currentDeckList.Count;

        for (int i = 0; i < drawCardCount; i++)
        {
            // 덱의 카드 수가 0이면 예외 처리
            if (currentDeckList.Count <= 0) return;

            // 덱 맨 위부터 드로우하기 때문에 리스트의 마지막 요소부터 시작함
            var targetCard = currentDeckList[currentDeckList.Count - 1];
            currentDeckList.Remove(targetCard);
            OnHandToCard?.Invoke(targetCard);
        }
    }

    public void DeckShuffle()
    {
        if (currentDeckList.Count == 0)
        {
            Debug.LogError("덱에 카드가 없어 셔플할 수 없습니다!");
            return;
        }
        Sequence deckShuffleSequence = DOTween.Sequence();

        for (int i = currentDeckList.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card tempCard = currentDeckList[i];
            currentDeckList[i] = currentDeckList[randomIndex];
            currentDeckList[randomIndex] = tempCard;

            deckShuffleSequence.Join(currentDeckList[i].CardShuffle(0.01f * i));
        }
        deckShuffleSound.Play();
        // ResetDeckCardPos 함수의 호출 타이밍을 뒤로 미루기 위해 OnComplete를 사용함
        deckShuffleSequence.OnComplete(() => ResetDeckCardPos()); 
    }

    private void ResetDeckCardPos()
    {
        for (int i = 0; i < currentDeckList.Count; i++)
        {
            currentDeckList[i].transform.localPosition = new Vector3(0, 0, i);
            // 덱을 셔플했을 때 카드가 이상하게 배치되는 현상을 발견함
            // z 값을 기준으로 UI를 렌더링하기 때문에 SetSiblingIndex 로 무작위로 섞은 카드와 하이어라키의 구조를 같게 해줘야함
            currentDeckList[i].transform.SetSiblingIndex(i);
        }
    }

    public Sequence MoveToDeck(Card returnCard)
    {
        return returnCard.CardTransition(transform, currentDeckList, CardState.InDeck)
            .JoinCallback(() => returnCard.FlipCard(false));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Deck))]
public class DeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Deck deckManager = (Deck)target;
        //SerializedProperty _drawCountIndex = serializedObject.FindProperty("drawCardCount");

        if (GUILayout.Button("카드 드로우"))
        {
            deckManager.CardDraw();
        }

        if (GUILayout.Button("덱 셔플"))
        {
            deckManager.DeckShuffle();
        }
    }
}
#endif