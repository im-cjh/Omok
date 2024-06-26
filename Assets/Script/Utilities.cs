﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


static public class Utilities
{
    public static GameObject FindGameObject(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj == null)
        {
            Debug.LogError($"{path} 오브젝트를 찾을 수 없습니다.");
        }
        return obj;
    }


    static public T FindAndAssign<T>(string path) where T : Component
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"{path} 오브젝트에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다.");
            }
            return component;
        }
        else
        {
            Debug.LogError($"{path} 오브젝트를 찾을 수 없습니다.");
            return null;
        }
    }
}

