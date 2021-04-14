using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct  Status
{
    public int m_hp;
    public int m_defence;
    public int m_attack;

    public Status(int hp,int defence, int attack)
    {
        m_hp = hp;
        m_defence = defence;
        m_attack = attack;
    }
}

