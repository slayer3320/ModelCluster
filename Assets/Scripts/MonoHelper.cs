using System;
using System.Collections;
using System.Collections.Generic;using UnityEngine;

public class MonoHelper : MonoBehaviour
{
    public static MonoHelper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("MonoHelper");
                instance = obj.AddComponent<MonoHelper>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    public static MonoHelper instance;
    
    
}

