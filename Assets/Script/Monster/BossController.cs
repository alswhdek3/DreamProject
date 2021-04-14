using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonsterController
{
    [SerializeField] private GameObject m_boltPrefab;
    [SerializeField] Transform[] m_blotPos;   
    private GameObjectPool<BoltController> m_boltPool;  

    //좌우 움직인 체크
    private float m_time = 0f;
    private bool m_isLeft = true;
    
    private IEnumerator Coroutine_BossDisable()
    {
        yield return new WaitForSeconds(5f);
        GameController.Instance.NextStage();
        gameObject.SetActive(false);
    }
    private IEnumerator Coroutine_MovePatten()
    {       
        while (true)
        {
            float duration = Random.Range(1f, 2f);
            m_time += Time.deltaTime;
            //yield return new WaitForSeconds(Random.Range(1f, 2f)); //3~5초에 한번씩 좌우로 움직인다.
            if(m_time >= duration)
            {
                if (m_isLeft) //왼쪽에 있으면
                    m_rigidBody.velocity = Vector2.right * 10f; //오른쪽 으로 이동
                else if (!m_isLeft) //오른쪽에 있으면
                    m_rigidBody.velocity = Vector2.left * 10f; //왼쪽 으로 이동
            }           
            if(m_isLeft)
            {
                if(transform.position.x >= 40f)
                {
                    m_rigidBody.velocity = Vector2.zero;
                    m_isLeft = false;
                    m_time = 0f;
                }
            }
            else if (!m_isLeft)
            {
                if (transform.position.x <= -40f)
                {
                    m_rigidBody.velocity = Vector2.zero;
                    m_isLeft = true;
                    m_time = 0f;
                }
            }
            if(!m_player.IsAlive)
            {
                m_rigidBody.velocity = Vector2.zero;
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator Coroutine_AttackPatten()
    {
        float timer = 0f;
        float duration = 0f;
        while (true)
        {
            duration = Random.Range(1.5f, 2.5f);
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                for (int i = 0; i < m_blotPos.Length; i++)
                {
                    //총알 생성
                    var bullet = CreateBullet();
                    bullet.transform.position = m_blotPos[i].transform.position;
                    var distance = m_player.transform.position - bullet.transform.position;
                    //distance.y = 0f;                   
                    var rigidBody = bullet.GetComponent<Rigidbody2D>();
                    if (rigidBody == null) rigidBody = bullet.GetComponent<Rigidbody2D>();
                    rigidBody.velocity = distance.normalized * 30f;
                }
                timer = 0f;
            }
            if (!m_player.IsAlive) yield break;
            yield return null;
        }
    }
    private BoltController CreateBullet()
    {
        var bullet = m_boltPool.Get();
        bullet.transform.SetParent(transform);
        bullet.gameObject.SetActive(true);
        bullet.SetBolt(this);
        return bullet;
    }
    public void RemoveBullet(BoltController bullet)
    {
        bullet.gameObject.SetActive(false);
        m_boltPool.Set(bullet);
    }
    public void StartPatten()
    {
        StartCoroutine(Coroutine_MovePatten());
        StartCoroutine(Coroutine_AttackPatten());
    }
    protected override void SetDie()
    {
        m_isAlive = false;
        StopAllCoroutines();
        m_rigidBody.velocity = Vector2.zero;
        m_sprite.color = Color.red;
        m_rigidBody.velocity = Vector2.up * 20f;
        StartCoroutine(Coroutine_BossDisable());
    }
    protected virtual void InitBullet()
    {
        m_boltPrefab = Resources.Load("Prefab/Bullt/Bullet_01") as GameObject;
        m_boltPool = new GameObjectPool<BoltController>(2, () =>
        {
            var obj = Instantiate(m_boltPrefab);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector2.zero;
            var bullet = obj.GetComponent<BoltController>();
            bullet.gameObject.SetActive(false);
            return bullet;
        });
    }
    protected virtual void InitBossStatus()
    {
        m_type = MonsterType.Boss_01;
        m_status = new Status((int)m_type + 1 * 200, (int)m_type + 2, (int)m_type + 1 * 10);
        m_currentHp = m_status.m_hp;
        m_hpBar.fillAmount = 1f;
    }
    void Start()
    {
        InitData();
        InitBullet();
        m_isAlive = true;
        InitBossStatus();        
    }    
}
