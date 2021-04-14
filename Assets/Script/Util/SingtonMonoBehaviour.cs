using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingtonMonoBehaviour<T> : MonoBehaviour where T : SingtonMonoBehaviour<T>
{
    static public T Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = (T)this;
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if(Instance == (T)this)
        {
            OnStart();
        }
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
}
