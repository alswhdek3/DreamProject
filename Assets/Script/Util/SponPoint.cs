using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SponPoint : MonoBehaviour
{
    private float m_time;
    private float m_duration = 3f; // 추후 스테이지가 높을수록 리스폰되는 시간 감소
    private bool m_isReady = false; //몬스터 생성 준비 완료

    public bool IsReady { get { return m_isReady; } set { m_isReady = value; } }

    private void Update()
    {
        if(!m_isReady)
        {
            m_time += Time.deltaTime;
            if (m_time >= m_duration)
            {
                m_isReady = true;
                m_time = 0f;
            }
        }
    }
}
