using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterAnimState
{
    None=-1,
    Idel,Trace,Hit,Die,
    Max
}
public class MonsterAnimController : AnimController
{
    private MonsterAnimState m_state;
    public MonsterAnimState GetCurrentAnimState { get { return m_state; } }
    
    public void Play(MonsterAnimState state,bool isBlend=true)
    {
        m_state = state;
        Play(state, isBlend);
    }
}
