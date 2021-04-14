using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private List<GameObject> m_unitList = new List<GameObject>();
    public List<GameObject> UnitList { get { return m_unitList; } }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Monster"))
        {
            m_unitList.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Monster"))
        {
            m_unitList.Remove(collision.gameObject);
        }
    }
}
