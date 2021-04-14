using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    None=-1,
    Nomal,Critical,
    Max
}
public class AttackUtil
{
    public static bool CriticalSuccess(float criRate)
    {
        float rate = Random.Range(1f, 101f);
        if(rate < criRate)
        {
            return true;
        }
        return false;
    }
    public static int CriticalDamage(int nomalAttack,int criAttack)
    {
        float damage = nomalAttack + (nomalAttack * criAttack / 10f) + Random.Range(1, 20);
        return Mathf.FloorToInt(damage);
    }
}
