using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Awake()
    {
        ModelFlattener.Instance.FlattenHierarchy(this.gameObject);
    }

}
