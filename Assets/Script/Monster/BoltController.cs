using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    private BossController m_boss;

    public void SetBolt(BossController boss) { m_boss = boss; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if(player != null)
            {
                int damage = m_boss.Status.m_attack + Random.Range(5, 10); 
                player.SetDamage(damage);
                if (m_boss != null) m_boss.RemoveBullet(this);
            }
        }
    }
}
