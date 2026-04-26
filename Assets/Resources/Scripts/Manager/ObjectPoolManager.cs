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
    [SerializeField] List<Monster> dequeueMonsters = new List<Monster>();

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
        GameEvents.OnBattleStart?.Invoke();
        GameEvents.OnTurnStart?.Invoke();
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
                queueObject.InitMonster();
                queueObject.gameObject.SetActive(false);
                monsterPools[i].Enqueue(queueObject);
            }
        }
    }

    public void SetMonsters()
    {
        for (int i = 0; i < monsterPos.Count; i++)
        {
            Monster dequeueMonster = monsterPools[Random.Range(0, monsterPools.Count)].Dequeue();
            dequeueMonster.transform.SetParent(monsterPos[i]);
            dequeueMonster.gameObject.SetActive(true);
            dequeueMonster.transform.position = monsterPos[i].position;
            dequeueMonsters.Add(dequeueMonster);
        }
        GameEvents.OnEnemyRegistered?.Invoke(dequeueMonsters);
    }

    public void ReturnPooledObject(Monster pooledObject)
    {
        if (pooledObject.gameObject.activeSelf == true)
        {
            pooledObject.gameObject.SetActive(false);

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