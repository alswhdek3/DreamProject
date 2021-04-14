using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Transform FindChildrenObejct(GameObject obj,string objName)
    {
        var target = obj.GetComponentsInChildren<Transform>();
        for(int i=0; i<target.Length; i++)
        {
            if(target[i].name == objName)
            {
                return target[i];
            }
        }
        return null;
    }
}
