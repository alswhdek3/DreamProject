using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolUnit : MonoBehaviour
{
    private EffectType m_type;

    private IEnumerator Coroutine_StartTimer()
    {
        yield return new WaitForSeconds(0.2f);
        //비활성화 시킨다.
        EffectPool.Instance.AddEffectPoolUnit(m_type, this);
    }
    public EffectType Type { get { return m_type; } }

    public void SetEffect(EffectType type)
    {
        m_type = type;
    }
    public void StartTimer()
    {
        StartCoroutine(Coroutine_StartTimer());
    }
}
