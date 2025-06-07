using System;
using UnityEngine;
using System.Linq; 

public static class TransformExtensions
{
    public static Transform FirstChildByQuery(this Transform parent, Func<Transform, bool> predicate)
    {
        return parent.Cast<Transform>().FirstOrDefault(predicate);
    }
}
