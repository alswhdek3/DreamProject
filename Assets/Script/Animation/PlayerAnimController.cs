using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimState
{
    None=-1,
    Idel,Attack_01,Attack_02,Hit,Skill_01,Die,
    Max
}
public class PlayerAnimController : AnimController
{
    private PlayerAnimState m_state;
    public PlayerAnimState GetCurrentAnimState { get { return m_state; } }
    
    public void Play(PlayerAnimState state,bool isBlend=true)
    {
        m_state = state;
        Play(state.ToString(), isBlend);
    }
}
