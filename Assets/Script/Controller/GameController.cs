using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : SingtonMonoBehaviour<GameController>
{
    [SerializeField] GameObject m_sponPointObject;
    private SponPoint[] m_sponPoint;
    private PlayerController m_player;
    [SerializeField] private BossController[] m_boss;
    //게임 스테이지 , 시간
    private int m_stage=1; //게임 스테이지
    private float m_time; //스테이지 마다 출현 몬스터가 변경된다.
    private float m_showTime; //실제 시간과 UI 상으로 보여주는 시간이 다르다.
    [SerializeField] private float m_bossTime = 10f;
    private bool m_isBoss;
    public int State { get { return m_stage; } }
    public bool IsBoss { get { return m_isBoss; } }
    private IEnumerator Coroutine_GameTimer()
    {
        int min = 0;
        while(true)
        {
            if(m_player.IsAlive) m_showTime += Time.deltaTime;
            if(m_showTime >= 60f)
            {
                min++;
                m_showTime = 0f;
                //UI 데이터 반영
            }
            if(!m_isBoss && m_player.IsAlive)
            {
                m_time += Time.deltaTime;
                if (m_time >= m_bossTime) //보스 등장시간이 되면
                {
                    m_isBoss = true;
                    OpenBoss(); //각 스테이지에 해당하는 보스 활성화
                    m_time = 0f;
                }
            }                      
            yield return null;
        }
    }
    private IEnumerator Coroutine_GamePatten()
    {
        while(true)
        {
            for(int i=0; i<m_sponPoint.Length; i++)
            {
                if(m_sponPoint[i].IsReady && m_player.IsAlive && !m_isBoss) //몬스터 생성준비가 되었으면
                {
                    var monster = MonsterManager.Instance.CreateMonster((MonsterType)m_stage - 1); //n번째 스테이지 해당하는 몬스터 생성
                    monster.transform.position = m_sponPoint[i].transform.position;
                    var distance = m_player.transform.position - transform.position;
                    distance.y = 0f;
                    monster.transform.forward = distance.normalized;
                    if (m_sponPoint[i].transform.position.x > 0f) monster.SetFilp(true);
                    else monster.SetFilp(false);
                    monster.ResetColor();
                    monster.Move();
                    m_sponPoint[i].IsReady = false; //몬스터 생성 시간 다시 초기화
                }
            }
            yield return null;
        }       
    }
    private IEnumerator Coroutine_NextStage()
    {
        yield return new WaitForSeconds(2f);
        m_isBoss = false;
    }
    private void OpenBoss()
    {
        switch(m_stage)
        {
            case 1:
                m_boss[0].gameObject.SetActive(true);
                m_boss[0].StartPatten();
                break;
            case 2:
                break;
        }
    }
    public void NextStage()
    {
        m_stage++;
        StartCoroutine(Coroutine_NextStage());
    }
    protected override void OnStart()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_sponPoint = m_sponPointObject.GetComponentsInChildren<SponPoint>();

        //보스 비활성화
        for (int i = 0; i < m_boss.Length; i++)
            m_boss[i].gameObject.SetActive(false);
        m_boss[0].transform.position = new Vector2(-30f, 40f); //01_보스 등장위치 초기화
        m_isBoss = false;

        StartCoroutine(Coroutine_GameTimer());
        StartCoroutine(Coroutine_GamePatten());       
    }
}
