using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Header("플레이어 프리팹")]
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform playerPosition;
    [Space(15)]
    [SerializeField] Transform monsterCanvas;
    [SerializeField] List<Monster> monsterPrefab;
    [SerializeField] List<Transform> monsterPos;

    private Dictionary<int, Queue<Monster>> monsterPools = new Dictionary<int, Queue<Monster>>();
    // 플레이어가 진행 도중에 죽었을 때 남은 몬스터를 회수하기 위해 사용하는 리스트 변수
    private List<Monster> dequeueMonsters = new List<Monster>();

    protected override void Awake()
    {
        base.Awake();
        EnqueueMonsters();
    }

    private void Start()
    {
        SummonPlayer();
        SetMonsters();
    }

    private void SummonPlayer()
    {
        Player playerObject = Instantiate(playerPrefab, playerPosition);
        playerObject.name = "Player";
        EventBus.Publish(GameEventType.BATTLE_START);
        EventBus.Publish(GameEventType.TURN_START);
    }

    private void EnqueueMonsters()
    {
        for (int i = 0; i < monsterPrefab.Count; i++)
            monsterPools.Add(i, new Queue<Monster>());

        for (int i = 0; i < monsterPrefab.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Monster queueObject = Instantiate(monsterPrefab[i]);
                queueObject.transform.SetParent(monsterCanvas);
                queueObject.gameObject.SetActive(false);
                queueObject.InitMonster();
                monsterPools[i].Enqueue(queueObject);
            }
        }
    }

    public void SetMonsters()
    {
        // 비활성화된 몬스터들 중에 필드에 나올 몬스터들만 다시 활성화시키고 Monster 스크립트에서 이벤트를 등록함
        for (int i = 0; i < monsterPos.Count; i++)
        {
            Monster dequeueMonster = monsterPools[Random.Range(0, monsterPools.Count)].Dequeue();
            dequeueMonster.transform.SetParent(monsterPos[i]);
            dequeueMonster.gameObject.SetActive(true);
            dequeueMonster.SubscribeEvent();
            dequeueMonster.transform.position = monsterPos[i].position;
            dequeueMonsters.Add(dequeueMonster);
        }
        EventBus<CardGameData>.Publish(GameEventType.ENEMY_REGISTER, new CardGameData { RegisterMonsters = dequeueMonsters });
    }

    public void ReturnPooledObject(Monster pooledObject)
    {
        if (pooledObject.gameObject.activeSelf == true)
        {
            pooledObject.gameObject.SetActive(false);
            pooledObject.UnsubscribeEvent();

            for (int i = 0; i < monsterPools.Count; i++)
            {
                Object monster = monsterPools.Values.ElementAt(i).Peek();

                // 오브젝트 풀링으로 재활용하기 큐에서 뺐던 오브젝트와 큐의 맨 앞에 있는 오브젝트와 비교해 일치하면 큐에 넣음
                if (pooledObject.name.Equals(monster.name))
                {
                    pooledObject.InitMonster();
                    pooledObject.transform.SetParent(monsterCanvas);
                    monsterPools.Values.ElementAt(i).Enqueue(pooledObject);
                    dequeueMonsters.Remove(pooledObject);
                }
            }
        }
    }

    public void ReturnPlayer(Player player)
    {
        for (int i = dequeueMonsters.Count - 1; i >= 0; i--)
            ReturnPooledObject(dequeueMonsters[i]);

        dequeueMonsters.Clear();
        player.InitPlayer();
    }

    public void OnBattleEnd()
    {
        if (dequeueMonsters.Count == 0)
            StartCoroutine(SoundManager.Instance.PlayWinSound());
    }
}