using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Header("플레이어 프리팹")]
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform playerPosition;
    [Header("몬스터 프리팹")]
    [SerializeField] Transform monsterParent;
    [SerializeField] List<Monster> monsterPrefab;
    [SerializeField] List<Transform> monsterPosition;

    private Dictionary<int, Queue<Monster>> monsterPools = new Dictionary<int, Queue<Monster>>();
    // 플레이어가 진행 도중에 죽었을 때 남은 몬스터를 회수하기 위해 사용하는 리스트 변수
    private List<Monster> dequeueMonsters = new List<Monster>();

    protected override void Awake()
    {
        base.Awake();
        EnqueueMonsters();
        SummonPlayer();
    }

    private void Start()
    {
        SetMonsters();
    }

    private void SummonPlayer()
    {
        Player playerObject = Instantiate(playerPrefab, playerPosition);
        playerObject.name = "Player";
    }

    private void EnqueueMonsters()
    {
        for (int i = 0; i < monsterPrefab.Count; i++)
            monsterPools.Add(i, new Queue<Monster>());

        for (int i = 0; i < monsterPrefab.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Monster queueObject = Instantiate(monsterPrefab[i], monsterParent);
                queueObject.InitMonster();
                queueObject.gameObject.SetActive(false);
                monsterPools[i].Enqueue(queueObject);
            }
        }
    }

    public void SetMonsters()
    {
        for (int i = 0; i < monsterPosition.Count; i++)
        {
            Monster dequeueMonster = monsterPools[Random.Range(0, monsterPools.Count)].Dequeue();
            dequeueMonsters.Add(dequeueMonster);
            dequeueMonster.transform.SetParent(monsterPosition[i]);
            dequeueMonster.gameObject.SetActive(true);
            dequeueMonster.transform.position = monsterPosition[i].position;
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

                // 오브젝트 풀링으로 재활용하기 큐에서 뺐던 오브젝트와 현재 큐의 맨 앞에 있는 오브젝트를 비교해 일치하면 다시 큐에 넣음
                if (pooledObject.name.Equals(monster.name))
                {
                    pooledObject.InitMonster();
                    pooledObject.transform.SetParent(monsterParent.parent);
                    monsterPools.Values.ElementAt(i).Enqueue(pooledObject);
                    dequeueMonsters.Remove(pooledObject);
                }
            }
        }
        if (dequeueMonsters.Count == 0)
            GameEvents.OnBattleEnd?.Invoke();
    }

    public void ReturnPooledMonsters()
    {
        // 플레이어가 죽었을 때 생성된 몬스터를 담은 리스트의 뒤부터 확인하면서 큐에 집어넣음
        for (int i = dequeueMonsters.Count - 1; i >= 0; i--)
            ReturnPooledObject(dequeueMonsters[i]);
    }
}