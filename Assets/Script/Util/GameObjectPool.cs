using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : class
{
    public delegate T CreateDel();
    private int m_count;
    private CreateDel m_createDel;
    private Queue<T> m_objectQueue = new Queue<T>();

    public GameObjectPool(int count,CreateDel createDel)
    {
        m_count = count;
        m_createDel = createDel;
        AllCreate();
    }
    private void AllCreate()
    {
        for(int i=0; i<m_count; i++)
        {
            var obj = m_createDel();
            m_objectQueue.Enqueue(obj);
        }
    }
    public T Get()
    {
        if(m_objectQueue.Count > 0)
        {
            return m_objectQueue.Dequeue();
        }
        else
        {
            var obj = m_createDel();
            return obj;
        }
    }
    public void Set(T obj)
    {
        m_objectQueue.Enqueue(obj);
    }
}
