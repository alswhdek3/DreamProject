using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    int[] number = new int[10] { 1, 2, 3, 5, 6, 7, 9, 55, 34, 77 };
    int prevNum;
    int[] result = new int[10];

    private void Start()
    {
        prevNum = number[0];
        for(int i=0; i<number.Length; i++)
        {
            if(i!=0)
            {
                result[i] = prevNum + number[i];
                Debug.LogError("Result[" + i + "]" + result[i]);
                prevNum = number[i];
            }
        }
    }
}