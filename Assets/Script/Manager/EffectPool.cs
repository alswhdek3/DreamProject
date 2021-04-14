using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    None=-1,
    Effect_01,
    Max
}
public class EffectPool : SingtonMonoBehaviour<EffectPool>
{
    [SerializeField] private GameObject m_effectPrefab;
    private GameObjectPool<EffectPoolUnit> m_effectPool;
    private Dictionary<EffectType, List<EffectPoolUnit>> m_dicEffectPool = new Dictionary<EffectType, List<EffectPoolUnit>>();
    private Dictionary<EffectType, GameObject> m_dicEffectPrefab = new Dictionary<EffectType, GameObject>();

    public EffectPoolUnit CreateEffect(EffectType type)
    {
        if (!m_dicEffectPool.ContainsKey(type)) return null;
        EffectPoolUnit poolUnit = null;
        var pool = m_dicEffectPool[type];
        if(pool.Count > 0) //여유분의 이펙트 리스트가 있으면
        {
            poolUnit = pool[0];
            pool.Remove(poolUnit);
            poolUnit.SetEffect(type);
            poolUnit.gameObject.SetActive(true);
            return poolUnit;
        }
        else
        {
            if (!m_dicEffectPrefab.ContainsKey(type)) return null;
            var prefab = m_dicEffectPrefab[type];
            poolUnit = CreateEffectPoolUnit(type, prefab);
            poolUnit.gameObject.SetActive(true);
            return poolUnit;
        }
    }
    private EffectPoolUnit CreateEffectPoolUnit(EffectType type,GameObject prefab)
    {
        var obj = Instantiate(prefab);
        var poolUnit = obj.GetComponent<EffectPoolUnit>();
        if (poolUnit == null) obj.GetComponent<EffectPoolUnit>();
        poolUnit.SetEffect(type);
        return poolUnit;
    }
    public void AddEffectPoolUnit(EffectType type,EffectPoolUnit poolUnit)
    {
        if (!m_dicEffectPool.ContainsKey(type)) return;
        else
        {
            poolUnit.gameObject.SetActive(false);
            m_dicEffectPool[type].Add(poolUnit);
        }
    }
    protected override void OnStart()
    {
        for(int i=0; i<(int)EffectType.Max; i++)
        {
            List<EffectPoolUnit> effectList = new List<EffectPoolUnit>();
            effectList.Clear();

            m_effectPrefab = Resources.Load("Prefab/Effect/"+(EffectType)i) as GameObject;
            m_effectPool = new GameObjectPool<EffectPoolUnit>(2, () =>
            {
                var obj = Instantiate(m_effectPrefab);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                var poolUnit = obj.GetComponent<EffectPoolUnit>();
                poolUnit.gameObject.SetActive(false);
                effectList.Add(poolUnit);
                return poolUnit;
            });
            if (!m_dicEffectPool.ContainsKey((EffectType)i)) m_dicEffectPool.Add((EffectType)i, effectList); //Dictionary에 타입에 해당하는 이펙트 리스트 추가

            //GameObject 추가
            var prefab = Instantiate(m_effectPrefab);
            prefab.transform.SetParent(transform);
            prefab.transform.localPosition = Vector3.zero;
            prefab.gameObject.SetActive(false);
            if (!m_dicEffectPrefab.ContainsKey((EffectType)i)) m_dicEffectPrefab.Add((EffectType)i, prefab); //Dictionary에 타입에 해당하는 게임오브젝트 추가
        }
    }
}
