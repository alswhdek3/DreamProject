using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    None=-1,
    Monster_01,Monster_02,Boss_01,
    Max
}
public class MonsterManager : SingtonMonoBehaviour<MonsterManager>
{
    [SerializeField] private GameObject m_monsterPrefab;
    private GameObjectPool<MonsterController> m_monsterPool;
    private Dictionary<MonsterType, List<MonsterController>> m_dicMonsterPool = new Dictionary<MonsterType, List<MonsterController>>();
    private Dictionary<MonsterType,GameObject> m_dicMonsterPrefab = new Dictionary<MonsterType, GameObject>();
    [SerializeField] private PlayerController m_player;

    public MonsterController CreateMonster(MonsterType type)
    {
        if (!m_dicMonsterPool.ContainsKey(type)) return null; // 몬스터를 생성하기전 Dictionary에 몬스터 타입이 존재하지않으면 null 반환
        MonsterController monster;
        var pool = m_dicMonsterPool[type];
        if(pool.Count > 0) //여유분의 몬스터가 있으면 Dictionary에서 몬스터를 꺼내서 쓴다.
        {
            monster = pool[0];
            pool.Remove(monster);
            monster.SetMonster(type);
            monster.gameObject.SetActive(true);
            return monster;
        }
        else //여유분의 몬스터가 없으면 새로 몬스터를 만든다.
        {
            var prefab = m_dicMonsterPrefab[type];                        
            monster = CreateMonsterPoolUnit(type, prefab);
            monster.transform.SetParent(transform);
            monster.gameObject.SetActive(true);
            return monster;
        }
    }
    private MonsterController CreateMonsterPoolUnit(MonsterType type,GameObject prefab)
    {
        var obj = Instantiate(prefab);
        var monster = obj.GetComponent<MonsterController>();
        if (monster == null) obj.GetComponent<MonsterController>();
        monster.SetMonster(type);
        return monster;
    }
    public void AddPoolUnit(MonsterType type,MonsterController monster)
    {
        if (!m_dicMonsterPool.ContainsKey(type)) return;
        else
        {
            monster.gameObject.SetActive(false);
            m_dicMonsterPool[type].Add(monster);
        }
    }

    protected override void OnStart()
    {
        for(int i=0; i<(int)MonsterType.Max; i++)
        {
            if(i != (int)MonsterType.Boss_01)
                m_monsterPrefab = Resources.Load("Prefab/Monster/" + (MonsterType)i) as GameObject;

            List<MonsterController> monsterList = new List<MonsterController>(); // 타입별 몬스터를 추가할 리스트
            monsterList.Clear(); // 추가하기전에 한번 초기화
            m_monsterPool = new GameObjectPool<MonsterController>(2,() =>
            {
                var obj = Instantiate(m_monsterPrefab);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector2(100f, 100f);
                var monster = obj.GetComponent<MonsterController>();
                monster.gameObject.SetActive(false);
                monsterList.Add(monster);
                return monster;
            });
            if (!m_dicMonsterPool.ContainsKey((MonsterType)i)) m_dicMonsterPool.Add((MonsterType)i, monsterList); //Dictionary에 추가
            //Dictionary Prefab 추가
            var prefab = Instantiate(m_monsterPrefab);
            prefab.transform.SetParent(transform);
            prefab.transform.localPosition = Vector3.zero;
            prefab.transform.localScale = new Vector2(100f, 100f);
            prefab.gameObject.SetActive(false);
            if (!m_dicMonsterPrefab.ContainsKey((MonsterType)i)) m_dicMonsterPrefab.Add((MonsterType)i, prefab);
        }      
    }
}
