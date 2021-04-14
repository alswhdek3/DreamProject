using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer m_sprite;
    private PlayerAnimController m_animCtr;
    [SerializeField] private GameObject m_attackAreaObject;
    private AttackArea[] m_attackArea; //스킬에따른 충돌체
    private Dictionary<PlayerAnimState, SkillData> m_dicSkillTable = new Dictionary<PlayerAnimState, SkillData>(); //애니메이션에 해당하는 스킬 데이터를 가져온다.
    private bool m_isAlive;
    //플레이어 HP
    private int m_hp,m_currHp;
    [SerializeField] private GameObject m_hpBackground;
    [SerializeField] private Image m_hpBar;
    public bool IsAlive { get { return m_isAlive; } }
    private void AnimEvent_FinishedMotion() { if (m_animCtr.GetCurrentAnimState != PlayerAnimState.Idel) m_animCtr.Play(PlayerAnimState.Idel); } // 모션이 종료되면 대기 상태로 변경 
    private void InitSkillData()
    {
        m_dicSkillTable.Add(PlayerAnimState.Attack_01, new SkillData() { m_areaIndex = 0, m_attack = 5, m_criticalRate = 20, m_criAttack = 10, m_coolTime = 0 });
        m_dicSkillTable.Add(PlayerAnimState.Attack_02, new SkillData() { m_areaIndex = 1, m_attack = 10, m_criticalRate = 50, m_criAttack = 20, m_coolTime = 0 });
        m_dicSkillTable.Add(PlayerAnimState.Skill_01, new SkillData() { m_areaIndex = 2, m_attack = 20, m_criticalRate = 70, m_criAttack = 30, m_coolTime = 0 });

        //ActionButton 초기화
        ActionManager.Instance.SetButton(ActionButtonType.LeftAttack, LeftAttack);
        ActionManager.Instance.SetButton(ActionButtonType.RightAttack, RightAttack);
        //ActionManager.Instance.SetButton(ActionButtonType.Skill, RightAttack);//추후 SkillButton 구현
    }
    private void LeftAttack()
    {
        var stateInfo = m_animCtr.StateInfo;
        if (m_isAlive)
        {
            m_sprite.flipX = true; // 왼쪽 방향으로 바라보기
            if (!stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) m_animCtr.Play(PlayerAnimState.Attack_01); // 공격상태가 아닐때 바로 공격
            else if (stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) return;   // 공격상태일때 아무 이벤트도 실행하지않고 빠져나간다.  
        }       
    }
    private void RightAttack()
    {
        var stateInfo = m_animCtr.StateInfo;
        if (m_isAlive)
        {
            m_sprite.flipX = false; // 오른쪽 방향으로 바라보기
            if (!stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) m_animCtr.Play(PlayerAnimState.Attack_01); // 공격상태가 아닐때 바로 공격
            else if (stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) return;   // 공격상태일때 아무 이벤트도 실행하지않고 빠져나간다. 
        }
    }
    #region AnimEvent
    private void AnimEvent_Attack()
    {
        SkillData skillData = null;
        m_dicSkillTable.TryGetValue(m_animCtr.GetCurrentAnimState, out skillData); //현재 애니메이션에 해당하는 스킬 데이터를 실시간으로 가져온다.

        if(skillData.m_areaIndex != 2) //궁극기 스킬을 제외한
        {
            if (m_sprite.flipX) m_attackArea[skillData.m_areaIndex].transform.localPosition = new Vector2(-0.7f, 0f);
            else m_attackArea[skillData.m_areaIndex].transform.localPosition = Vector2.zero;
        }

        var unitList = m_attackArea[m_dicSkillTable[m_animCtr.GetCurrentAnimState].m_areaIndex].UnitList; //현재 애니메이션에 해당하는 충돌체의 들어온 몬스터 리스트들을 가져온다.
        for(int i=0; i<unitList.Count; i++)
        {
            var dummy = Util.FindChildrenObejct(unitList[i].gameObject, "Dummy_Hit"); // 몬스터의 "Dummy_Hit" Transform 컴포넌트를 검색
            if (dummy != null) //null값이 아니면
            {
                var monster = unitList[i].GetComponent<MonsterController>();
                //이펙트(몬스터 종류에따른 이펙트가 다르다.)
                EffectType effectType = EffectType.None;
                if (monster.Type == MonsterType.Monster_01 || monster.Type == MonsterType.Monster_02 || monster.Type == MonsterType.Boss_01)
                    effectType = EffectType.Effect_01;
                var effect = EffectPool.Instance.CreateEffect(effectType);
                effect.transform.position = dummy.position;
                var distance = transform.position - effect.transform.position;
                effect.transform.rotation = Quaternion.LookRotation(effect.transform.forward, distance.normalized);
                effect.StartTimer();
                //피격
                int damage = 0;
                DamageType damageType = RandomDamageType(skillData, out damage);
                monster.SetDamage(damage); //추후 UI 개발시 데미지 타입도 같이 전달
            }
        }
    }
    private void AnimEvent_DisablePlayer() { gameObject.SetActive(false); }
    #endregion

    private DamageType RandomDamageType(SkillData skillData,out int damage)
    {
        DamageType type = DamageType.None;
        if(AttackUtil.CriticalSuccess(skillData.m_criticalRate)) //크리티컬 공격에 성공하면
        {
            type = DamageType.Critical;
            damage = AttackUtil.CriticalDamage(skillData.m_attack, skillData.m_criAttack);
        }
        else
        {
            type = DamageType.Nomal;
            damage = skillData.m_attack + Random.Range(5, 10);
        }
        return type;
    }
    public void AreaRemoveMonster(MonsterController monster)
    {
        for(int i=0; i<m_attackArea.Length; i++)
        {
            var unitList = m_attackArea[i].UnitList;
            unitList.Remove(monster.gameObject);
        }
    }
    public void SetDamage(int damage)
    {
        if (damage < 0 || m_currHp <= 0) return;
        else
        {
            m_currHp -= damage;
            m_hpBar.fillAmount = m_currHp / (float)m_hp;
            if (m_currHp <= 0)
            {
                m_isAlive = false;
                m_animCtr.Play(PlayerAnimState.Die);
                m_hpBar.fillAmount = 0f;
                m_hpBackground.gameObject.SetActive(false);
            }
            else
            {
                if (!IsInvoking("ResetColor"))
                {
                    m_sprite.color = Color.red;
                    Invoke("ResetColor", 0.5f);
                }
            }
        }
    }
    private void ResetColor() { m_sprite.color = new Color(1f, 1f, 1f); }
    private void Awake()
    {
        m_isAlive = true;
    }
    private void Start()
    {
        m_sprite = GetComponent<SpriteRenderer>();
        m_animCtr = GetComponent<PlayerAnimController>();
        m_attackArea = m_attackAreaObject.GetComponentsInChildren<AttackArea>();

        InitSkillData();      
        m_hp = 200;
        m_currHp = m_hp;
        m_hpBar.fillAmount = 1f;
    }
    private void Update()
    {
        var stateInfo = m_animCtr.StateInfo;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && m_isAlive) //방향키 왼쪽 버튼 클릭시
        {
            m_sprite.flipX = true; // 왼쪽 방향으로 바라보기
            if (!stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) m_animCtr.Play(PlayerAnimState.Attack_01); // 공격상태가 아닐때 바로 공격
            else if (stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) return;   // 공격상태일때 아무 이벤트도 실행하지않고 빠져나간다.            
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && m_isAlive) //방향키 오른쪽 버튼 클릭시
        {
            m_sprite.flipX = false; // 오른쪽 방향으로 바라보기
            if (!stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) m_animCtr.Play(PlayerAnimState.Attack_01); // 공격상태가 아닐때 바로 공격
            else if (stateInfo.IsName(PlayerAnimState.Attack_01.ToString())) return;   // 공격상태일때 아무 이벤트도 실행하지않고 빠져나간다.            
        }
    }
}
