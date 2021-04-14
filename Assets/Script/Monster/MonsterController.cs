using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    private MonsterAnimController m_animCtr;
    protected SpriteRenderer m_sprite;   
    protected Rigidbody2D m_rigidBody;

    protected MonsterType m_type;
    protected PlayerController m_player;
    protected Status m_status;

    [SerializeField] protected GameObject m_hpBackground;
    [SerializeField] protected Image m_hpBar;
    protected int m_currentHp;

    protected bool m_isAlive;
    protected Transform m_dummyHit;
    protected Transform m_dummyHud;
    public MonsterType Type { get { return m_type; } }
    public Status Status { get { return m_status; } }
    public bool IsAlive { get { return m_isAlive; } }
    public void SetFilp(bool isOnFlip)
    {
        m_sprite.flipX = isOnFlip;
        if (m_type == MonsterType.Monster_02)
        {
            if (m_sprite.flipX) m_dummyHud.localPosition = new Vector2(-0.01f, 0.13f);
            else m_dummyHud.localPosition = new Vector2(0.05f, 0.13f);
        }       
    }
    public void SetMonster(MonsterType type)
    {
        m_type = type;
        m_isAlive = true;      
        if (!m_hpBackground.gameObject.activeInHierarchy) m_hpBackground.gameObject.SetActive(true);
        m_hpBar.fillAmount = 1f;
        m_dummyHud = Util.FindChildrenObejct(gameObject, "Dummy_Hud");
        if(m_dummyHud == null) m_dummyHud = Util.FindChildrenObejct(gameObject, "Dummy_Hud");
        
        m_status = new Status((int)type + 1 * 20, (int)type + 1, (int)type + 1 * 5);
        m_currentHp = m_status.m_hp;
    }
    public void ResetColor() { m_sprite.color = new Color(1f, 1f, 1f); }                     
    public void Move()
    {
        var distance = m_player.transform.position - transform.position; //플레이어와 몬스터의 사이의 거리를 구한다.
        distance.y = 0f; //y 좌표는 영향을 받지 않는다.
        if (m_rigidBody == null) m_rigidBody = GetComponent<Rigidbody2D>();
        if (m_player.IsAlive) m_rigidBody.velocity = distance.normalized * 12f; //플레이어 방향으로 추적한다.
        else m_rigidBody.velocity = Vector2.zero;
    }
    public void SetDamage(int damage) // 추후 데미지 타입에 해당하는 UIText 적용
    {
        if (damage < 0 || m_status.m_hp < 0) return; //잘못된 데미지 형식이 들어오는지 확인 , 이미 몬스터가 죽어있는지 확인      
        m_currentHp -= damage;
        if(m_hpBackground.gameObject.activeInHierarchy && m_hpBar != null) m_hpBar.fillAmount = m_currentHp / (float)m_status.m_hp;
        Debug.LogError("Hp : " + m_currentHp);
        if(m_currentHp <= 0) SetDie();
        else
        {
            if (transform.position.x < 0f && m_type != MonsterType.Boss_01) m_sprite.flipX = true;
            else if(transform.position.x > 0f && m_type != MonsterType.Boss_01) m_sprite.flipX = false;
            if(m_type != MonsterType.Boss_01) Invoke("ResetFlip", 0.5f);
            // 피격 애니메이션 재생                   
            // 피격 사운드 재생
        }
    }
    protected virtual void SetDie()
    {
        //죽는 애니메이션 재생

        m_isAlive = false;
        Invoke("RemoveMonster", 1f);
        //Hp UI 반영
        m_hpBar.fillAmount = 0f;
        m_hpBackground.gameObject.SetActive(false);

        m_sprite.color = Color.red; //피격 당하면 빨간색으로 Sprite 변경           
        m_rigidBody.velocity = Vector2.zero; //추격을 멈춘다.
        m_player.AreaRemoveMonster(this); //몬스터가 죽으면 AttackArea에서 모두 삭제한다.                  
                                          //죽는 사운드 재생
    }
    private void ResetFlip()
    {
        if (m_sprite.flipX) m_sprite.flipX = false;
        else m_sprite.flipX = true;
    }
    private void RemoveMonster()
    {
        if (m_type != MonsterType.Boss_01)
            MonsterManager.Instance.AddPoolUnit(m_type, this);
        else
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            m_player.SetDamage(m_status.m_attack);
            if(m_type != MonsterType.Boss_01)
            {
                m_player.AreaRemoveMonster(this);
                MonsterManager.Instance.AddPoolUnit(m_type, this); //몬스터를 다시 폴링해준다.
            }
        }
    }
    protected void InitData()
    {       
        m_sprite = GetComponent<SpriteRenderer>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_dummyHit = Util.FindChildrenObejct(gameObject, "Dummy_Hit");
    }
    void Awake() { InitData(); }
    private void Start()
    {
        m_animCtr = GetComponent<MonsterAnimController>();      
    }
    private void Update()
    {
        if (!m_player.IsAlive) m_rigidBody.velocity = Vector2.zero;
        if (GameController.Instance.IsBoss) //보스 등장 시 기존에 생성된 일반 몬스터는 죽는다.
        {
            if (m_type != MonsterType.Boss_01)
                SetDie();
        }
    }
}
